import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  loading = signal<boolean>(false);
  busyRequestCount = 0;

  constructor() { }

  busy() {
    this.busyRequestCount++;
    this.loading.set(true);
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.loading.set(false);
    }
  }
}
