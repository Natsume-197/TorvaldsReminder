export enum InvoiceStatus {
  PrimerRecordatorio = 'PrimerRecordatorio',
  SegundoRecordatorio = 'SegundoRecordatorio',
  Desactivado = 'Desactivado'
}

export interface Client {
  id: string;
  name: string;
  email: string;
}

export interface Invoice {
  id: string;
  clientId: string;
  number: string;
  amount: number;
  dueDate: string;
  status: InvoiceStatus;
}
