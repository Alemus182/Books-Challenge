import React, { createContext, useContext, useState, useCallback } from 'react';
import { AuthState } from '../models/auth-state.model';
import { getSession, clearSession } from '../services/auth.service';

interface AuthContextValue {
  user: AuthState | null;
  setUser: (user: AuthState | null) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue>({
  user: null,
  setUser: () => {},
  logout: () => {},
});

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUserState] = useState<AuthState | null>(() => getSession());

  const setUser = useCallback((u: AuthState | null) => {
    if (u) {
      sessionStorage.setItem('userData', JSON.stringify(u));
    } else {
      clearSession();
    }
    setUserState(u);
  }, []);

  const logout = useCallback(() => {
    clearSession();
    setUserState(null);
  }, []);

  return (
    <AuthContext.Provider value={{ user, setUser, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
