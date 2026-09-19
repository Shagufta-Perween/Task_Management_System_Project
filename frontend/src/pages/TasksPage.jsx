import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { Plus, MessageSquare, Calendar, User, Trash2, Edit3, Filter, CheckCircle } from 'lucide-react';
import api from '../api/axios';

export const TasksPage = () => {
  const { user } = useAuth();
  const [tasks, setTasks] = useState([]);
  const [teams, setTeams] = useState([]);
  const [assignableUsers, setAssignableUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  // Filters
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');

  // Modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedTaskComments, setSelectedTaskComments] = useState(null); // taskId for comment modal
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');

  // Form State
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState('Medium');
  const [deadline, setDeadline] = useState('');
  const [teamId, setTeamId] = useState('');
  const [assignedToId, setAssignedToId] = useState('');

  const fetchTasks = async () => {
    setLoading(true);
    try {
      const params = {};
      if (statusFilter) params.status = statusFilter;
      if (priorityFilter) params.priority = priorityFilter;

      const res = await api.get('/tasks', { params });
      setTasks(res.data);
    } catch (err) {
      console.error('Failed to fetch tasks', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchMetaData = async () => {
    try {
      const [teamsRes, usersRes] = await Promise.all([
        api.get('/teams'),
        (user?.role === 'Admin' || user?.role === 'Manager') ? api.get('/users/all') : Promise.resolve({ data: [] })
      ]);
      setTeams(teamsRes.data);
      setAssignableUsers(usersRes.data);

      if (teamsRes.data.length > 0) {
        setTeamId((prev) => prev || teamsRes.data[0].id.toString());
      }
      if (usersRes.data.length > 0) {
        setAssignedToId((prev) => prev || usersRes.data[0].id);
      }
    } catch (err) {
      console.error('Failed to fetch metadata', err);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, [statusFilter, priorityFilter]);

  useEffect(() => {
    fetchMetaData();
  }, [user]);

  const handleOpenCreateModal = () => {
    fetchMetaData();
    setShowCreateModal(true);
  };

  const handleCreateTask = async (e) => {
    e.preventDefault();
    try {
      await api.post('/tasks', {
        title,
        description,
        priority,
        deadline: deadline ? new Date(deadline).toISOString() : null,
        teamId: parseInt(teamId),
        assignedToId
      });
      setShowCreateModal(false);
      setTitle('');
      setDescription('');
      fetchTasks();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to create task');
    }
  };

  const handleStatusChange = async (taskId, newStatus) => {
    try {
      await api.patch(`/tasks/${taskId}/status`, { status: newStatus });
      fetchTasks();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to update task status');
    }
  };

  const handleDeleteTask = async (taskId) => {
    if (!window.confirm('Are you sure you want to delete this task?')) return;
    try {
      await api.delete(`/tasks/${taskId}`);
      fetchTasks();
    } catch (err) {
      alert(err.response?.data?.message || 'Failed to delete task');
    }
  };

  const openComments = async (taskId) => {
    setSelectedTaskComments(taskId);
    try {
      const res = await api.get(`/tasks/${taskId}/comments`);
      setComments(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const handleAddComment = async (e) => {
    e.preventDefault();
    if (!newComment.trim()) return;
    try {
      await api.post(`/tasks/${selectedTaskComments}/comments`, { content: newComment });
      setNewComment('');
      openComments(selectedTaskComments);
      fetchTasks();
    } catch (err) {
      alert('Failed to add comment');
    }
  };

  const renderTaskColumn = (statusTitle, statusValue, badgeClass) => {
    const colTasks = tasks.filter(t => t.status === statusValue);
    return (
      <div className="glass-card" style={{ padding: '20px', flex: 1, minWidth: '300px', display: 'flex', flexDirection: 'column' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px', paddingBottom: '12px', borderBottom: '1px solid var(--border-color)' }}>
          <h3 style={{ fontSize: '1rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: '8px' }}>
            <span className={`badge ${badgeClass}`}>{colTasks.length}</span> {statusTitle}
          </h3>
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', gap: '14px', flex: 1 }}>
          {colTasks.map((task) => (
            <div key={task.id} className="glass-panel" style={{ padding: '16px', borderLeft: '4px solid ' + (statusValue === 'Done' ? '#34d399' : statusValue === 'InProgress' ? '#38bdf8' : '#fbbf24') }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '8px' }}>
                <h4 style={{ fontSize: '0.95rem', fontWeight: 700 }}>{task.title}</h4>
                <span className={`badge badge-${task.priority.toLowerCase()}`} style={{ fontSize: '0.65rem' }}>{task.priority}</span>
              </div>

              {task.description && (
                <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', marginBottom: '12px' }}>{task.description}</p>
              )}

              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', fontSize: '0.75rem', color: 'var(--text-dark)', marginBottom: '12px' }}>
                <span style={{ display: 'flex', alignItems: 'center', gap: '4px' }}><User size={14} /> {task.assignedToName}</span>
                {task.deadline && (
                  <span style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <Calendar size={14} /> {new Date(task.deadline).toLocaleDateString()}
                  </span>
                )}
              </div>

              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', paddingTop: '10px', borderTop: '1px solid var(--border-color)' }}>
                {/* Status Switcher */}
                <select
                  className="form-select"
                  style={{ width: 'auto', padding: '4px 8px', fontSize: '0.75rem' }}
                  value={task.status}
                  onChange={(e) => handleStatusChange(task.id, e.target.value)}
                >
                  <option value="ToDo">To Do</option>
                  <option value="InProgress">In Progress</option>
                  <option value="Done">Done</option>
                </select>

                <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                  <button onClick={() => openComments(task.id)} className="btn btn-secondary btn-sm" style={{ padding: '4px 8px', fontSize: '0.75rem' }}>
                    <MessageSquare size={14} /> {task.commentCount}
                  </button>

                  {(user?.role === 'Admin' || user?.role === 'Manager') && (
                    <button onClick={() => handleDeleteTask(task.id)} className="btn btn-danger btn-sm" style={{ padding: '4px 6px' }}>
                      <Trash2 size={14} />
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  };

  return (
    <div className="page-container">
      {/* Header */}
      <div className="mobile-header-stack" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
        <div>
          <h1 style={{ fontSize: '1.75rem', fontWeight: 800 }}>Tasks</h1>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem', marginTop: '4px' }}>
            Track, assign, and manage workflow progress
          </p>
        </div>

        {(user?.role === 'Admin' || user?.role === 'Manager') && (
          <button onClick={handleOpenCreateModal} className="btn btn-primary">
            <Plus size={18} /> New Task
          </button>
        )}
      </div>

      {/* Filter bar */}
      <div className="glass-card filter-bar-container">
        <div className="filter-bar-header">
          <Filter size={16} /> Filters:
        </div>
        <select className="form-select filter-select-item" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All Statuses</option>
          <option value="ToDo">To Do</option>
          <option value="InProgress">In Progress</option>
          <option value="Done">Done</option>
        </select>
        <select className="form-select filter-select-item" value={priorityFilter} onChange={(e) => setPriorityFilter(e.target.value)}>
          <option value="">All Priorities</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>
      </div>

      {/* Kanban Board Columns */}
      <div style={{ display: 'flex', gap: '20px', overflowX: 'auto', paddingBottom: '16px' }}>
        {renderTaskColumn('To Do', 'ToDo', 'badge-todo')}
        {renderTaskColumn('In Progress', 'InProgress', 'badge-inprogress')}
        {renderTaskColumn('Done', 'Done', 'badge-done')}
      </div>

      {/* Create Task Modal */}
      {showCreateModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h2 style={{ fontSize: '1.25rem', fontWeight: 800, marginBottom: '20px' }}>Create New Task</h2>
            <form onSubmit={handleCreateTask}>
              <div className="form-group">
                <label className="form-label">Task Title</label>
                <input className="form-input" value={title} onChange={(e) => setTitle(e.target.value)} required placeholder="Implementation of API..." />
              </div>

              <div className="form-group">
                <label className="form-label">Description</label>
                <textarea className="form-textarea" rows={3} value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Provide task details..." />
              </div>

              <div className="form-grid-2" style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
                <div className="form-group">
                  <label className="form-label">Priority</label>
                  <select className="form-select" value={priority} onChange={(e) => setPriority(e.target.value)}>
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                  </select>
                </div>

                <div className="form-group">
                  <label className="form-label">Deadline</label>
                  <input type="date" className="form-input" value={deadline} onChange={(e) => setDeadline(e.target.value)} />
                </div>
              </div>

              <div className="form-group">
                <label className="form-label">Select Team</label>
                <select className="form-select" value={teamId} onChange={(e) => setTeamId(e.target.value)} required>
                  {teams.length === 0 ? (
                    <option value="">No teams available</option>
                  ) : (
                    teams.map((t) => (
                      <option key={t.id} value={t.id.toString()}>
                        {t.name}
                      </option>
                    ))
                  )}
                </select>
                {teams.length === 0 && (
                  <p style={{ fontSize: '0.8rem', color: '#fb7185', marginTop: '4px' }}>
                    ⚠️ No teams exist. Please create a team first in the Teams tab!
                  </p>
                )}
              </div>

              <div className="form-group">
                <label className="form-label">Assign To Member</label>
                <select className="form-select" value={assignedToId} onChange={(e) => setAssignedToId(e.target.value)} required>
                  {assignableUsers.length === 0 ? (
                    <option value="">No users available</option>
                  ) : (
                    assignableUsers.map((u) => (
                      <option key={u.id} value={u.id}>
                        {u.fullName} ({u.role})
                      </option>
                    ))
                  )}
                </select>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', marginTop: '24px' }}>
                <button type="button" onClick={() => setShowCreateModal(false)} className="btn btn-secondary">Cancel</button>
                <button type="submit" className="btn btn-primary">Create Task</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Comments Modal */}
      {selectedTaskComments && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
              <h3 style={{ fontSize: '1.1rem', fontWeight: 700 }}>Task Comments</h3>
              <button onClick={() => setSelectedTaskComments(null)} style={{ background: 'none', border: 'none', color: 'var(--text-muted)', cursor: 'pointer', fontSize: '1.2rem' }}>✕</button>
            </div>

            <div style={{ maxHeight: '300px', overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: '12px', marginBottom: '20px' }}>
              {comments.length === 0 ? (
                <p style={{ color: 'var(--text-muted)', fontSize: '0.85rem', textAlign: 'center' }}>No comments yet. Be the first to comment!</p>
              ) : (
                comments.map((c) => (
                  <div key={c.id} style={{ background: 'var(--bg-input)', padding: '10px 14px', borderRadius: '8px', border: '1px solid var(--border-color)' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '4px', fontSize: '0.8rem', fontWeight: 700, color: 'var(--primary)' }}>
                      <span>{c.userName}</span>
                      <span style={{ color: 'var(--text-dark)', fontWeight: 400 }}>{new Date(c.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                    </div>
                    <p style={{ fontSize: '0.85rem' }}>{c.content}</p>
                  </div>
                ))
              )}
            </div>

            <form onSubmit={handleAddComment} style={{ display: 'flex', gap: '8px' }}>
              <input className="form-input" value={newComment} onChange={(e) => setNewComment(e.target.value)} placeholder="Write a comment..." required />
              <button type="submit" className="btn btn-primary btn-sm">Post</button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
