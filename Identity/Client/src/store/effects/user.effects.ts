import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import {
  login,
  loginFailure,
  loginSuccess,
  loadCurrentUser,
  loadCurrentUserFailure,
  loadCurrentUserSuccess,
  register,
  registerFailure,
  registerSuccess,
} from '../actions/user.actions';
import { AuthService } from '../../services/AuthService';
import { catchError, exhaustMap, map, of, switchMap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

export class UserEffects {
  private actions$ = inject(Actions);
  private authService = inject(AuthService);

  loadCurrentUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(loadCurrentUser),
      switchMap(() =>
        this.authService.getCurrentUser().pipe(
          map((user) => loadCurrentUserSuccess({ user })),
          catchError((error: unknown) =>
            of(loadCurrentUserFailure({ error: getAuthError(error) })),
          ),
        ),
      ),
    ),
  );

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(login),
      exhaustMap(({ email, password }) =>
        this.authService.login(email, password).pipe(
          switchMap(() =>
            this.authService.getCurrentUser().pipe(map((user) => loginSuccess({ user }))),
          ),
          catchError((error: unknown) =>
            of(
              loginFailure({
                error:
                  error instanceof HttpErrorResponse && error.status === 401
                    ? 'Неверный email или пароль.'
                    : getAuthError(error),
              }),
            ),
          ),
        ),
      ),
    ),
  );

  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(register),
      exhaustMap(({ email, username, password }) =>
        this.authService.register(email, username, password).pipe(
          map(() => registerSuccess()),
          catchError((error: unknown) =>
            of(registerFailure({ error: getAuthError(error) })),
          ),
        ),
      ),
    ),
  );
}

function getAuthError(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const response = error.error as { error?: unknown } | null;
    if (typeof response?.error === 'string') {
      return response.error;
    }
  }

  return 'Не удалось выполнить запрос. Попробуйте ещё раз.';
}
