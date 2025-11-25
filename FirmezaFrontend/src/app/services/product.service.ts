import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Product {
  id: number;
  name: string;
  description?: string;
  type: string;
  price: number;
  stock: number;
  imageUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${environment.apiUrl}/Products`;

  constructor(private http: HttpClient) { }

  getProducts(search?: string, type?: string): Observable<Product[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (type) params = params.set('type', type);
    return this.http.get<Product[]>(this.apiUrl, { params });
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: any): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }

  updateProduct(id: number, product: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
