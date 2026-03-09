import {
  ActionReducerMap,
} from "@ngrx/store";

import * as mainStore from './reducers';

export interface IAppState {
  authState: mainStore.IAuthState,
}

export const appReducers: ActionReducerMap<IAppState> = {
  authState: mainStore.authReducer,
};
