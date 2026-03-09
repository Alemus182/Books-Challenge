import {AuthState} from "../../models/auth-state.model";
import * as actions from '../actions';

export interface IAuthState {
  user: AuthState;
}

export const initAuthState: IAuthState = {
  user: null,
};

export function authReducer(
  state = initAuthState,
  action: actions.AuthActions,
): IAuthState {
  switch (action.type) {
    case actions.AuthActionTypes.SET_AUTH_STATE:
      return {
        ...state,
        user: {...action.authState}
      }

    case actions.AuthActionTypes.GET_AUTH_STATE:
      return state

    case actions.AuthActionTypes.CLEAR_AUTH_STATE:
      state = initAuthState;
      return state
  }
}
