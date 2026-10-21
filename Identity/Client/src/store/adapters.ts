import { createEntityAdapter } from '@ngrx/entity';

export const usersAdapter = createEntityAdapter<User>({
  selectId: (u) => u.id,
});
