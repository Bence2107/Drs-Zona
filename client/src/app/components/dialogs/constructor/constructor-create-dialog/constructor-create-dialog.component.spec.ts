import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { ConstructorCreateDialogComponent } from './constructor-create-dialog.component';

describe('ConstructorCreateDialogComponent', () => {
  let component: ConstructorCreateDialogComponent;
  let fixture: ComponentFixture<ConstructorCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConstructorCreateDialogComponent],
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

    fixture = TestBed.createComponent(ConstructorCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
