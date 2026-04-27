import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  private baseUrl = 'https://localhost:7094/api/Booking';

  constructor(private http: HttpClient) {}

  createBooking(data: any): Observable<any> {
    return this.http.post(this.baseUrl, data);
  }

  getMyBookings(email: string, phone: string) {
    return this.http.get(
      `${this.baseUrl}/guest-bookings?email=${email}&phone=${phone}`
  );
  }

  cancelBooking(id: number) {
    return this.http.put(`${this.baseUrl}/${id}/cancel`, {});
  }
}