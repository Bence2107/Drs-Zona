import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EntryCreateComponent } from './entry-create.component';

describe('EntryCreateComponent', () => {
  let component: EntryCreateComponent;
  let fixture: ComponentFixture<EntryCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EntryCreateComponent]
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
