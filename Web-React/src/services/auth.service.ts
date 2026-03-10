import api from './api';
import endpoints from './endpoints';
import { AuthState } from '../models/auth-state.model';

export interface LoginData {
  userName: string;
  password: string;
}

export async function login(data: LoginData): Promise<AuthState> {
  const response = await api.post<AuthState>(endpoints.AUTH.LOGIN, data);
  return response.data;
}

export function getSession(): AuthState | null {
  const raw = sessionStorage.getItem('userData');
  if (raw && raw !== 'undefined') {
    return JSON.parse(raw) as AuthState;
  }
  return null;
}

export function clearSession(): void {
  sessionStorage.removeItem('userData');
}
