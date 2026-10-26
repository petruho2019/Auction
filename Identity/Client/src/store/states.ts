import { EntityState } from '@ngrx/entity';
import { User } from '../models/user/User';

export type AuthStatus = 'idle' | 'loading' | 'authenticated' | 'error' | 'unauthenticated';

export interface UserState extends EntityState<User> {
  authStatus: AuthStatus;
  authError: string | null;
}
