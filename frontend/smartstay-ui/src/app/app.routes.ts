import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Hotels } from './pages/hotels/hotels';
import { Offers } from './pages/offers/offers';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'hotels', component: Hotels },
  { path: 'offers', component: Offers }
];