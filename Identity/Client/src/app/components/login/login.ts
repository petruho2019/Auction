import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { AsyncPipe } from '@angular/common';
import { clearAuthStatus, login, register } from '../../../store/actions/user.actions';
import { selectAuthError, selectAuthStatus } from '../../../store/selectors/user.selector';
import { Router } from '@angular/router';
import { map, Subscription } from 'rxjs';

@Component({
  imports: [AsyncPipe, ReactiveFormsModule],
  selector: 'app-login',
  styleUrl: './login.scss',
  templateUrl: './login.html',
})
export class Login implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly store = inject(Store);
  private readonly router = inject(Router);

  protected readonly isRegistering = signal(false);
  protected readonly authStatus$ = this.store.select(selectAuthStatus);
  protected readonly authError$ = this.store.select(selectAuthError);
  protected authStatusSub: Subscription = null!;

  protected readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    username: [''],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  constructor() {
    this.updateUsernameValidators();
  }
  ngOnInit(): void {
    this.authStatusSub = this.authStatus$
      .pipe(map((state) => state.authStatus))
      .subscribe((authStatus) => {
        if (authStatus === 'authenticated') {
          this.router.navigate(['/profile']);
        }
      });
  }

  ngOnDestroy(): void {
    if (this.authStatusSub) {
      this.authStatusSub.unsubscribe();
    }
  }

  protected toggleMode(): void {
    this.isRegistering.update((registering) => !registering);
    this.updateUsernameValidators();
    this.form.controls.username.reset('');
    this.store.dispatch(clearAuthStatus());
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, username, password } = this.form.getRawValue();
    if (this.isRegistering()) {
      this.store.dispatch(register({ email, username, password }));
    } else {
      this.store.dispatch(login({ email, password }));
    }
  }

  private updateUsernameValidators(): void {
    const validators = this.isRegistering()
      ? [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50),
          Validators.pattern(/^[\p{L}\p{N}._-]+$/u),
        ]
      : [];
    this.form.controls.username.setValidators(validators);
    this.form.controls.username.updateValueAndValidity();
  }
}
