import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Client {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  document: string;
  address: string;
  registerDate: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private apiUrl = `${environment.apiUrl}/Clients`;

  constructor(private http: HttpClient) { }

  getClients(): Observable<Client[]> {
    return this.http.get<Client[]>(this.apiUrl);
  }

  getClient(id: number): Observable<Client> {
    return this.http.get<Client>(`${this.apiUrl}/${id}`);
  }

  createClient(client: any): Observable<Client> {
    return this.http.post<Client>(this.apiUrl, client);
  }

  updateClient(id: number, client: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, client);
  }

  deleteClient(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
