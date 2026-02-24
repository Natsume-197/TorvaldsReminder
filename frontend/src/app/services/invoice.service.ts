import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice, Client } from '../models/invoice.model';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api';

  getInvoices(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(`${this.apiUrl}/invoices`);
  }

  getClients(): Observable<Client[]> {
    return this.http.get<Client[]>(`${this.apiUrl}/clients`);
  }

  processInvoices(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/invoices/process`, {});
  }

  resetData(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/seed/reset`, {});
  }
}
