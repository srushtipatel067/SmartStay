import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
})
export class Login {

  email: string = '';
  password: string = '';

  showPassword = false;

  errorMessage: string = '';
  successMessage: string = '';

  loading = false;

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  login() {

  this.errorMessage = '';
  this.successMessage = '';
  this.loading = true;

  const data = {
    email: this.email,
    password: this.password
  };

  // validations
  if (!this.email || !this.password) {
    this.loading = false;
    this.errorMessage = "Email and password are required";
    return;
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  if (!emailRegex.test(this.email)) {
    this.loading = false;
    this.errorMessage = "Enter a valid email address";
    this.cdr.detectChanges();
    return;
  }

  if (this.password.length < 8) {
    this.loading = false;
    this.errorMessage = "Password must be at least 8 characters";
    return;
  }

  this.http.post('https://localhost:7094/api/auth/login', data)
    .subscribe({
      next: (res: any) => {
        localStorage.setItem('token', res.data.token);

        this.loading = false;
        this.successMessage = "Login Successful!";
        this.cdr.detectChanges(); // force UI refresh
      },
      error: (err) => {
        this.loading = false;

        this.errorMessage =
          err.error?.data?.message ||
          err.error?.message ||
          "Invalid email or password";

        this.cdr.detectChanges(); // force UI refresh
      }
    });
}
}