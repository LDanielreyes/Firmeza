import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface UserFormData {
    id?: number;
    fullName: string;
    email: string;
    phone: string;
    document: string;
    address: string;
    age: number;
    registerDate?: string;
}

@Component({
    selector: 'app-user-modal',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './user-modal.component.html',
    styleUrls: ['./user-modal.component.css']
})
export class UserModalComponent implements OnInit {
    @Input() user: UserFormData | null = null;
    @Input() isEdit: boolean = false;
    @Output() save = new EventEmitter<UserFormData>();
    @Output() close = new EventEmitter<void>();

    formData: UserFormData = {
        fullName: '',
        email: '',
        phone: '',
        document: '',
        address: '',
        age: 18
    };

    ngOnInit() {
        if (this.user) {
            this.formData = { ...this.user };
        }
    }

    onSubmit() {
        if (this.isValid()) {
            this.save.emit(this.formData);
        }
    }

    onClose() {
        this.close.emit();
    }

    isValid(): boolean {
        return this.formData.fullName.trim().length > 0 &&
            this.formData.email.trim().length > 0 &&
            this.formData.email.includes('@') &&
            this.formData.age > 0 && this.formData.age < 150;
    }
}
