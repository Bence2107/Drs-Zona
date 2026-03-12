import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentNewsComponent } from './recent-news.component';
import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {provideRouter} from '@angular/router';

describe('RecentNewsComponent', () => {
  let component: RecentNewsComponent;
  let fixture: ComponentFixture<RecentNewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecentNewsComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecentNewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
