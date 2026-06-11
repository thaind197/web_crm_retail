import { create } from 'zustand';

interface AuthState {
  token: string | null;
  username: string | null;
  fullName: string | null;
  branchId: string | null;
  roles: string[];
  login: (token: string, username: string, fullName: string, roles: string[], branchId: string | null) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
  isAdmin: () => boolean;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  token: localStorage.getItem('token'),
  username: localStorage.getItem('username'),
  fullName: localStorage.getItem('fullName'),
  branchId: localStorage.getItem('branchId') || null,
  roles: JSON.parse(localStorage.getItem('roles') || '[]'),

  login: (token, username, fullName, roles, branchId) => {
    localStorage.setItem('token', token);
    localStorage.setItem('username', username);
    localStorage.setItem('fullName', fullName);
    localStorage.setItem('roles', JSON.stringify(roles));
    localStorage.setItem('branchId', branchId || '');

    set({ token, username, fullName, roles, branchId });
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('fullName');
    localStorage.removeItem('roles');
    localStorage.removeItem('branchId');

    set({ token: null, username: null, fullName: null, roles: [], branchId: null });
  },

  isAuthenticated: () => {
    return !!get().token;
  },

  isAdmin: () => {
    return get().roles.includes('Admin');
  }
}));
