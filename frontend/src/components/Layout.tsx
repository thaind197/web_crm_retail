import React, { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/useAuthStore';
import { 
  LayoutDashboard, 
  ShoppingCart, 
  Warehouse, 
  Users, 
  LogOut, 
  Menu, 
  X, 
  Store,
  Settings,
  Sun,
  Moon
} from 'lucide-react';
import { useThemeStore } from '../store/useThemeStore';
import { loadAndChangeLanguage } from '../i18n';
import './Layout.css';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { fullName, roles, branchId, logout } = useAuthStore();
  const { theme, toggleTheme } = useThemeStore();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const toggleSidebar = () => {
    setSidebarOpen(!sidebarOpen);
  };

  const changeLanguage = (lng: string) => {
    loadAndChangeLanguage(lng);
  };

  const getRoleBadgeColor = () => {
    if (roles.includes('Admin')) return 'badge-admin';
    return 'badge-staff';
  };

  return (
    <div className="layout-container">
      {/* Sidebar Overlay for Mobile */}
      {sidebarOpen && <div className="sidebar-overlay" onClick={toggleSidebar}></div>}

      {/* Sidebar Panel */}
      <aside className={`sidebar ${sidebarOpen ? 'open' : ''}`}>
        <div className="sidebar-header">
          <div className="sidebar-logo">
            <svg 
              className="logo-icon" 
              viewBox="0 0 24 24" 
              fill="none" 
              stroke="currentColor" 
              strokeWidth="2.5" 
              strokeLinecap="round" 
              strokeLinejoin="round"
            >
              <path d="M3 3h18v18H3z" />
              <path d="M21 9H3" />
              <path d="M21 15H3" />
              <path d="M12 3v18" />
            </svg>
            <span className="logo-text">SalesCRM</span>
          </div>
          <button className="close-btn" onClick={toggleSidebar}>
            <X size={20} />
          </button>
        </div>

        <nav className="sidebar-menu">
          {roles.includes('Admin') && (
            <NavLink 
              to="/" 
              className={({ isActive }) => `menu-item ${isActive ? 'active' : ''}`}
              onClick={() => setSidebarOpen(false)}
            >
              <LayoutDashboard size={20} />
              <span>{t('menu.dashboard')}</span>
            </NavLink>
          )}

          <NavLink 
            to="/pos" 
            className={({ isActive }) => `menu-item ${isActive ? 'active' : ''}`}
            onClick={() => setSidebarOpen(false)}
          >
            <ShoppingCart size={20} />
            <span>{t('menu.pos')}</span>
          </NavLink>

          <NavLink 
            to="/inventory" 
            className={({ isActive }) => `menu-item ${isActive ? 'active' : ''}`}
            onClick={() => setSidebarOpen(false)}
          >
            <Warehouse size={20} />
            <span>{t('menu.inventory')}</span>
          </NavLink>

          {roles.includes('Admin') && (
            <NavLink 
              to="/crm" 
              className={({ isActive }) => `menu-item ${isActive ? 'active' : ''}`}
              onClick={() => setSidebarOpen(false)}
            >
              <Users size={20} />
              <span>{t('menu.crm')}</span>
            </NavLink>
          )}

          {roles.includes('Admin') && (
            <NavLink 
              to="/admin" 
              className={({ isActive }) => `menu-item ${isActive ? 'active' : ''}`}
              onClick={() => setSidebarOpen(false)}
            >
              <Settings size={20} />
              <span>{t('menu.admin')}</span>
            </NavLink>
          )}
        </nav>

        <div className="sidebar-footer">
          <button className="logout-btn" onClick={handleLogout}>
            <LogOut size={20} />
            <span>{t('common.logout')}</span>
          </button>
        </div>
      </aside>

      {/* Main Content Area */}
      <div className="main-wrapper">
        <header className="main-header glass-container">
          <div className="header-left">
            <button className="hamburger-btn" onClick={toggleSidebar}>
              <Menu size={24} />
            </button>
            <div className="branch-info">
              <Store size={18} className="branch-icon" />
              <span className="branch-name">
                {branchId ? `${t('layout.branch_label')}${branchId.substring(0, 8)}...` : t('layout.hq_label')}
              </span>
            </div>
          </div>

          <div className="header-right">
            {/* User Profile */}
            <div className="user-profile">
              <div className="user-details">
                <span className="user-name">
                  {fullName === 'System Administrator (Offline)' 
                    ? t('common.sys_admin_offline') 
                    : fullName === 'Nhân viên Cửa Hàng 1 (Offline)' 
                    ? t('common.staff_user_offline')
                    : fullName || 'User'}
                </span>
                <span className={`user-role ${getRoleBadgeColor()}`}>
                  {roles.includes('Admin') ? 'Admin' : 'Staff'}
                </span>
              </div>
              <div className="user-avatar">
                {(fullName || 'U').charAt(0).toUpperCase()}
              </div>
            </div>

            {/* Theme Toggle */}
            <button className="theme-toggle-btn" onClick={toggleTheme} title="Toggle Theme">
              {theme === 'light' ? <Moon size={18} /> : <Sun size={18} />}
            </button>

            {/* Language Selector */}
            <div className="lang-selector">
              <button 
                className={`lang-option ${i18n.language === 'vi' ? 'active' : ''}`}
                onClick={() => changeLanguage('vi')}
              >
                VI
              </button>
              <span className="lang-divider">|</span>
              <button 
                className={`lang-option ${i18n.language === 'en' ? 'active' : ''}`}
                onClick={() => changeLanguage('en')}
              >
                EN
              </button>
            </div>
          </div>
        </header>

        <main className="content-container">
          {children}
        </main>
      </div>
    </div>
  );
};
