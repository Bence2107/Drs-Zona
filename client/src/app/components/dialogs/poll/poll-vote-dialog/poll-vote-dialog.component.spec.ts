import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { PollVoteDialogComponent } from './poll-vote-dialog.component';

describe('PollVoteDialogComponent', () => {
    let component: PollVoteDialogComponent;
    let fixture: ComponentFixture<PollVoteDialogComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [PollVoteDialogComponent],
            providers: [
                provideHttpClient(),
                provideHttpClientTesting(),
                { provide: MatDialogRef, useValue: {
                        close: vi.fn().mockName("MatDialogRef.close")
                    } },
                { provide: MAT_DIALOG_DATA, useValue: { poll: { id: '1', pollOptions: [] }, userId: null } },
            ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(PollVoteDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
