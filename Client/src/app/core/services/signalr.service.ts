import { Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { Order } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  hubUrl = environment.hubUrl;
  hubConnection?: HubConnection;
  orderSignal = signal<Order | null>(null);

  constructor() { }

  createHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(error => console.log(error));

    this.hubConnection.on('OrderCompleteNotification', (order: Order) => {
      // Used in the checkout-success.component
      this.orderSignal.set(order);
      console.log('OrderSignal set: ' + order);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop()
        .catch(error => console.log(error));
    }
  }
}
