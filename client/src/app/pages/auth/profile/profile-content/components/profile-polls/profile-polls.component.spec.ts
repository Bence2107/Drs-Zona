import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfilePollsComponent } from './profile-polls.component';

describe('ProfilePollsComponent', () => {
  let component: ProfilePollsComponent;
  let fixture: ComponentFixture<ProfilePollsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfilePollsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfilePollsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
