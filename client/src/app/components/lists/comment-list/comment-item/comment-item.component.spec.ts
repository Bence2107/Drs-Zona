import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

import { CommentItemComponent } from './comment-item.component';
import { UIComment } from '../../../../models/ui-comment';

describe('CommentItemComponent', () => {
  let component: CommentItemComponent;
  let fixture: ComponentFixture<CommentItemComponent>;

  const comment = {
    id: '1',
    content: 'Comment',
    username: 'user',
    upVotes: 0,
    downVotes: 0,
    loadingReplies: false,
  } as UIComment;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommentItemComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(CommentItemComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('comment', comment);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
