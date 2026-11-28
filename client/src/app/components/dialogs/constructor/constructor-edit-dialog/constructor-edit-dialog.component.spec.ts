import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConstructorEditDialogComponent } from './constructor-edit-dialog.component';

describe('ConstructorEditDialogComponent', () => {
  let component: ConstructorEditDialogComponent;
  let fixture: ComponentFixture<ConstructorEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConstructorEditDialogComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: MatDialogRef,
          useValue: {
            close: vi.fn().mockName('MatDialogRef.close'),
          },
        },
        { provide: MAT_DIALOG_DATA, useValue: { constructor: { id: '1' } } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ConstructorEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
