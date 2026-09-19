import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LayoutDashboard, CheckSquare, Users, ShieldCheck, LogOut } from 'lucide-react';

export const Sidebar = ({ mobileMenuOpen, onCloseMobileMenu }) => {
  const { user, logout } = useAuth();

  const navItems = [
    { label: 'Dashboard', path: '/dashboard', icon: LayoutDashboard, roles: ['Admin', 'Manager', 'User'] },
    { label: 'Tasks', path: '/tasks', icon: CheckSquare, roles: ['Admin', 'Manager', 'User'] },
    { label: 'Teams', path: '/teams', icon: Users, roles: ['Admin', 'Manager', 'User'] },
    { label: 'Manage Users', path: '/users', icon: ShieldCheck, roles: ['Admin'] },
  ];

  const filteredNav = navItems.filter(item => item.roles.includes(user?.role));

  return (
    <>
      {/* Mobile backdrop */}
      {mobileMenuOpen && (
        <div className="sidebar-backdrop" onClick={onCloseMobileMenu} />
      )}

      <aside className={`glass-panel sidebar-container ${mobileMenuOpen ? 'mobile-open' : ''}`}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <span style={{ fontSize: '0.75rem', fontWeight: 700, color: 'var(--text-dark)', textTransform: 'uppercase', letterSpacing: '1px', paddingLeft: '12px', marginBottom: '8px' }}>
            Navigation
          </span>
          {filteredNav.map((item) => {
            const Icon = item.icon;
            return (
              <NavLink
                key={item.path}
                to={item.path}
                onClick={onCloseMobileMenu}
                className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
              >
                <Icon size={18} />
                {item.label}
              </NavLink>
            );
          })}
        </div>

        <div style={{ paddingTop: '16px', borderTop: '1px solid var(--border-color)' }}>
          <button
            onClick={() => {
              if (onCloseMobileMenu) onCloseMobileMenu();
              logout();
            }}
            className="sidebar-logout-btn"
          >
            <LogOut size={18} />
            Sign Out
          </button>
        </div>
      </aside>
    </>
  );
};
