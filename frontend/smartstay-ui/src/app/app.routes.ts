import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { adminGuard } from './guards/admin-guard';
import { noAuthGuard } from './guards/no-auth-guard';


export const routes = [

  { path: '', loadComponent: () => import('./pages/home/home').then(m => m.Home) },

  {
    path: 'login',
    canActivate: [noAuthGuard],
    loadComponent: () => import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    canActivate: [noAuthGuard],
    loadComponent: () => import('./pages/register/register').then(m => m.Register)
  },
  { path: 'hotels', loadComponent: () => import('./pages/hotels/hotels').then(m => m.Hotels) },

  { path: 'rooms/:hotelId', loadComponent: () => import('./pages/rooms/rooms').then(m => m.Rooms) },

  // PROTECTED ROUTES
  {
    path: 'booking/:roomId',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/booking/booking').then(m => m.Booking)
  },

  {
    path: 'my-bookings',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/my-bookings/my-bookings').then(m => m.MyBookings)
  },

  {
    path: 'admin',
    canActivate: [authGuard, adminGuard],
    loadComponent: () => import('./pages/admin/admin').then(m => m.Admin)
  },

  {
    path: 'profile',
    loadComponent: () => import('./pages/profile/profile').then(m => m.Profile)
  }
];