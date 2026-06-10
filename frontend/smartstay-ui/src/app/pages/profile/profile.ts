import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { ChangeDetectorRef, NgZone } from '@angular/core';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class Profile implements OnInit {

  user: any = {};
  loading = false;
  isEditMode = false;
  message = '';
  messageType: 'success' | 'error' = 'success';

  selectedFile: File | null = null;

  previewImage: string | null = null;
  showImagePreview = false;

  activeTab: 'profile' | 'bookings' = 'profile';
  myBookings: any[] = [];
  bookingLoading = false;

  selectedBooking: any = null;
  showModal = false;

  selectedStatus: string = 'All';
  filteredBookings: any[] = [];

  toastMessage = '';
  toastType: 'success' | 'error' = 'success';

  currentPage = 1;
  pageSize = 5;

  constructor(
    private authService: AuthService,
    private bookingService: BookingService,
    private cdr: ChangeDetectorRef,
    private zone: NgZone
  ) { }

  ngOnInit() {
    this.loadProfile();
  }

  showToast(message: string, type: 'success' | 'error') {
    this.toastMessage = message;
    this.toastType = type;

    setTimeout(() => {
      this.toastMessage = '';
    }, 3000);
  }

  loadProfile() {
    this.authService.getProfile().subscribe({
      next: (res: any) => {
        this.zone.run(() => {
          this.user = res.data;

          this.user.dateOfBirth = this.user.dateOfBirth
            ? this.user.dateOfBirth.split('T')[0]
            : '';

          this.cdr.detectChanges();
        });
      }
    });
  }

  onFileChange(event: any) {
    const file = event.target.files[0];

    if (file) {
      this.selectedFile = file;

      const reader = new FileReader();
      reader.onload = () => {
        this.previewImage = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  saveProfile() {
    const formData = new FormData();

    if (this.user.fullName) formData.append('FullName', this.user.fullName);
    if (this.user.phoneNumber) formData.append('PhoneNumber', this.user.phoneNumber);
    if (this.user.dateOfBirth) formData.append('DateOfBirth', this.user.dateOfBirth);
    if (this.user.address) formData.append('Address', this.user.address);
    if (this.selectedFile) formData.append('ProfileImage', this.selectedFile);

    this.loading = true;

    this.authService.updateProfile(formData).subscribe({
      next: () => {
        this.messageType = 'success';
        this.message = 'Profile updated successfully';
        this.loading = false;
        this.loadProfile();
      },
      error: () => {
        this.messageType = 'error';
        this.message = 'Profile update failed';
        this.loading = false;
      }
    });
  }


  openImagePreview() {
    this.showImagePreview = true;
  }

  closeImagePreview() {
    this.showImagePreview = false;
  }

  applyFilter() {
    if (this.selectedStatus === 'All') {
      this.filteredBookings = this.myBookings;
    } else {
      this.filteredBookings = this.myBookings.filter(
        b => b.bookingStatus === this.selectedStatus
      );
    }
    this.currentPage = 1;
  }

  loadMyBookings() {
    this.activeTab = 'bookings';
    this.bookingLoading = true;

    this.bookingService.getLoggedInUserBookings()
      .pipe(
        finalize(() => {
          this.zone.run(() => {
            this.bookingLoading = false;
            this.cdr.detectChanges();
          });
        })
      )
      .subscribe({
        next: (res: any) => {
          this.zone.run(() => {
            const list = res?.data || res || [];

            this.myBookings = list.map((b: any) => ({
              bookingId: b.bookingId || b.BookingId,
              hotelName: b.hotelName || b.HotelName,
              roomType: b.roomType || b.RoomType,

              addressLine1: b.addressLine1 || b.AddressLine1,
              addressLine2: b.addressLine2 || b.AddressLine2,
              city: b.city || b.City,
              state: b.state || b.State,
              country: b.country || b.Country,
              postalCode: b.postalCode || b.PostalCode,

              checkInDate: b.checkInDate || b.CheckInDate,
              checkOutDate: b.checkOutDate || b.CheckOutDate,
              adults: b.adults || b.Adults,
              children: b.children || b.Children,
              roomsBooked: b.roomsBooked || b.RoomsBooked,
              totalNights: b.totalNights || b.TotalNights,
              totalAmount: b.totalAmount || b.TotalAmount,
              specialRequest: b.specialRequest || b.SpecialRequest,
              bookingStatus: b.bookingStatus || b.BookingStatus,
              paymentStatus: b.paymentStatus || b.PaymentStatus
            }));
            // APPLY FILTER AFTER LOAD
            this.applyFilter();
          });
        },
        error: (err) => {
          console.error('Booking load failed', err);
          this.myBookings = [];
        }
      });
  }

  cancelBooking(id: number) {
    if (!confirm('Are you sure you want to cancel this booking?')) return;

    this.bookingService.cancelBooking(id).subscribe({
      next: () => {
        this.showToast('Booking cancelled successfully', 'success');
        this.loadMyBookings(); // refresh table
      },
      error: () => {
        this.showToast('Cancellation failed', 'error');
      }
    });
  }

  openBookingDetails(booking: any) {
    this.selectedBooking = booking;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  get pagedBookings() {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.filteredBookings.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredBookings.length / this.pageSize);
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
  }
}