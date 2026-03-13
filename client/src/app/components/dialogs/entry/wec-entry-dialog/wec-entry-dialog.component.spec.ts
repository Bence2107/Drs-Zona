import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WecCarEntryDialogComponent } from './wec-entry-dialog.component';

describe('WecEntryDialogComponent', () => {
  let component: WecCarEntryDialogComponent;
  let fixture: ComponentFixture<WecCarEntryDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WecCarEntryDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WecCarEntryDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
