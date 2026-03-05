import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GrandPrixCreateDialogComponent } from './grand-prix-creation-dialog.component';

describe('GrandPrixCreationDialogComponent', () => {
  let component: GrandPrixCreateDialogComponent;
  let fixture: ComponentFixture<GrandPrixCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GrandPrixCreateDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GrandPrixCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
