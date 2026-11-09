import { usersAdapter } from '../adapters';
import { UserState } from '../states';
import {
  clearAuthStatus,
  loadCurrentUserSuccess,
  login,
  loginFailure,
  loginSuccess,
  logout,
  logoutFailure,
  logoutSuccess,
  register,
  registerFailure,
  registerSuccess,
} from '../actions/user.actions';
import { createReducer, on } from '@ngrx/store';

export const userFeatureKey = 'User';

export const initialState: UserState = {
  authStatus: 'idle',
  authError: null,
  user: null,
  id: '',
};

export const userReducer = createReducer(
  initialState,
  on(loadCurrentUserSuccess, (state, { user }) => ({
    ...state,
    authStatus: user.isAuthenticated ? 'authenticated' : 'unauthenticated',
    authError: null,
    user: user,
  })),
  on(login, register, (state) => ({ ...state, authStatus: 'loading', authError: null })),
  on(logout, (state) => ({ ...state, authStatus: 'loading', authError: null })),
  on(loginSuccess, (state, { user }) => ({
    ...state,
    authStatus: 'authenticated',
    authError: null,
    user: user,
  })),
  on(registerSuccess, (state) => ({ ...state, authStatus: 'registered', authError: null })),
  on(loginFailure, registerFailure, (state, { error }) => ({
    ...state,
    authStatus: 'error',
    authError: error,
  })),
  on(logoutSuccess, (state) => ({
    ...state,
    authStatus: 'unauthenticated',
    authError: null,
    user: null,
  })),
  on(logoutFailure, (state, { error }) => ({
    ...state,
    authStatus: 'authenticated',
    authError: error,
  })),
  on(clearAuthStatus, (state) => ({ ...state, authStatus: 'idle', authError: null })),
);
