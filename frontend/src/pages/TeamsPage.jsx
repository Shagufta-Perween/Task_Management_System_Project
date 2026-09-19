import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { Users, Plus, UserPlus, Trash2, Shield } from 'lucide-react';
import api from '../api/axios';

export const TeamsPage = () => {
  const { user } = useAuth();
  const [teams, setTeams] = useState([]);
  const [allUsers, setAllUsers] = useState([]);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedTeamForAdd, setSelectedTeamForAdd] = useState(null);
  
  // Form state
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [selectedUserId, setSelectedUserId] = useState('');

  const fetchTeams = async () => {
    try {
      const res = await api.get('/teams');
      setTeams(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchUsers = async () => {
    if (user?.role === 'Admin' || user?.role === 'Manager') {
      try {
        const res = await api.get('/users/all');
        setAllUsers(res.data);
        if (res.data.length > 0) setSelectedUserId(res.data[0].id);
      } catch (err) {
        console.error(err);
      }
    }
  };

  useEffect(() => {
    fetchTeams();
    fetchUsers();
  }, [user]);

  const handleCreateTeam = async (e) => {
    e.preventDefault();
    try {
      await api.post('/teams', { name, description });
      setShowCreateModal(false);
      setName('');
      setDescription('');
      fetchTeams();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to create team');
    }
  };

  const handleAddMember = async (e) => {
    e.preventDefault();
    if (!selectedUserId) return;
    try {
      await api.post(`/teams/${selectedTeamForAdd}/members`, { userId: selectedUserId });
      setSelectedTeamForAdd(null);
      fetchTeams();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to add member to team');
    }
  };

  const handleRemoveMember = async (teamId, memberUserId) => {
    if (!window.confirm('Remove member from team?')) return;
    try {
      await api.delete(`/teams/${teamId}/members/${memberUserId}`);
      fetchTeams();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to remove member');
    }
  };

  return (
    <div className="page-container">
      {/* Header */}
      <div className="mobile-header-stack" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
        <div>
          <h1 style={{ fontSize: '1.75rem', fontWeight: 800 }}>Teams</h1>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem', marginTop: '4px' }}>
            Organize users into functional workgroups
          </p>
        </div>

        {(user?.role === 'Admin' || user?.role === 'Manager') && (
          <button onClick={() => setShowCreateModal(true)} className="btn btn-primary">
            <Plus size={18} /> Create Team
          </button>
        )}
      </div>

      {/* Team Cards Grid */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(340px, 1fr))', gap: '24px' }}>
        {teams.map((team) => (
          <div key={team.id} className="glass-card" style={{ padding: '24px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '12px' }}>
              <div>
                <h3 style={{ fontSize: '1.1rem', fontWeight: 700 }}>{team.name}</h3>
                <span style={{ fontSize: '0.75rem', color: 'var(--text-dark)' }}>Created by {team.createdByName}</span>
              </div>
              <span className="badge badge-inprogress">{team.memberCount} Members</span>
            </div>

            <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', marginBottom: '20px' }}>
              {team.description || 'No description provided.'}
            </p>

            <div style={{ borderTop: '1px solid var(--border-color)', paddingTop: '16px' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
                <span style={{ fontSize: '0.8rem', fontWeight: 700, color: 'var(--text-muted)', textTransform: 'uppercase' }}>Members</span>
                {(user?.role === 'Admin' || user?.role === 'Manager') && (
                  <button onClick={() => setSelectedTeamForAdd(team.id)} className="btn btn-secondary btn-sm" style={{ fontSize: '0.75rem', padding: '4px 8px' }}>
                    <UserPlus size={14} /> Add
                  </button>
                )}
              </div>

              <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                {team.members?.map((m) => (
                  <div key={m.userId} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '6px 10px', background: 'var(--bg-input)', borderRadius: '6px', fontSize: '0.85rem' }}>
                    <div>
                      <span style={{ fontWeight: 600 }}>{m.fullName}</span>
                      <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)', marginLeft: '8px' }}>({m.role})</span>
                    </div>
                    {(user?.role === 'Admin' || user?.role === 'Manager') && (
                      <button onClick={() => handleRemoveMember(team.id, m.userId)} style={{ background: 'none', border: 'none', color: '#fb7185', cursor: 'pointer' }} title="Remove">
                        <Trash2 size={14} />
                      </button>
                    )}
                  </div>
                ))}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Create Team Modal */}
      {showCreateModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h2 style={{ fontSize: '1.25rem', fontWeight: 800, marginBottom: '20px' }}>Create New Team</h2>
            <form onSubmit={handleCreateTeam}>
              <div className="form-group">
                <label className="form-label">Team Name</label>
                <input className="form-input" value={name} onChange={(e) => setName(e.target.value)} placeholder="Frontend Engineering" required />
              </div>
              <div className="form-group">
                <label className="form-label">Description</label>
                <textarea className="form-textarea" rows={3} value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Responsible for client web apps..." />
              </div>
              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '24px' }}>
                <button type="button" onClick={() => setShowCreateModal(false)} className="btn btn-secondary">Cancel</button>
                <button type="submit" className="btn btn-primary">Create</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Add Member Modal */}
      {selectedTeamForAdd && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h2 style={{ fontSize: '1.25rem', fontWeight: 800, marginBottom: '20px' }}>Add Team Member</h2>
            <form onSubmit={handleAddMember}>
              <div className="form-group">
                <label className="form-label">Select User</label>
                <select className="form-select" value={selectedUserId} onChange={(e) => setSelectedUserId(e.target.value)}>
                  {allUsers.map((u) => (
                    <option key={u.id} value={u.id}>{u.fullName} ({u.email} - {u.role})</option>
                  ))}
                </select>
              </div>
              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '24px' }}>
                <button type="button" onClick={() => setSelectedTeamForAdd(null)} className="btn btn-secondary">Cancel</button>
                <button type="submit" className="btn btn-primary">Add Member</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
