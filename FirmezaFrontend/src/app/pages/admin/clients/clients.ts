import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Client, ClientService } from '../../../services/client.service';

import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './clients.html',
  styleUrls: ['./clients.css']
})
export class ClientsComponent implements OnInit {
  clients: Client[] = [];
  clientForm: FormGroup;
  isEditing: boolean = false;
  showForm: boolean = false;
  showDetails: boolean = false;
  selectedClient: Client | null = null;
  currentClientId: number | null = null;

  constructor(
    private clientService: ClientService,
    private fb: FormBuilder
  ) {
    this.clientForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      document: ['', Validators.required],
      address: ['']
    });
  }

  ngOnInit() {
    this.loadClients();
  }

  loadClients() {
    this.clientService.getClients().subscribe(data => {
      this.clients = data;
    });
  }

  onSubmit() {
    if (this.clientForm.valid) {
      if (this.isEditing && this.currentClientId) {
        this.clientService.updateClient(this.currentClientId, this.clientForm.value).subscribe(() => {
          this.loadClients();
          this.resetForm();
        });
      } else {
        this.clientService.createClient(this.clientForm.value).subscribe(() => {
          this.loadClients();
          this.resetForm();
        });
      }
    }
  }

  editClient(client: Client) {
    this.isEditing = true;
    this.currentClientId = client.id;
    this.clientForm.patchValue(client);
    this.showForm = true;
  }

  deleteClient(id: number) {
    if (confirm('Are you sure you want to delete this client?')) {
      this.clientService.deleteClient(id).subscribe(() => {
        this.loadClients();
      });
    }
  }

  viewDetails(client: Client) {
    this.selectedClient = client;
    this.showDetails = true;
    this.showForm = false;
  }

  closeDetails() {
    this.showDetails = false;
    this.selectedClient = null;
  }

  resetForm() {
    this.isEditing = false;
    this.currentClientId = null;
    this.clientForm.reset();
    this.showForm = false;
    this.showDetails = false;
  }

  toggleForm() {
    this.showForm = !this.showForm;
    this.showDetails = false;
    if (!this.showForm) {
      this.resetForm();
    }
  }
}
