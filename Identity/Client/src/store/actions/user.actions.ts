import { createAction, props } from '@ngrx/store';

export const loadCurrentUser = createAction('[User] Load');
export const loadCurrentUserSuccess = createAction('[User] Load Success', props<{ user: User }>());
export const loadCurrentUserFailure = createAction(
  '[User] Load Failure',
  props<{ error: string }>(),
);
