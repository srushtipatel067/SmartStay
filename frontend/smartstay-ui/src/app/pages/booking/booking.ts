import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RoomService } from '../../services/room.service';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../services/booking.service';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css',
})
export class Booking implements OnInit {

  roomId!: number;
  room: any;
  loading = true;

  guestName: string = '';
  guestEmail: string = '';
  guestPhone: string = '';

  checkInDate: string = '';
  checkOutDate: string = '';

  today!: string;

  errorMessage: string = '';
  successMessage: string = '';

  bookNow() {

  this.errorMessage = '';
  this.successMessage = '';

  if (!this.guestName || !this.guestEmail || !this.guestPhone) {
    this.errorMessage = "Please fill all guest details";
    return;
  }

  if (!this.checkInDate || !this.checkOutDate) {
    this.errorMessage = "Please select dates";
    return;
  }

  const today = new Date().toISOString().split('T')[0];

  if (this.checkInDate < this.today) {
    this.errorMessage = "Check-in cannot be in past";
    return;
  }

  if (this.checkOutDate <= this.checkInDate) {
    this.errorMessage = "Check-out must be after check-in";
    return;
  }

  const bookingData = {
    userId: 0,
    guestName: this.guestName,
    guestEmail: this.guestEmail,
    guestPhone: this.guestPhone,

    hotelId: this.room.hotelId,
    roomId: this.roomId,

    checkInDate: this.checkInDate,
    checkOutDate: this.checkOutDate,

    adults: 1,
    children: 0,
    roomsBooked: 1,
    specialRequest: ""
  };

  this.bookingService.createBooking(bookingData).subscribe({
    next: () => {
      this.errorMessage = '';
      this.successMessage = "Booking Successful!";
        this.cdr.detectChanges(); // FIX - detect changes/ UI update
    },
    error: (err) => {
      this.errorMessage = err.error?.data?.message || err.error?.message || "Booking failed";
    }
  });
}

isPastDate(date: string): boolean {
  const selected = new Date(date);
  const today = new Date(this.today);

  selected.setHours(0,0,0,0);
  today.setHours(0,0,0,0);

  return selected < today;
}

onDateChange() {

  this.errorMessage = '';
  this.successMessage = '';

  if (!this.checkInDate) return;

  const today = new Date(this.today);
  const checkIn = new Date(this.checkInDate);
  const checkOut = new Date(this.checkOutDate);

  today.setHours(0,0,0,0);
  checkIn.setHours(0,0,0,0);
  if (this.checkOutDate) checkOut.setHours(0,0,0,0);

  // past date block
  if (checkIn < today) {
    this.errorMessage = "Check-in cannot be in past";
    this.checkInDate = '';
    this.cdr.detectChanges(); // force UI
    return;
  }

  // checkout validation
  if (this.checkOutDate && checkOut <= checkIn) {
    this.errorMessage = "Check-out must be after check-in";
    this.checkOutDate = '';
    this.cdr.detectChanges(); // force UI
    return;
  }

  this.cdr.detectChanges();
}

closeSuccess() {
  this.successMessage = '';
}

  constructor(
    private route: ActivatedRoute,
    private roomService: RoomService,
    private bookingService: BookingService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {

    this.today = new Date().toISOString().split('T')[0];

    this.roomId = Number(this.route.snapshot.paramMap.get('roomId'));

    this.roomService.getRoomById(this.roomId).subscribe({
      next: (res: any) => {
        this.room = res;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}