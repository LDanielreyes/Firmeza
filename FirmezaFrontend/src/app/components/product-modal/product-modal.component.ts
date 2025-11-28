import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface ProductFormData {
    id?: number;
    name: string;
    description?: string;
    type?: string;
    price: number;
    stock: number;
    category?: string;
    imageUrl?: string;
}

@Component({
    selector: 'app-product-modal',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './product-modal.component.html',
    styleUrls: ['./product-modal.component.css']
})
export class ProductModalComponent implements OnInit {
    @Input() product: ProductFormData | null = null;
    @Input() isEdit: boolean = false;
    @Output() save = new EventEmitter<ProductFormData>();
    @Output() close = new EventEmitter<void>();

    formData: ProductFormData = {
        name: '',
        description: '',
        type: 'Product',
        price: 0,
        stock: 0,
        category: '',
        imageUrl: ''
    };

    ngOnInit() {
        if (this.product) {
            this.formData = { ...this.product };
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
        return this.formData.name.trim().length > 0 &&
            this.formData.price >= 0 &&
            this.formData.stock >= 0;
    }
}
