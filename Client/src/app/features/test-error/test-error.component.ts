import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-test-error',
  imports: [MatButton],
  templateUrl: './test-error.component.html',
  styleUrl: './test-error.component.scss'
})
export class TestErrorComponent {
  baseUrl = environment.apiUrl;
  validationErros?: string[];
  
  constructor(private httpClient: HttpClient) {}

  get404Error() {
    this.httpClient.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    });
  }

  get400Error() {
    this.httpClient.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    });
  }

  get400ValidationError() {
    this.httpClient.post(this.baseUrl + 'buggy/validation-error', {}).subscribe({
      next: response => console.log(response),
      error: error => {
        this.validationErros = error,
        console.log(error)
        //error: error => console.log(error)
      }
    });
  }

  get401Error() {
    this.httpClient.get(this.baseUrl + 'buggy/unauthorized').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    });
  }

  get500Error() {
    this.httpClient.get(this.baseUrl + 'buggy/internal-error').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    });
  }
}
