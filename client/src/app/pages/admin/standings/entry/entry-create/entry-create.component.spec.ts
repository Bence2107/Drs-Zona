import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

import { EntryCreateComponent } from './entry-create.component';

describe('EntryCreateComponent', () => {
  let component: EntryCreateComponent;
  let fixture: ComponentFixture<EntryCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EntryCreateComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EntryCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
