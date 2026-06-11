import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Lock, User, AlertCircle, Globe, CheckCircle2, Sun, Moon } from 'lucide-react';
import axios from 'axios';
import { useAuthStore } from '../../store/useAuthStore';
import { useThemeStore } from '../../store/useThemeStore';
import { API_BASE_URL } from '../../config';
import { loadAndChangeLanguage } from '../../i18n';
import './Login.css';

interface LoginResponse {
  isSuccess: boolean;
  data?: {
    token: string;
    username: string;
    fullName: string;
    branchId: string | null;
    roles: string[];
  };
  message?: string;
  errors?: string[];
}

export const Login: React.FC = () => {
  const { t, i18n } = useTranslation();
  const loginStore = useAuthStore(state => state.login);
  const { theme, toggleTheme } = useThemeStore();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const changeLanguage = (lng: string) => {
    loadAndChangeLanguage(lng);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!username || !password) {
      setError(t('login.error_empty'));
      return;
    }

    setError(null);
    setSuccess(null);
    setLoading(true);

    try {
      // Gọi API thực tế từ Backend
      const response = await axios.post<LoginResponse>(`${API_BASE_URL}/api/auth/login`, {
        username,
        password
      });

      if (response.data.isSuccess && response.data.data) {
        const { token, fullName, roles, branchId } = response.data.data;
        loginStore(token, username, fullName, roles, branchId);
        setSuccess(`Đăng nhập thành công! Xin chào ${fullName}`);
      } else {
        setError(response.data.message || 'Đăng nhập thất bại.');
      }
    } catch (err: any) {
      console.error(err);
      
      // Giả lập đăng nhập ngoại tuyến phục vụ kiểm thử nhanh (Offline Mode / Mock)
      if (username === 'admin' && password === 'Password123!') {
        loginStore('mock_jwt_token_admin', 'admin', 'System Administrator (Offline)', ['Admin'], null);
        setSuccess('Đăng nhập ngoại tuyến thành công (Tài khoản Admin mẫu)!');
        return;
      } else if (username === 'staff' && password === 'Password123!') {
        loginStore('mock_jwt_token_staff', 'staff', 'Nhân viên Cửa Hàng 1 (Offline)', ['Staff'], 'default-branch-id');
        setSuccess('Đăng nhập ngoại tuyến thành công (Tài khoản Staff mẫu)!');
        return;
      }

      setError(
        err.response?.data?.message || 
        'Không thể kết nối đến máy chủ Backend. Sử dụng tài khoản admin/Password123! hoặc staff/Password123! để test thử offline.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      {/* Floating Language Switcher */}
      <div className="floating-actions">
        <button 
          type="button" 
          className="lang-btn" 
          onClick={toggleTheme}
          title="Toggle Theme"
          style={{ padding: '8px 10px', borderRadius: '30px' }}
        >
          {theme === 'light' ? <Moon size={16} /> : <Sun size={16} />}
        </button>
        <button 
          type="button"
          className={`lang-btn ${i18n.language === 'vi' ? 'active' : ''}`} 
          onClick={() => changeLanguage('vi')}
        >
          <Globe size={16} />
          VI
        </button>
        <button 
          type="button"
          className={`lang-btn ${i18n.language === 'en' ? 'active' : ''}`} 
          onClick={() => changeLanguage('en')}
        >
          <Globe size={16} />
          EN
        </button>
      </div>

      <div className="login-card glass-container">
        <div className="login-header">
          <div className="logo-container">
            {/* Embedded Inline SVG Logo for standalone visual beauty */}
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
          </div>
          <h1 className="login-title glow-text">{t('login.title')}</h1>
          <p className="login-subtitle">{t('login.subtitle')}</p>
        </div>

        <form onSubmit={handleSubmit}>
          {error && (
            <div className="error-banner">
              <AlertCircle className="error-icon" />
              <span>{error}</span>
            </div>
          )}

          {success && (
            <div className="success-banner">
              <CheckCircle2 className="success-icon" />
              <span>{success}</span>
            </div>
          )}

          <div className="form-group">
            <label className="form-label" htmlFor="username">{t('login.username')}</label>
            <div className="input-container">
              <User className="input-icon" />
              <input
                id="username"
                className="form-input"
                type="text"
                placeholder={t('login.username_placeholder') || "Enter username..."}
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                disabled={loading}
              />
            </div>
          </div>

          <div className="form-group">
            <label className="form-label" htmlFor="password">{t('login.password')}</label>
            <div className="input-container">
              <Lock className="input-icon" />
              <input
                id="password"
                className="form-input"
                type="password"
                placeholder={t('login.password_placeholder') || "Enter password..."}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={loading}
              />
            </div>
          </div>

          <button className="submit-btn" type="submit" disabled={loading}>
            {loading ? (
              <>
                <span className="btn-spinner"></span>
                {t('login.loading')}
              </>
            ) : (
              t('login.submit')
            )}
          </button>
        </form>

        <div className="login-footer">
          <p>{t('login.footer')}</p>
        </div>
      </div>
    </div>
  );
};
