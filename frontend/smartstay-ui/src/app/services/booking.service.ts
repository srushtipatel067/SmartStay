import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  private baseUrl = 'https://localhost:7094/api/Booking';

  constructor(private http: HttpClient) { }

  createBooking(data: any): Observable<any> {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.post(
      this.baseUrl,
      data,
      { headers }
    );
  }

  getAllBookings() {
    return this.http.get('https://localhost:7094/api/Booking');
  }

  getMyBookings(email: string, phone: string) {
    return this.http.get(
      `${this.baseUrl}/guest-bookings?email=${email}&phone=${phone}`
    );
  }

  getUserBookings() {
    const token = localStorage.getItem('token');

    const headers = {
      Authorization: `Bearer ${token}`
    };

    return this.http.get(
      'https://localhost:7094/api/Booking/my-bookings',
      { headers }
    );
  }

  getLoggedInUserBookings() {
    return this.http.get<any>(`${this.baseUrl}/my-bookings`);
  }

  cancelBooking(id: number) {
    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.put(
      `https://localhost:7094/api/Booking/${id}/cancel`,
      {},
      { headers }
    );
  }

  updateBookingStatus(id: number, status: string) {
    return this.http.put(
      `https://localhost:7094/api/Booking/${id}/status`,
      { status }
    );
  }

  updatePaymentStatus(id: number, status: string) {
    return this.http.put(
      `https://localhost:7094/api/Booking/${id}/payment`,
      { status }
    );
  }
}