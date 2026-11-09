import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { filter, map, take, tap } from 'rxjs';
import { selectAuthStatus } from '../../../store/selectors/user.selector';
import { loadCurrentUser } from '../../../store/actions/user.actions';

export const authorizeGuard: CanActivateFn = () => {
  const router = inject(Router);
  const store = inject(Store);

  return store.select(selectAuthStatus).pipe(
    tap((u) => {
      if (!u) {
        store.dispatch(loadCurrentUser());
      }
    }),
    filter((s) => [`unauthenticated`, `authenticated`].includes(s.authStatus)),
    take(1),
    map((s) => {
      if (s.authStatus === `authenticated`) {
        return true;
      }
      return router.createUrlTree(['/login']);
    }),
  );
};
