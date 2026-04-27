import { Component, OnInit } from '@angular/core';
import { ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../services/booking.service';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-bookings.html',
  styleUrl: './my-bookings.css',
})


export class MyBookings implements OnInit {

  guestEmail: string = '';
  guestPhone: string = ''; 

  errorMessage: string = '';

  bookings: any[] = [];
  loading = false;

  constructor(
    private bookingService: BookingService,
    private cdr: ChangeDetectorRef
  ) {}

  fetchBookings() {

  this.errorMessage = '';
  this.bookings = [];

  if (!this.guestEmail || !this.guestPhone) {
    this.errorMessage = "Enter email and phone";
    return;
  }
  
  this.loading = true;

  this.bookingService.getMyBookings(this.guestEmail, this.guestPhone).subscribe({
    next: (res: any) => {
      this.bookings = res.data || [];
      this.loading = false;
      this.cdr.detectChanges(); // force UI refresh
    },
    error: (err) => {
      this.loading = false;
      this.errorMessage = err.error?.message || "Failed to fetch bookings";
      this.cdr.detectChanges(); // force UI refresh
    }
  });
}
  ngOnInit(): void {}
}

