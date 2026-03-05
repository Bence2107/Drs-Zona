import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChampionshipcreatedialogComponent } from './championshipcreatedialog.component';

describe('ChampionshipcreatedialogComponent', () => {
  let component: ChampionshipcreatedialogComponent;
  let fixture: ComponentFixture<ChampionshipcreatedialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChampionshipcreatedialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChampionshipcreatedialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
