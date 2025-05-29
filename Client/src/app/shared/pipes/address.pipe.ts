import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { ShippingAddress } from '../models/order';

@Pipe({
  name: 'address'
})
export class AddressPipe implements PipeTransform {

  transform(value?: ConfirmationToken['shipping'] | ShippingAddress, ..._args: unknown[]): unknown {    
    if (value && 'address' in value && value.name) {
      const {line1, line2, city, state, country, postal_code} = 
          (value as ConfirmationToken['shipping'])?.address!;

      return `${value.name}, 
              ${line1}${line2 ? ', ' + line2 : ''}, 
              ${city}, ${state}, ${postal_code}, ${country}`;
    } else if (value && 'line1' in value) { //Only ShippingAddress has line1
      const {line1, line2, city, state, country, postalCode} = 
          value as ShippingAddress;

      return `${value.name}, 
              ${line1}${line2 ? ', ' + line2 : ''}, 
              ${city}, ${state}, ${postalCode}, ${country}`;
    }
    
    // if (value?.address && value.name) {
    //   const {line1, line2, city, state, country, postal_code} = value.address;

    //   return `${value.name}, 
    //           ${line1}${line2 ? ', ' + line2 : ''}, 
    //           ${city}, ${state}, ${postal_code}, ${country}`;
    // }
    
    return 'Unknown address';
  }
}
