import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'chores', pathMatch: 'full' },
  {
    path: 'chores',
    loadComponent: () => import('./features/chores/chores.page').then(m => m.ChoresPage)
  }
];
