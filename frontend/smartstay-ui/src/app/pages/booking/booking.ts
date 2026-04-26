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

  checkInDate: string = '';
  checkOutDate: string = '';

  bookNow() {

  const bookingData = {
    userId: 0, // guest booking
    guestName: "Guest User",
    guestEmail: "guest@smartstay.com",
    guestPhone: "9000000000",

    hotelId: this.room.hotelId, // IMPORTANT
    roomId: this.roomId,

    checkInDate: this.checkInDate,
    checkOutDate: this.checkOutDate,

    adults: 1,
    children: 0,
    roomsBooked: 1,

    specialRequest: ""
  };

  console.log("SENDING:", bookingData);

  this.bookingService.createBooking(bookingData).subscribe({
    next: (res: any) => {
      console.log("BOOKING SUCCESS:", res);
      alert("Booking Successful!");
    },
    error: (err) => {
      console.error("BACKEND RESPONSE:", err.error);
      alert("Booking Failed");
    }
  });
}

  constructor(
    private route: ActivatedRoute,
    private roomService: RoomService,
    private bookingService: BookingService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
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