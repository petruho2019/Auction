import { Routes } from '@angular/router';
import { UserProfile } from './components/user-profile/user-profile';
import { Login } from './components/login/login';
import { authorizeGuard } from './guards/authorize/authorize-guard';

export const routes: Routes = [
  { path: 'profile', component: UserProfile, canActivate: [authorizeGuard] },
  { path: 'login', component: Login },
  // { path: '', pathMatch: 'full', redirectTo: 'profile' },
];
