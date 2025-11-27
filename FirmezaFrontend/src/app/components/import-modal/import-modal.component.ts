import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImportExportService, ImportResult } from '../../services/import-export.service';

@Component({
    selector: 'app-import-modal',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './import-modal.component.html',
    styleUrls: ['./import-modal.component.css']
})
export class ImportModalComponent {
    @Input() endpoint: string = '';
    @Input() title: string = 'Importar Datos';
    @Output() close = new EventEmitter<void>();
    @Output() imported = new EventEmitter<void>();

    file: File | null = null;
    uploading = false;
    result: ImportResult | null = null;
    error: string | null = null;
    showErrors = false;

    constructor(private importExportService: ImportExportService) { }

    onFileSelected(event: any) {
        this.file = event.target.files[0];
        this.result = null;
        this.error = null;
    }

    upload() {
        if (!this.file || !this.endpoint) return;

        this.uploading = true;
        this.error = null;
        this.result = null;

        this.importExportService.importExcel(this.file, this.endpoint).subscribe({
            next: (res) => {
                this.result = res;
                this.uploading = false;
                if (res.successCount > 0) {
                    this.imported.emit();
                }
            },
            error: (err) => {
                this.error = 'Error al subir el archivo. Por favor intente nuevamente.';
                this.uploading = false;
                console.error(err);
            }
        });
    }

    closeModal() {
        this.close.emit();
    }

    toggleErrors() {
        this.showErrors = !this.showErrors;
    }
}
