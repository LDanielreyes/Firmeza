import { Product } from "./product.model";

export interface SaleLine {
    productId: number;
    product?: Product;
    quantity: number;
    pricePerUnit: number;
    netTotal: number;
}

export interface Receipt {
    id: number;
    clientId: number;
    receiptDate: string;
    grossTotal: number;
    ivaTotal: number;
    saleLines: SaleLine[];
}

export interface CreateSaleDto {
    clientId: number;
    items: {
        productId: number;
        quantity: number;
    }[];
}
