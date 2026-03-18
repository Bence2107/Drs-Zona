import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChampionshipCreateDialogComponent } from './championship-create-dialog.component';

describe('ChampionshipcreatedialogComponent', () => {
  let component: ChampionshipCreateDialogComponent;
  let fixture: ComponentFixture<ChampionshipCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChampionshipCreateDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChampionshipCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
