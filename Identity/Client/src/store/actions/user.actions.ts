import { createAction, props } from '@ngrx/store';
import { User } from '../../models/user/User';

export const loadCurrentUser = createAction('[User] Load');
export const loadCurrentUserSuccess = createAction('[User] Load Success', props<{ user: User }>());
export const loadCurrentUserFailure = createAction(
  '[User] Load Failure',
  props<{ error: string }>(),
);

export const login = createAction(
  '[User] Login',
  props<{ email: string; password: string }>(),
);
export const loginSuccess = createAction('[User] Login Success', props<{ user: User }>());
export const loginFailure = createAction('[User] Login Failure', props<{ error: string }>());

export const register = createAction(
  '[User] Register',
  props<{ email: string; username: string; password: string }>(),
);
export const registerSuccess = createAction('[User] Register Success');
export const registerFailure = createAction('[User] Register Failure', props<{ error: string }>());
export const clearAuthStatus = createAction('[User] Clear Authentication Status');
