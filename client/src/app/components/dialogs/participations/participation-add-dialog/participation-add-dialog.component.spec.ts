import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { ParticipationAddDialogComponent } from './participation-add-dialog.component';

describe('ParticipationAddDialogComponent', () => {
  let component: ParticipationAddDialogComponent;
  let fixture: ComponentFixture<ParticipationAddDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ParticipationAddDialogComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: MatDialogRef,
          useValue: {
            close: vi.fn().mockName('MatDialogRef.close'),
          },
        },
        {
          provide: MAT_DIALOG_DATA,
          useValue: { lookup: { driversChampId: '1', constructorsChampId: '1' } },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ParticipationAddDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
