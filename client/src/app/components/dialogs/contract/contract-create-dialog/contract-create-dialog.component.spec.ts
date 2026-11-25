import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { ContractCreateDialogComponent } from './contract-create-dialog.component';

describe('ContractCreateDialogComponent', () => {
  let component: ContractCreateDialogComponent;
  let fixture: ComponentFixture<ContractCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContractCreateDialogComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: MatDialogRef,
          useValue: {
            close: vi.fn().mockName('MatDialogRef.close'),
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ContractCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
