import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
})
export class Register {

  name: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  phoneNumber: string = '';

  errorMessage: string = '';
  successMessage: string = '';
  loading = false;
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private http: HttpClient,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  register() {

    this.errorMessage = '';
    this.successMessage = '';
    this.loading = true;

    // validation
    if (!this.name || !this.email || !this.password) {
      this.loading = false;
      this.errorMessage = "All fields are required";
      this.cdr.detectChanges();
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailRegex.test(this.email)) {
      this.loading = false;
      this.errorMessage = "Enter a valid email address";
      this.cdr.detectChanges();
      return;
    }

    // Password rules
    if (this.password.length < 8) {
      this.loading = false;
      this.errorMessage = "Password must be at least 8 characters.";
      this.cdr.detectChanges();
      return;
    }

    if (!/[A-Z]/.test(this.password) ||
        !/[a-z]/.test(this.password) ||
        !/[0-9]/.test(this.password) ||
        !/[^A-Za-z0-9]/.test(this.password)) {

      this.loading = false;
      this.errorMessage =
        "Password must include uppercase, lowercase, number, and special character.";
      this.cdr.detectChanges();
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.loading = false;
      this.errorMessage = "Password and confirm password do not match.";
      this.cdr.detectChanges();
      return;
    }

    const phoneRegex = /^[0-9+() ]{8,15}$/;

    if (!phoneRegex.test(this.phoneNumber)) {
      this.loading = false;
      this.errorMessage = "Enter a valid phone number";
      this.cdr.detectChanges();
      return;
    }

    const data = {
      fullName: this.name,        // correct mapping
      email: this.email,
      password: this.password,
      confirmPassword: this.confirmPassword,
      phoneNumber: this.phoneNumber
    };

    this.http.post('https://localhost:7094/api/auth/register', data)
      .subscribe({
        next: (res: any) => {

        const token = res.data?.token;

        if (token) {
          localStorage.setItem('token', token);
        }

          this.loading = false;
          this.successMessage = "Account created successfully!";

          this.cdr.detectChanges();

          // redirect after short delay
          setTimeout(() => {
            window.location.href = '/';
          }, 1000);
        },
        error: (err) => {
          this.loading = false;

          this.errorMessage =
            err.error?.data?.message ||
            err.error?.message ||
            "Registration failed";

          this.cdr.detectChanges();
        }
      });
  }
}