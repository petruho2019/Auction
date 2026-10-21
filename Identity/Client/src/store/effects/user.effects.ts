import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import {
  loadCurrentUser,
  loadCurrentUserFailure,
  loadCurrentUserSuccess,
} from '../actions/user.actions';
import { AuthService } from '../../services/AuthService';
import { catchError, from, map, of, switchMap } from 'rxjs';

export class UserEffects {
  private actions$ = inject(Actions);
  private authService = inject(AuthService);

  loadCurrentUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadCurrentUser),
      switchMap(() =>
        from(this.authService.getCurrentUser()).pipe(
          map((user) => {
            console.log(user.isAuthenticated);
            return loadCurrentUserSuccess({ user: user });
          }),
          catchError((error) => {
            return of(loadCurrentUserFailure({ error: error }));
          }),
        ),
      ),
    ),
  );
}
