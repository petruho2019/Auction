import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideStore, Store } from '@ngrx/store';
import { User } from '../../../models/user/User';
import { logoutSuccess, loginSuccess } from '../../../store/actions/user.actions';
import { initialState, userFeatureKey, userReducer } from '../../../store/reducer/user.reducer';
import { UserProfile } from './user-profile';

describe('UserProfile', () => {
  let component: UserProfile;
  let fixture: ComponentFixture<UserProfile>;
  let store: Store;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserProfile],
      providers: [
        provideStore({ [userFeatureKey]: userReducer }),
        provideRouter([]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(UserProfile);
    component = fixture.componentInstance;
    store = TestBed.inject(Store);
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display the current user details', () => {
    const user = new User('user-1', 'user@example.com', 'Example User');
    user.isAuthenticated = true;
    store.dispatch(loginSuccess({ user }));

    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Example User');
    expect(fixture.nativeElement.textContent).toContain('user@example.com');
  });

  it('should clear user data after logout succeeds', () => {
    const user = new User('user-1', 'user@example.com', 'Example User');
    user.isAuthenticated = true;
    const loggedInState = userReducer(initialState, loginSuccess({ user }));
    const loggedOutState = userReducer(loggedInState, logoutSuccess());

    expect(loggedOutState.ids).toEqual([]);
    expect(loggedOutState.entities).toEqual({});
    expect(loggedOutState.authStatus).toBe('unauthenticated');
  });
});
