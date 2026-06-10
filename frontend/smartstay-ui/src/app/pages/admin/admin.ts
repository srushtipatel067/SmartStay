import { Component, OnInit, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';
import { BookingService } from '../../services/booking.service';
import { FormsModule } from '@angular/forms';
import { HotelService } from '../../services/hotel.service';
import { RoomService } from '../../services/room.service';
import { ChangeDetectorRef } from '@angular/core';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styleUrls: ['./admin.css']
})
export class Admin implements OnInit {

  showHotelForm = false;
  showBookings = false;
  hotelLoading = false;
  hotelsLoading = false;
  hotelMessage = '';
  hotelMessageType: 'success' | 'error' = 'success';

  newHotel = {
    hotelName: '',
    hotelCode: '',
    description: '',
    addressLine1: '',
    addressLine2: '',
    city: '',
    state: '',
    country: 'India',
    postalCode: '',
    latitude: 0,
    longitude: 0,
    starRating: 5,
    basePricePerNight: 0,
    thumbnailImageUrl: '',
    contactEmail: '',
    contactPhone: '',
    checkInTime: '12:00',
    checkOutTime: '10:00'
  };

  totalHotels = 0;
  totalRooms = 0;
  totalBookings = 0;
  totalRevenue = 0;

  recentBookings: any[] = [];
  allBookings: any[] = [];
  bookingView: 'none' | 'recent' | 'all' = 'none';
  bookingLoading = false;
  updatingId: number | null = null;

  hotels: any[] = [];
  showHotels = false;
  editingHotelId: number | null = null;
  isEditMode = false;

  rooms: any[] = [];
  showRooms = false;
  selectedHotelId: number = 0;

  showRoomForm = false;
  roomLoading = false;
  roomMessage = '';
  roomMessageType: 'success' | 'error' = 'success';

  newRoom = {
    hotelId: 0,
    roomType: '',
    description: '',
    pricePerNight: 0,
    maxAdults: 1,
    maxChildren: 0,
    totalRooms: 1,
    availableRooms: 1,
    thumbnailImageUrl: ''
  };

  editingRoomId: number | null = null;
  isRoomEditMode = false;

  constructor(
    private dashboardService: DashboardService,
    private bookingService: BookingService,
    private hotelService: HotelService,
    private cdr: ChangeDetectorRef,
    private zone: NgZone,
    private roomService: RoomService
  ) { }

  ngOnInit(): void {
    this.loadDashboard();
    this.loadHotelsForDropdown();
  }

  loadDashboard() {

    // Summary API
    this.dashboardService.getSummary().subscribe({
      next: (res: any) => {
        const data = res?.data;

        this.totalHotels = data?.totalHotels || 0;
        this.totalRooms = data?.totalRooms || 0;
        this.totalBookings = data?.totalBookings || 0;
        this.totalRevenue = data?.totalRevenue || 0;
      }
    });

    // Recent bookings
    this.dashboardService.getRecentBookings().subscribe({
      next: (res: any) => {
        this.recentBookings = res?.data || [];
      }
    });
  }

  loadAllBookings() {
    this.zone.run(() => {
      this.closeAllPanels();

      this.bookingLoading = true;
      this.bookingView = 'all';

      this.cdr.detectChanges();
    });

    this.bookingService.getAllBookings().subscribe({
      next: (res: any) => {
        const list = res?.data || res || [];

        this.allBookings = list.map((b: any) => ({
          bookingId: b.BookingId || b.bookingId,
          guestName: b.GuestName || b.guestName,
          guestEmail: b.GuestEmail || b.guestEmail,
          hotelName: b.HotelName || b.hotelName || b.HotelId || b.hotelId,
          roomType: b.RoomType || b.roomType || b.RoomId || b.roomId,
          checkInDate: b.CheckInDate || b.checkInDate,
          checkOutDate: b.CheckOutDate || b.checkOutDate,
          bookingStatus: b.BookingStatus || b.bookingStatus,
          paymentStatus: b.PaymentStatus || b.paymentStatus
        }));

        this.zone.run(() => {
          this.bookingLoading = false;
          this.cdr.detectChanges();
        });
      },
      error: (err) => {
        console.error('All bookings fetch failed', err);
        this.bookingLoading = false;
      }
    });
  }

  showRecentBookings() {
    const shouldHide = this.bookingView === 'recent';

    this.closeAllPanels();

    this.bookingView = shouldHide ? 'none' : 'recent';
    this.cdr.detectChanges();
  }

  getCurrentBookings(): any[] {
    return this.bookingView === 'all'
      ? this.allBookings
      : this.recentBookings;
  }

  updateStatus(id: number, status: string) {
    this.updatingId = id;

    this.bookingService.updateBookingStatus(id, status).subscribe({
      next: () => {
        const booking = this.getCurrentBookings().find((b: any) => b.bookingId === id);

        if (booking) {
          booking.bookingStatus = status;
        }

        this.updatingId = null;
      },
      error: (err) => {
        console.error('Status update failed', err);
        this.updatingId = null;
      }
    });
  }

