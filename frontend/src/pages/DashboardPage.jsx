import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { CheckCircle2, Clock, AlertTriangle, ListTodo, Filter, Plus, TrendingUp } from 'lucide-react';
import api from '../api/axios';
import { Link } from 'react-router-dom';

export const DashboardPage = () => {
  const { user } = useAuth();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  
  // Filters
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');

  const fetchDashboard = async () => {
    setLoading(true);
    try {
      const params = {};
      if (statusFilter) params.status = statusFilter;
      if (priorityFilter) params.priority = priorityFilter;

      const res = await api.get('/dashboard', { params });
      setStats(res.data);
    } catch (err) {
      console.error('Failed to load dashboard', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDashboard();
  }, [statusFilter, priorityFilter]);

  const completionPercentage = stats?.totalTasks > 0
    ? Math.round((stats.doneCount / stats.totalTasks) * 100)
    : 0;

  return (
    <div className="page-container">
      {/* Header */}
      <div className="mobile-header-stack" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '28px' }}>
        <div>
          <h1 style={{ fontSize: '1.75rem', fontWeight: 800 }}>
            Hello, {user?.fullName?.split(' ')[0]} 👋
          </h1>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem', marginTop: '4px' }}>
            Here is your task performance and team overview
          </p>
        </div>

        {(user?.role === 'Admin' || user?.role === 'Manager') && (
          <Link to="/tasks" className="btn btn-primary">
            <Plus size={18} /> Create Task
          </Link>
        )}
      </div>

      {/* Filter Toolbar */}
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

      {/* Metric Cards Grid */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: '20px', marginBottom: '28px' }}>
        <div className="glass-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: 'var(--text-muted)', fontSize: '0.85rem', fontWeight: 600 }}>
            Total Tasks
            <ListTodo size={20} color="#818cf8" />
          </div>
          <div style={{ fontSize: '2rem', fontWeight: 800, marginTop: '10px' }}>{stats?.totalTasks || 0}</div>
        </div>

        <div className="glass-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: 'var(--text-muted)', fontSize: '0.85rem', fontWeight: 600 }}>
            To Do
            <Clock size={20} color="#fbbf24" />
          </div>
          <div style={{ fontSize: '2rem', fontWeight: 800, marginTop: '10px', color: '#fbbf24' }}>{stats?.todoCount || 0}</div>
        </div>

        <div className="glass-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: 'var(--text-muted)', fontSize: '0.85rem', fontWeight: 600 }}>
            In Progress
            <TrendingUp size={20} color="#38bdf8" />
          </div>
          <div style={{ fontSize: '2rem', fontWeight: 800, marginTop: '10px', color: '#38bdf8' }}>{stats?.inProgressCount || 0}</div>
        </div>

        <div className="glass-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: 'var(--text-muted)', fontSize: '0.85rem', fontWeight: 600 }}>
            Completed
            <CheckCircle2 size={20} color="#34d399" />
          </div>
          <div style={{ fontSize: '2rem', fontWeight: 800, marginTop: '10px', color: '#34d399' }}>{stats?.doneCount || 0}</div>
        </div>

        <div className="glass-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: 'var(--text-muted)', fontSize: '0.85rem', fontWeight: 600 }}>
            Overdue
            <AlertTriangle size={20} color="#fb7185" />
          </div>
          <div style={{ fontSize: '2rem', fontWeight: 800, marginTop: '10px', color: '#fb7185' }}>{stats?.overdueTasks || 0}</div>
        </div>
      </div>

      {/* Completion Progress Bar */}
      <div className="glass-card" style={{ padding: '20px', marginBottom: '28px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px', fontWeight: 700, fontSize: '0.9rem' }}>
          <span>Completion Progress</span>
          <span style={{ color: 'var(--primary)' }}>{completionPercentage}%</span>
        </div>
        <div style={{ width: '100%', height: '10px', background: 'var(--bg-input)', borderRadius: '5px', overflow: 'hidden' }}>
          <div style={{ width: `${completionPercentage}%`, height: '100%', background: 'linear-gradient(90deg, #6366f1 0%, #34d399 100%)', transition: 'width 0.5s ease' }} />
        </div>
      </div>

      {/* Recent Tasks List */}
      <div className="glass-card" style={{ padding: '24px' }}>
        <h3 style={{ fontSize: '1.1rem', fontWeight: 700, marginBottom: '16px' }}>Recent Tasks</h3>
        {stats?.recentTasks?.length === 0 ? (
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem', textAlign: 'center', padding: '24px 0' }}>No tasks found matching your filters.</p>
        ) : (
          <div style={{ overflowX: 'auto' }}>
            <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '0.9rem' }}>
              <thead>
                <tr style={{ borderBottom: '1px solid var(--border-color)', color: 'var(--text-muted)', fontSize: '0.8rem', textTransform: 'uppercase' }}>
                  <th style={{ padding: '12px' }}>Title</th>
                  <th style={{ padding: '12px' }}>Status</th>
                  <th style={{ padding: '12px' }}>Priority</th>
                  <th style={{ padding: '12px' }}>Assigned To</th>
                  <th style={{ padding: '12px' }}>Team</th>
                </tr>
              </thead>
              <tbody>
                {stats?.recentTasks?.map((t) => (
                  <tr key={t.id} style={{ borderBottom: '1px solid var(--border-color)' }}>
                    <td style={{ padding: '12px', fontWeight: 600 }}>{t.title}</td>
                    <td style={{ padding: '12px' }}>
                      <span className={`badge badge-${t.status.toLowerCase()}`}>{t.status}</span>
                    </td>
                    <td style={{ padding: '12px' }}>
                      <span className={`badge badge-${t.priority.toLowerCase()}`}>{t.priority}</span>
                    </td>
                    <td style={{ padding: '12px', color: 'var(--text-muted)' }}>{t.assignedToName || 'Unassigned'}</td>
                    <td style={{ padding: '12px', color: 'var(--text-muted)' }}>{t.teamName || 'N/A'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};
