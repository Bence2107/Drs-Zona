import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ArticleManageComponent } from './article-manage.component';

describe('ArticleManageComponent', () => {
  let component: ArticleManageComponent;
  let fixture: ComponentFixture<ArticleManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ArticleManageComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(ArticleManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
