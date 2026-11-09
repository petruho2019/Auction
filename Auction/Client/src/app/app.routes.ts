import { Routes } from '@angular/router';
import { CreateAuction } from './pages/create-auction/create-auction';

export const routes: Routes = [
  { path: 'auction', children: [{ path: 'create', component: CreateAuction }] },
  { path: '', pathMatch: 'full', redirectTo: 'auction/create' },
  { path: '**', redirectTo: 'auction/create' },
];
