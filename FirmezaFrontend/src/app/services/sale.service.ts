import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SaleService {
  private apiUrl = `${environment.apiUrl}/Sales`;

  constructor(private http: HttpClient) { }

  createSale(saleData: any): Observable<any> {
    return this.http.post(this.apiUrl, saleData);
  }

  getSales(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getSale(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
}
