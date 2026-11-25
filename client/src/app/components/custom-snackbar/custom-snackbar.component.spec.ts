import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';

import { CustomSnackbarComponent } from './custom-snackbar.component';

describe('CustomSnackbarComponent', () => {
    let component: CustomSnackbarComponent;
    let fixture: ComponentFixture<CustomSnackbarComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CustomSnackbarComponent],
            providers: [
                { provide: MatSnackBarRef, useValue: {
                        dismiss: vi.fn().mockName("MatSnackBarRef.dismiss")
                    } },
                { provide: MAT_SNACK_BAR_DATA, useValue: { message: 'Message', actionLabel: 'OK' } },
            ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(CustomSnackbarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
