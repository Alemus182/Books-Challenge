export interface AuthState {
  valid: boolean;
  id: string;
  token: string;
  refreshToken: string;
  message: string;
}
