import { nanoid } from 'nanoid';

export type ShoppingCartType = {
    id: string;
    items: ShoppingCartItem[];
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
}