  updatePayment(id: number, status: string) {
    this.updatingId = id;

    this.bookingService.updatePaymentStatus(id, status).subscribe({
      next: () => {
        const booking = this.getCurrentBookings().find((b: any) => b.bookingId === id);

        if (booking) {
          booking.paymentStatus = status;
        }

        this.updatingId = null;
      },
      error: (err) => {
        console.error('Payment update failed', err);
        this.updatingId = null;
      }
    });
  }

  // Hotel
  loadHotels() {
    this.zone.run(() => {
      this.showHotels = true;
      this.showRooms = false;
      this.showRoomForm = false;
      this.bookingView = 'none';
      this.showHotelForm = false;
    });

    this.hotelService.getAllHotels().subscribe({
      next: (res: any) => {
        this.zone.run(() => {
          this.hotels = res?.data || res || [];
          this.cdr.detectChanges();
        });
      },
      error: (err) => {
        console.error('Hotels fetch failed', err);
      }
    });
  }

  loadHotelsForDropdown() {
    this.hotelsLoading = true;

    this.hotelService.getAllHotels().subscribe({
      next: (res: any) => {
        this.hotels = res?.data || res || [];
        this.hotelsLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Hotels fetch failed', err);
        this.hotels = [];
        this.hotelsLoading = false;
      }
    });
  }

  editHotel(h: any) {
    this.isEditMode = true;
    this.editingHotelId = h.hotelId;
    this.showHotelForm = true;
    this.showHotels = false;
    this.bookingView = 'none';

    this.newHotel = {
      hotelName: h.hotelName || '',
      hotelCode: h.hotelCode || '',
      description: h.description || '',
      addressLine1: h.addressLine1 || '',
      addressLine2: h.addressLine2 || '',
      city: h.city || '',
      state: h.state || '',
      country: h.country || 'India',
      postalCode: h.postalCode || '',
      latitude: h.latitude || 0,
      longitude: h.longitude || 0,
      starRating: h.starRating || 5,
      basePricePerNight: h.basePricePerNight || 0,
      thumbnailImageUrl: h.thumbnailImageUrl || '',
      contactEmail: h.contactEmail || '',
      contactPhone: h.contactPhone || '',
      checkInTime: h.checkInTime || '12:00',
      checkOutTime: h.checkOutTime || '10:00'
    };
  }

  saveHotel() {

    this.hotelMessage = '';

    const payload = {
      ...this.newHotel,
      checkInTime: this.newHotel.checkInTime + ':00',
      checkOutTime: this.newHotel.checkOutTime + ':00'
    };

    if (!this.newHotel.hotelName || !this.newHotel.city) {
      this.hotelMessageType = 'error';
      this.hotelMessage = 'Hotel name and city are required';
      return;
    }

    this.hotelLoading = true;

    const request$ = this.isEditMode && this.editingHotelId
      ? this.hotelService.updateHotel(this.editingHotelId, {
        ...payload,
        hotelId: this.editingHotelId,
        isActive: true
      })
      : this.hotelService.createHotel(payload);

    request$.subscribe({
      next: (res: any) => {
        this.zone.run(() => {
          this.hotelLoading = false;
          this.hotelMessageType = 'success';
          this.hotelMessage =
            res?.message || (this.isEditMode ? 'Hotel updated successfully' : 'Hotel added successfully');
        });

        this.newHotel = {
          hotelName: '',
          hotelCode: '',
          description: '',
          addressLine1: '',
          addressLine2: '',
          city: '',
          state: '',
          country: 'India',
          postalCode: '',
          latitude: 0,
          longitude: 0,
          starRating: 5,
          basePricePerNight: 0,
          thumbnailImageUrl: '',
          contactEmail: '',
          contactPhone: '',
          checkInTime: '12:00',
          checkOutTime: '10:00'
        };

        this.loadDashboard();
        this.showHotelForm = false;
        this.isEditMode = false;
        this.editingHotelId = null;
        this.cdr.detectChanges();

        setTimeout(() => {
          this.hotelMessage = '';
        }, 4000);
      },
      error: (err) => {
        console.error('HOTEL CREATE ERROR:', err);

        this.hotelLoading = false;
        this.hotelMessageType = 'error';
        this.hotelMessage =
          err.error?.message ||
          err.error?.errors?.[0] ||
          'Failed to add hotel';
      }
    });
  }

  openAddHotelForm() {

    this.isEditMode = false;
    this.editingHotelId = null;

    this.newHotel = {
      hotelName: '',
      hotelCode: '',
      description: '',
      addressLine1: '',
      addressLine2: '',
      city: '',
      state: '',
      country: 'India',
      postalCode: '',
      latitude: 0,
      longitude: 0,
      starRating: 5,
      basePricePerNight: 0,
      thumbnailImageUrl: '',
      contactEmail: '',
      contactPhone: '',
      checkInTime: '12:00',
      checkOutTime: '10:00'
    };

    this.showHotelForm = !this.showHotelForm;
  }

