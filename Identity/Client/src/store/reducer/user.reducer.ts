import { createReducer, on } from '@ngrx/store';
import { usersAdapter } from '../adapters';
import { UserState } from '../states';
import {
  clearAuthStatus,
  loadCurrentUserSuccess,
  login,
  loginFailure,
  loginSuccess,
  register,
  registerFailure,
  registerSuccess,
} from '../actions/user.actions';

export const userFeatureKey = 'User';

export const initialState: UserState = usersAdapter.getInitialState({
  authStatus: 'idle',
  authError: null,
});

export const userReducer = createReducer(
  initialState,
  on(loadCurrentUserSuccess, (state, { user }) =>
    user.id ? usersAdapter.upsertOne(user, state) : state,
  ),
  on(login, register, (state) => ({ ...state, authStatus: 'loading', authError: null })),
  on(loginSuccess, (state, { user }) =>
    user.id
      ? usersAdapter.upsertOne(user, { ...state, authStatus: 'success', authError: null })
      : { ...state, authStatus: 'success', authError: null },
  ),
  on(registerSuccess, (state) => ({ ...state, authStatus: 'success', authError: null })),
  on(loginFailure, registerFailure, (state, { error }) => ({
    ...state,
    authStatus: 'error',
    authError: error,
  })),
  on(clearAuthStatus, (state) => ({ ...state, authStatus: 'idle', authError: null })),
);

export const selectAuthStatus = (state: { User: UserState }) => state.User.authStatus;
export const selectAuthError = (state: { User: UserState }) => state.User.authError;
