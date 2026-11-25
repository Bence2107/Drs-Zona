import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { ChampionshipCreateDialogComponent } from './championship-create-dialog.component';

describe('ChampionshipcreatedialogComponent', () => {
    let component: ChampionshipCreateDialogComponent;
    let fixture: ComponentFixture<ChampionshipCreateDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ChampionshipCreateDialogComponent],
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

        fixture = TestBed.createComponent(ChampionshipCreateDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
