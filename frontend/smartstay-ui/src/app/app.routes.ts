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
  }, 
  {
    path: 'rooms/:hotelId',
    loadComponent: () =>
      import('./pages/rooms/rooms').then(m => m.Rooms)
  }, 
  {
    path: 'booking/:roomId',
    loadComponent: () =>
      import('./pages/booking/booking').then(m => m.Booking)
  }, 
  {
    path: 'my-bookings',
    loadComponent: () =>
      import('./pages/my-bookings/my-bookings').then(m => m.MyBookings)
  }
];