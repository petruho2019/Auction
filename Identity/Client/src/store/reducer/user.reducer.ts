import { createReducer, on } from '@ngrx/store';
import { usersAdapter } from '../adapters';
import { UserState } from '../states';
import { loadCurrentUserSuccess } from '../actions/user.actions';

export const userFeatureKey = 'User';

export const initialState: UserState = usersAdapter.getInitialState();

export const userReducer = createReducer(
  initialState,
  on(loadCurrentUserSuccess, (state, { user }) => usersAdapter.addOne(user, state)),
);
