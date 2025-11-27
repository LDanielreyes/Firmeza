import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ImportResult {
    totalRows: number;
    successCount: number;
    errorCount: number;
    errors: ImportError[];
    message: string;
}

export interface ImportError {
    row: number;
    field: string;
    message: string;
    value: string;
}

@Injectable({
    providedIn: 'root'
})
export class ImportExportService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    importExcel(file: File, endpoint: string): Observable<ImportResult> {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post<ImportResult>(`${this.apiUrl}/${endpoint}/import`, formData);
    }

    exportExcel(endpoint: string): void {
        this.downloadFile(`${this.apiUrl}/${endpoint}/export/excel`, `${endpoint}_export.xlsx`);
    }

    exportPdf(endpoint: string): void {
        this.downloadFile(`${this.apiUrl}/${endpoint}/export/pdf`, `${endpoint}_export.pdf`);
    }

    private downloadFile(url: string, filename: string): void {
        this.http.get(url, { responseType: 'blob' }).subscribe(blob => {
            const a = document.createElement('a');
            const objectUrl = URL.createObjectURL(blob);
            a.href = objectUrl;
            a.download = filename;
            a.click();
            URL.revokeObjectURL(objectUrl);
        });
    }
}
