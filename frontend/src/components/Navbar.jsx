import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { Bell, CheckCheck, User, LogOut, Sun, Moon, Menu } from 'lucide-react';
import api from '../api/axios';

export const Navbar = ({ mobileMenuOpen, onToggleMobileMenu }) => {
  const { user, logout } = useAuth();
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [showDropdown, setShowDropdown] = useState(false);
  const [theme, setTheme] = useState(() => localStorage.getItem('theme') || 'light');

  useEffect(() => {
    if (theme === 'dark') {
      document.documentElement.setAttribute('data-theme', 'dark');
    } else {
      document.documentElement.removeAttribute('data-theme');
    }
    localStorage.setItem('theme', theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => (prev === 'light' ? 'dark' : 'light'));
  };

  const fetchNotifications = async () => {
    try {
      const [listRes, countRes] = await Promise.all([
        api.get('/notifications'),
        api.get('/notifications/unread-count')
      ]);
      setNotifications(listRes.data);
      setUnreadCount(countRes.data.count);
    } catch (err) {
      console.error('Failed to fetch notifications', err);
    }
  };

  useEffect(() => {
    if (user) {
      fetchNotifications();
      const interval = setInterval(fetchNotifications, 15000); // Polling every 15s
      return () => clearInterval(interval);
    }
  }, [user]);

  const handleMarkAsRead = async (id) => {
    try {
      await api.patch(`/notifications/${id}/read`);
      fetchNotifications();
    } catch (err) {
      console.error(err);
    }
  };

  const handleMarkAllRead = async () => {
    try {
      await api.patch('/notifications/read-all');
      fetchNotifications();
    } catch (err) {
      console.error(err);
    }
  };

  const getRoleBadge = (role) => {
    if (role === 'Admin') return 'badge-role-admin';
    if (role === 'Manager') return 'badge-role-manager';
    return 'badge-role-user';
  };

  return (
    <header className="glass-panel" style={{ borderRadius: 0, borderTop: 0, borderLeft: 0, borderRight: 0, padding: '12px 20px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', zIndex: 100, position: 'sticky', top: 0 }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
        <button
          className="mobile-menu-btn"
          onClick={onToggleMobileMenu}
          title="Toggle navigation"
        >
          <Menu size={20} />
        </button>
        <h2 className="navbar-logo-text" style={{ fontSize: '1.25rem', fontWeight: 800, background: 'linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
          TaskPulse
        </h2>
      </div>

      <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
        {/* Theme Toggle Button */}
        <button
          onClick={toggleTheme}
          className="btn btn-secondary btn-sm"
          style={{ borderRadius: '50%', width: '40px', height: '40px', padding: 0 }}
          title={theme === 'light' ? 'Switch to Dark Theme' : 'Switch to Light Theme'}
        >
          {theme === 'light' ? <Moon size={18} /> : <Sun size={18} />}
        </button>
        {/* Notifications Dropdown */}
        <div style={{ position: 'relative' }}>
          <button 
            onClick={() => setShowDropdown(!showDropdown)} 
            className="btn btn-secondary btn-sm"
            style={{ position: 'relative', borderRadius: '50%', width: '40px', height: '40px', padding: 0 }}
            title="Notifications"
          >
            <Bell size={18} />
            {unreadCount > 0 && (
              <span style={{ position: 'absolute', top: '-4px', right: '-4px', background: '#f43f5e', color: 'white', fontSize: '0.65rem', fontWeight: 'bold', width: '18px', height: '18px', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', boxShadow: '0 0 8px rgba(244,63,94,0.6)' }}>
                {unreadCount}
              </span>
            )}
          </button>

          {showDropdown && (
            <div className="glass-card notification-dropdown-card">
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px', paddingBottom: '8px', borderBottom: '1px solid var(--border-color)' }}>
                <span style={{ fontWeight: 700, fontSize: '0.9rem' }}>Notifications</span>
                {unreadCount > 0 && (
                  <button onClick={handleMarkAllRead} style={{ background: 'none', border: 'none', color: 'var(--primary)', fontSize: '0.75rem', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: '4px' }}>
                    <CheckCheck size={14} /> Mark all read
                  </button>
                )}
              </div>

              {notifications.length === 0 ? (
                <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', textAlign: 'center', padding: '16px 0' }}>No notifications yet</p>
              ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                  {notifications.map((n) => (
                    <div key={n.id} onClick={() => !n.isRead && handleMarkAsRead(n.id)} style={{ padding: '10px 12px', borderRadius: '8px', background: n.isRead ? 'transparent' : 'rgba(99, 102, 241, 0.1)', border: '1px solid ' + (n.isRead ? 'transparent' : 'rgba(99, 102, 241, 0.2)'), cursor: n.isRead ? 'default' : 'pointer' }}>
                      <p style={{ fontSize: '0.85rem', color: n.isRead ? 'var(--text-muted)' : 'var(--text-main)', fontWeight: n.isRead ? 400 : 600 }}>{n.message}</p>
                      <span style={{ fontSize: '0.7rem', color: 'var(--text-dark)', marginTop: '4px', display: 'block' }}>{new Date(n.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>

        {/* User profile */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', paddingLeft: '12px', borderLeft: '1px solid var(--border-color)' }}>
          <div style={{ width: '36px', height: '36px', borderRadius: '50%', background: 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'white', fontWeight: 700, fontSize: '0.9rem', flexShrink: 0 }}>
            {user?.fullName?.charAt(0) || 'U'}
          </div>
          <div className="navbar-user-text" style={{ display: 'flex', flexDirection: 'column' }}>
            <span style={{ fontSize: '0.85rem', fontWeight: 700 }}>{user?.fullName}</span>
            <span className={`badge ${getRoleBadge(user?.role)}`} style={{ alignSelf: 'flex-start', marginTop: '2px', fontSize: '0.65rem' }}>
              {user?.role}
            </span>
          </div>
        </div>
      </div>
    </header>
  );
};
