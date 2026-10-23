import { EntityState } from '@ngrx/entity';
import { User } from '../models/user/User';

export type AuthStatus = 'idle' | 'loading' | 'success' | 'error';

export interface UserState extends EntityState<User> {
  authStatus: AuthStatus;
  authError: string | null;
}
