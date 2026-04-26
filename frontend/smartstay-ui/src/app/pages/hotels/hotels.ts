import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router'; 
import { HotelService } from '../../services/hotel.service';

@Component({
  selector: 'app-hotels',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './hotels.html',
  styleUrl: './hotels.css',
})
export class Hotels implements OnInit {

  hotels: any[] = [];
  loading = true;

  constructor(
  private hotelService: HotelService,
  private cdr: ChangeDetectorRef
) {}

 ngOnInit(): void {
  this.loading = true;

  this.hotelService.getAllHotels().subscribe({
    next: (res: any) => {
      this.hotels = res.data || [];
      this.loading = false;

      this.cdr.detectChanges(); // IMPORTANT
    },
    error: (err) => {
      console.error(err);
      this.loading = false;

      this.cdr.detectChanges(); // IMPORTANT
    }
  });
}
}