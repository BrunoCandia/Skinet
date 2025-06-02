import { DeliveryMethod } from "./deliveryMethod"

export interface Order {
  id: string
  orderDate: string
  buyerEmail: string
  deliveryMethodId: string
  deliveryMethod: DeliveryMethod
  shippingAddress: ShippingAddress
  paymentSummary: PaymentSummary
  subtotal: number
  discount?: number
  status: number
  statusString: string
  paymentIntentId: string
  orderItems: OrderItem[]
  total: number
}

// export interface DeliveryMethod {
//   id: string
//   shortName: string
//   deliveryTime: string
//   description: string
//   price: number
// }

export interface ShippingAddress {
  name: string
  line1: string
  line2: any
  city: string
  state: string
  postalCode: string
  country: string
}

export interface PaymentSummary {
  last4: number
  brand: string
  expirationMonth: number
  expirationYear: number
}

export interface OrderItem {
  id: string
  itemOrdered: ItemOrdered
  price: number
  quantity: number
}

export interface ItemOrdered {
  productId: string
  productName: string
  pictureUrl: string
}

export interface OrderToCreate {
  shoppingCartId: string;
  deliveryMethodId: string;
  shippingAddress: ShippingAddress;
  paymentSummary: PaymentSummary;
  discount?: number
}