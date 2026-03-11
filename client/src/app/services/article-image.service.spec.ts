import { TestBed } from '@angular/core/testing';

import { ArticleImageService } from './article-image.service';

describe('ArticleImageService', () => {
  let service: ArticleImageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ArticleImageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
