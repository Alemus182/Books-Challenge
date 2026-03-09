export class AuthState {
  valid: boolean;
  id: string;
  token: string;
  refreshToken: string;
  message: string;

  constructor(
    valid: boolean,
    id: string,
    token: string,
    refreshToken?: string,
  ){
    this.valid = valid;
    this.id = id;
    this.token = token;
    this.refreshToken = refreshToken;
  }
}