  cancelHotelForm() {
    this.showHotelForm = false;
    this.isEditMode = false;
    this.editingHotelId = null;
    this.hotelMessage = '';

    this.newHotel = {
      hotelName: '',
      hotelCode: '',
      description: '',
      addressLine1: '',
      addressLine2: '',
      city: '',
      state: '',
      country: 'India',
      postalCode: '',
      latitude: 0,
      longitude: 0,
      starRating: 5,
      basePricePerNight: 0,
      thumbnailImageUrl: '',
      contactEmail: '',
      contactPhone: '',
      checkInTime: '12:00',
      checkOutTime: '10:00'
    };
  }

  removeHotel(id: number) {
    if (!confirm('Are you sure you want to remove this hotel?')) {
      return;
    }

    this.hotelService.deleteHotel(id).subscribe({
      next: (res: any) => {
        this.hotelMessageType = 'success';
        this.hotelMessage = res?.message || 'Hotel removed successfully';

        this.loadHotels();
        this.loadDashboard();

        setTimeout(() => {
          this.hotelMessage = '';
        }, 3000);
      },
      error: (err) => {
        this.hotelMessageType = 'error';
        this.hotelMessage =
          err.error?.message || 'Failed to remove hotel';
      }
    });
  }

  openManageRooms() {
    this.closeAllPanels();

    this.showRooms = true;
    this.selectedHotelId = 0;
    this.rooms = [];
    this.roomMessage = '';
    this.roomLoading = false;

    if (this.hotels.length === 0) {
      this.loadHotelsForDropdown();
    }

    this.cdr.detectChanges();
  }

  loadRoomsByHotel() {
    if (!this.selectedHotelId) return;

    this.rooms = []; // reset first

    this.roomService.getRoomsByHotel(this.selectedHotelId).subscribe({
      next: (res: any) => {
        this.zone.run(() => {
          this.rooms = res?.data || res || [];
          this.cdr.detectChanges(); // IMPORTANT
        });
      },
      error: (err) => {
        console.error('Rooms fetch failed', err);
      }
    });
  }

  saveRoom() {
    this.roomMessage = '';

    if (!this.selectedHotelId) {
      this.roomMessageType = 'error';
      this.roomMessage = 'Please select hotel first';
      return;
    }

    if (!this.newRoom.roomType || this.newRoom.pricePerNight <= 0) {
      this.roomMessageType = 'error';
      this.roomMessage = 'Room type and price are required';
      return;
    }

    this.roomLoading = true;

    const payload = {
      ...this.newRoom,
      hotelId: this.selectedHotelId
    };

    const request$ = this.isRoomEditMode && this.editingRoomId
      ? this.roomService.updateRoom(this.editingRoomId, {
        ...payload,
        roomId: this.editingRoomId,
        isActive: true
      })
      : this.roomService.createRoom(payload);

    request$
      .pipe(
        finalize(() => {
          this.zone.run(() => {
            this.roomLoading = false;
            this.cdr.detectChanges();
          });
        })
      )
      .subscribe({
        next: (res: any) => {
          this.zone.run(() => {
            this.roomMessageType = 'success';
            this.roomMessage =
              res?.message || (this.isRoomEditMode ? 'Room updated successfully' : 'Room added successfully');

            this.showRoomForm = false;
            this.isRoomEditMode = false;
            this.editingRoomId = null;

            this.newRoom = {
              hotelId: 0,
              roomType: '',
              description: '',
              pricePerNight: 0,
              maxAdults: 1,
              maxChildren: 0,
              totalRooms: 1,
              availableRooms: 1,
              thumbnailImageUrl: ''
            };

            this.loadRoomsByHotel();
            this.loadDashboard();
          });
        },
        error: (err) => {
          this.roomMessageType = 'error';
          this.roomMessage = err.error?.message || 'Room save failed';
        }
      });
  }

  editRoom(r: any) {
    this.isRoomEditMode = true;
    this.editingRoomId = r.roomId;
    this.showRoomForm = true;

    this.newRoom = {
      hotelId: this.selectedHotelId,
      roomType: r.roomType,
      description: r.description,
      pricePerNight: r.pricePerNight,
      maxAdults: r.maxAdults,
      maxChildren: r.maxChildren,
      totalRooms: r.totalRooms,
      availableRooms: r.availableRooms,
      thumbnailImageUrl: r.thumbnailImageUrl
    };
  }

  removeRoom(id: number) {
    if (!confirm('Are you sure you want to remove this room?')) {
      return;
    }

    this.roomService.deleteRoom(id).subscribe({
      next: (res: any) => {
        this.roomMessageType = 'success';
        this.roomMessage = res?.message || 'Room removed successfully';

        this.loadRoomsByHotel();
        this.loadDashboard();

        setTimeout(() => {
          this.roomMessage = '';
        }, 3000);
      },
      error: (err) => {
        this.roomMessageType = 'error';
        this.roomMessage =
          err.error?.message || 'Failed to remove room';
      }
    });
  }

  closeAllPanels() {
    this.showHotelForm = false;
    this.showHotels = false;
    this.showRooms = false;
    this.showRoomForm = false;
    this.bookingView = 'none';
  }
}