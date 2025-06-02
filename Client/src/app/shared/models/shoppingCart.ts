import { nanoid } from 'nanoid';
import { Coupon } from './coupon';

export type ShoppingCartType = {
    id: string;
    items: ShoppingCartItem[];
    deliveryMethodId?: string;
    clientSecret?: string;
    paymentIntentId?: string;
}

export type ShoppingCartItem = {
    productId: string;
    productName: string;
    price: number;
    quantity: number;
    pictureUrl: string;
    brand: string;
    type: string;
}

export class ShoppingCart implements ShoppingCartType {
    id = nanoid();
    items: ShoppingCartItem[] = [];
    deliveryMethodId?: string;
    clientSecret?: string;
    paymentIntentId?: string;
    coupon?: Coupon;
}