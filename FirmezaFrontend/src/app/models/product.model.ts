export interface Product {
    id: number;
    name: string;
    description?: string;
    type?: string;
    price: number;
    stock: number;
    imageUrl?: string;
    // Add other properties if needed based on backend
}
