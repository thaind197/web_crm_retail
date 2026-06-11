import React, { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuthStore } from './store/useAuthStore';
import { Login } from './features/auth/Login';
import { Layout } from './components/Layout';
import { Dashboard } from './features/dashboard/Dashboard';
import { POS } from './features/pos/POS';
import { Inventory } from './features/inventory/Inventory';
import { CRM } from './features/crm/CRM';
import { AdminPanel } from './features/admin/AdminPanel';
import i18n, { loadAndChangeLanguage } from './i18n';

const ProtectedRoute: React.FC<{ children: React.ReactNode; allowedRoles?: string[] }> = ({ children, allowedRoles }) => {
  const { token, roles } = useAuthStore();
  
  if (!token) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && !allowedRoles.some(r => roles.includes(r))) {
    const isStaffOnly = roles.includes('Staff') && !roles.includes('Admin');
    return <Navigate to={isStaffOnly ? "/pos" : "/"} replace />;
  }
  
  return <Layout>{children}</Layout>;
};

const App: React.FC = () => {
  const token = useAuthStore(state => state.token);
  const [i18nReady, setI18nReady] = useState(false);

  useEffect(() => {
    const initLanguage = async () => {
      const currentLang = i18n.language || localStorage.getItem('i18nextLng') || 'vi';
      await loadAndChangeLanguage(currentLang);
      setI18nReady(true);
    };
    initLanguage();
  }, []);

  if (!i18nReady) {
    return (
      <div style={{ display: 'flex', height: '100vh', width: '100vw', alignItems: 'center', justifyContent: 'center', background: '#09090e', color: '#fff' }}>
        <div className="spinner"></div>
      </div>
    );
  }

  return (
    <BrowserRouter>
      <Routes>
        <Route 
          path="/login" 
          element={token ? (
            // Automatically redirect staff to POS, admin to Dashboard
            <Navigate to="/" replace />
          ) : <Login />} 
        />
        
        <Route 
          path="/" 
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <Dashboard />
            </ProtectedRoute>
          } 
        />
        
        <Route 
          path="/pos" 
          element={
            <ProtectedRoute>
              <POS />
            </ProtectedRoute>
          } 
        />
        
        <Route 
          path="/inventory" 
          element={
            <ProtectedRoute>
              <Inventory />
            </ProtectedRoute>
          } 
        />
        
        <Route 
          path="/crm" 
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <CRM />
            </ProtectedRoute>
          } 
        />

        <Route 
          path="/admin" 
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <AdminPanel />
            </ProtectedRoute>
          } 
        />

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
};

export default App;
