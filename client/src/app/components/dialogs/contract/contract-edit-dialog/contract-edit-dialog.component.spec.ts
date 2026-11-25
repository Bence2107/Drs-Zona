import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { ContractEditDialogComponent } from './contract-edit-dialog.component';

describe('ContractEditDialogComponent', () => {
    let component: ContractEditDialogComponent;
    let fixture: ComponentFixture<ContractEditDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ContractEditDialogComponent],
            providers: [
                provideHttpClient(),
                provideHttpClientTesting(),
                { provide: MatDialogRef, useValue: {
                        close: vi.fn().mockName("MatDialogRef.close")
                    } },
                { provide: MAT_DIALOG_DATA, useValue: { contract: { id: '1' } } },
            ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(ContractEditDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
