import {Action} from "@ngrx/store";
import {AuthState} from "../../models/auth-state.model";

export enum AuthActionTypes {
  SET_AUTH_STATE = '[Auth] SET_AUTH_STATE',
  GET_AUTH_STATE = '[Auth] GET_AUTH_STATE',
  CLEAR_AUTH_STATE = '[Auth] CLEAR_AUTH_STATE',
}

export class SetAuth implements Action {
  readonly type = AuthActionTypes.SET_AUTH_STATE;
  constructor(public authState: AuthState) {}
}

export class GetAuth implements Action {
  readonly type = AuthActionTypes.GET_AUTH_STATE;
}

export class ClearAuth implements Action {
  readonly type = AuthActionTypes.CLEAR_AUTH_STATE;
}

export type AuthActions =
  | SetAuth
  | ClearAuth
  | GetAuth;

