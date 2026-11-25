import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { GrandPixManageDialogComponent } from './grand-prix-manage-dialog.component';

describe('GrandPrixManageDialogComponent', () => {
    let component: GrandPixManageDialogComponent;
    let fixture: ComponentFixture<GrandPixManageDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [GrandPixManageDialogComponent],
            providers: [
                provideHttpClient(),
                provideHttpClientTesting(),
                { provide: MatDialogRef, useValue: {
                        close: vi.fn().mockName("MatDialogRef.close")
                    } },
                { provide: MAT_DIALOG_DATA, useValue: { seriesList: [] } },
            ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(GrandPixManageDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
