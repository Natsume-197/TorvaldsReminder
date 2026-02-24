import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InvoiceService } from './services/invoice.service';
import { Invoice, InvoiceStatus, Client } from './models/invoice.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly invoiceService = inject(InvoiceService);

  invoices = signal<Invoice[]>([]);
  clients = signal<Client[]>([]);
  isProcessing = signal(false);
  message = signal('');

  // Computed signal to join invoices with client names, IMPORTANT
  displayInvoices = computed(() => {
    const invs = this.invoices();
    const cls = this.clients();
    return invs.map(inv => ({
      ...inv,
      clientName: cls.find(c => c.id === inv.clientId)?.name || 'Desconocido'
    }));
  });

  ngOnInit() {
    this.reloadData();
  }

  reloadData() {
    forkJoin({
      invoices: this.invoiceService.getInvoices(),
      clients: this.invoiceService.getClients()
    }).subscribe({
      next: (data) => {
        this.invoices.set(data.invoices);
        this.clients.set(data.clients);
      },
      error: (err) => console.error('Error loading data', err)
    });
  }

  process() {
    this.isProcessing.set(true);
    this.message.set('Procesando facturas y enviando correos...');
    this.invoiceService.processInvoices().subscribe({
      next: (res) => {
        this.message.set(res.message);
        this.reloadData();
        this.isProcessing.set(false);
      },
      error: (err) => {
        console.error('Error processing invoices', err);
        this.message.set('Error al procesar las facturas.');
        this.isProcessing.set(false);
      }
    });
  }

  resetData() {
    this.message.set('Reseteando datos...');
    this.invoiceService.resetData().subscribe({
      next: (res) => {
        this.message.set(res.message);
        this.reloadData();
      },
      error: (err) => {
        console.error('Error resetting data', err);
        this.message.set('Error al resetear los datos.');
      }
    });
  }

  getInvoiceStatusName(status: InvoiceStatus): string {
    switch (status) {
      case InvoiceStatus.PrimerRecordatorio: return 'Primer Recordatorio';
      case InvoiceStatus.SegundoRecordatorio: return 'Segundo Recordatorio';
      case InvoiceStatus.Desactivado: return 'Desactivado';
      default: return 'Desconocido';
    }
  }

  getStatusClass(status: InvoiceStatus): string {
    switch (status) {
      case InvoiceStatus.PrimerRecordatorio: return 'status-primary';
      case InvoiceStatus.SegundoRecordatorio: return 'status-secondary';
      case InvoiceStatus.Desactivado: return 'status-disabled';
      default: return '';
    }
  }
}
