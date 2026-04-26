import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/home/home').then(m => m.Home)
  },
  {
    path: 'hotels',
    loadComponent: () =>
      import('./pages/hotels/hotels').then(m => m.Hotels)
  },
  {
    path: 'offers',
    loadComponent: () =>
      import('./pages/offers/offers').then(m => m.Offers)
  }
];