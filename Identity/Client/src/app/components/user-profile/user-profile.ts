import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { filter } from 'rxjs';
import { logout } from '../../../store/actions/user.actions';
import { selectAuthStatus, selectCurrentUser } from '../../../store/selectors/user.selector';

@Component({
  imports: [AsyncPipe],
  selector: 'app-user-profile',
  styleUrl: './user-profile.scss',
  templateUrl: './user-profile.html',
})
export class UserProfile {
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly user$ = this.store.select(selectCurrentUser);
  protected readonly authStatus$ = this.store.select(selectAuthStatus);

  constructor() {
    this.authStatus$
      .pipe(
        filter((state) => state.authStatus === 'unauthenticated'),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => {
        this.router.navigate(['/login']);
      });
  }

  protected logout(): void {
    this.store.dispatch(logout());
  }
}
