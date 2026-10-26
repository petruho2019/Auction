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
import { createReducer, on } from '@ngrx/store';

export const userFeatureKey = 'User';

export const initialState: UserState = usersAdapter.getInitialState({
  authStatus: 'idle',
  authError: null,
});

export const userReducer = createReducer(
  initialState,
  on(loadCurrentUserSuccess, (state, { user }) =>
    usersAdapter.upsertOne(user, {
      ...state,
      authStatus: user.isAuthenticated ? 'authenticated' : 'unauthenticated',
      authError: null,
    }),
  ),
  on(login, register, (state) => ({ ...state, authStatus: 'loading', authError: null })),
  on(loginSuccess, (state, { user }) =>
    usersAdapter.upsertOne(user, { ...state, authStatus: 'authenticated', authError: null }),
  ),
  on(registerSuccess, (state) => ({ ...state, authStatus: 'authenticated', authError: null })),
  on(loginFailure, registerFailure, (state, { error }) => ({
    ...state,
    authStatus: 'error',
    authError: error,
  })),
  on(clearAuthStatus, (state) => ({ ...state, authStatus: 'idle', authError: null })),
);
