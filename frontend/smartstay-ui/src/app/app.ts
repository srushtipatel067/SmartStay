import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  isLoggedIn = false;

  profileImageUrl: string | null = null;

  constructor(public auth: AuthService) { }

  getUserInitial(): string {
    const decoded = this.auth.getDecodedToken();

    const name =
      decoded?.fullName ||
      decoded?.name ||
      decoded?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

    return name ? name.charAt(0).toUpperCase() : 'U';
  }

  ngOnInit() {
    this.auth.loggedIn$.subscribe(status => {
      if (status) {
        this.auth.getProfile().subscribe({
          next: (res: any) => {
            const image = res?.data?.profileImage;
            this.profileImageUrl = image
              ? 'https://localhost:7094' + image
              : null;
          }
        });
      } else {
        this.profileImageUrl = null;
      }
      
      this.isLoggedIn = status;
    });
  }

  logout() {
    this.auth.logout();
    window.location.href = '/login';
  }
}