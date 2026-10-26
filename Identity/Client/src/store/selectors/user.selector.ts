import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UserState } from '../states';
import { userFeatureKey } from '../reducer/user.reducer';
import { usersAdapter } from '../adapters';

export const selectAuthStatus = (state: { User: UserState }) => state.User;
export const selectAuthError = (state: { User: UserState }) => state.User.authError;

const selectUserState = createFeatureSelector<UserState>(userFeatureKey);
const { selectAll: selectAllUsers } = usersAdapter.getSelectors(selectUserState);

export const selectCurrentUser = createSelector(selectAllUsers, (users) =>
  users.find((user) => user),
);
