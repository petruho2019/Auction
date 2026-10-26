import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { concatMap, filter, map, switchMap, take, tap, withLatestFrom } from 'rxjs';
import { selectAuthStatus, selectCurrentUser } from '../../../store/selectors/user.selector';
import { loadCurrentUser } from '../../../store/actions/user.actions';
import { UserState } from '../../../store/states';
import { AuthService } from '../../../services/AuthService';

export const authorizeGuard: CanActivateFn = () => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const store = inject(Store);

  // return authService.getCurrentUser().pipe(
  //   map((u) => {
  //     if (u.isAuthenticated) return true;

  //     return router.createUrlTree(['/login']);
  //   }),
  // );

  return store.select(selectAuthStatus).pipe(
    tap((u) => {
      if (!u) {
        store.dispatch(loadCurrentUser());
      }

      console.log(`selectAuthStatus: ${JSON.stringify(u, null, 2)}`);
    }),
    filter((s) => [`unauthenticated`, `authenticated`].includes(s.authStatus)),
    take(1),
    map((s) => {
      console.log(`map: ${JSON.stringify(s, null, 2)}`);
      if (s.authStatus === 'authenticated') {
        return true;
      }
      return router.createUrlTree(['/login']);
    }),
  );
};

// return store.select(selectAuthStatus).pipe(
//     tap(({ User }) => {
//       console.log(`selectAuthStatus: ${JSON.stringify(User, null, 2)}`);
//       if (!User || User?.authStatus === 'idle') {
//       }
//     }),
//     // filter(({ User }) => !!User),
//     take(1),
//     switchMap(() =>
//       store.select(selectCurrentUser).pipe(
//         filter((u) => !!u),
//         take(1),
//         map((user) => {
//           console.log(`selectCurrentUser: ${JSON.stringify(user, null, 2)}`);

//           if (user?.isAuthenticated) {
//             return true;
//           }

//           return router.createUrlTree(['/login']);
//         }),
//       ),
//     ),
//   );
