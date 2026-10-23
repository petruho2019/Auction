import { createEntityAdapter } from '@ngrx/entity';
import { User } from '../models/user/User';

export const usersAdapter = createEntityAdapter<User>({
  selectId: (u) => u.id,
});
