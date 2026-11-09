import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UserState } from '../states';
import { userFeatureKey } from '../reducer/user.reducer';

export const selectAuthStatus = (state: { User: UserState }) => state.User;
export const selectAuthError = (state: { User: UserState }) => state.User.authError;

const selectUserState = createFeatureSelector<UserState>(userFeatureKey);

export const selectCurrentUser = createSelector(selectUserState, (userState) => userState.user);
