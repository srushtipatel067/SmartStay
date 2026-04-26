import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HotelService {

  private baseUrl = 'https://localhost:7094/api/Hotel'; // backend URL

  constructor(private http: HttpClient) {}

  getAllHotels(): Observable<any> {
    return this.http.get(this.baseUrl);
  }
}