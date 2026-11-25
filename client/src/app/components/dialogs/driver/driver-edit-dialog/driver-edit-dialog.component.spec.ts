import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { provideNativeDateAdapter } from '@angular/material/core';

import { DriverEditDialogComponent } from './driver-edit-dialog.component';

describe('DriverEditDialogComponent', () => {
  let component: DriverEditDialogComponent;
  let fixture: ComponentFixture<DriverEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DriverEditDialogComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideNativeDateAdapter(),
        {
          provide: MatDialogRef,
          useValue: {
            close: vi.fn().mockName('MatDialogRef.close'),
          },
        },
        { provide: MAT_DIALOG_DATA, useValue: { driver: { id: '1' } } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DriverEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
