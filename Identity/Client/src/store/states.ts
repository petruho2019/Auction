import { EntityState } from '@ngrx/entity';
import { User } from '../models/user/User';

export type AuthStatus =
  'idle' | 'loading' | 'authenticated' | 'error' | 'unauthenticated' | 'registered';

export interface UserState {
  id: string;
  user: User | null;
  authStatus: AuthStatus;
  authError: string | null;
}
