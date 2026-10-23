import { Routes } from '@angular/router';
import { UserProfile } from './components/user-profile/user-profile';
import { Login } from './components/login/login';

export const routes: Routes = [
  {
    path: 'identity',
    children: [
      { path: 'profile', component: UserProfile },
      { path: 'login', component: Login },
    ],
  },
];
