import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {
  apiArticleImageDraftIdDelete,
  apiArticleImageDraftIdImagesSlotPost,
  apiArticleImageDraftIdPromoteSlugPost
} from '../api/functions';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ArticleImageService {
  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) { }

  uploadDraftImage(draftId: string, slot: string, file: Blob): Observable<string> {
    return apiArticleImageDraftIdImagesSlotPost(this.http, this.apiConfig.rootUrl, {
      draftId,
      slot,
      body: { file }
    }).pipe(
      map((response: any) => {
        try {
          const data = JSON.parse(response.body);
          return data.url;
        } catch {
          return response.body?.url || '';
        }
      })
    );
  }

  deleteDraft(draftId: string): Observable<void> {
    return apiArticleImageDraftIdDelete(this.http, this.apiConfig.rootUrl, {
      draftId
    }).pipe(map(() => undefined));
  }

  promoteDraftImages(draftId: string, slug: string): Observable<void> {
    return apiArticleImageDraftIdPromoteSlugPost(this.http, this.apiConfig.rootUrl, {
      draftId,
      slug
    }).pipe(map(() => void 0));
  }
}
