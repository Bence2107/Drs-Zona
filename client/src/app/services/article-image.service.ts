import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {
  apiArticleDraftsDraftIdDelete,
  apiArticleDraftsDraftIdImagesSlotPost,
  apiArticleDraftsDraftIdPromoteSlugPost
} from '../api/functions';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ArticleImageService {
  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) { }

  uploadDraftImage(draftId: string, slot: string, file: Blob): Observable<string> {
    return apiArticleDraftsDraftIdImagesSlotPost(this.http, this.apiConfig.rootUrl, {
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

  promoteDraftImages(draftId: string, slug: string): Observable<void> {
    return apiArticleDraftsDraftIdPromoteSlugPost(this.http, this.apiConfig.rootUrl, {
      draftId,
      slug
    }).pipe(map(() => void 0));
  }

  deleteDraft(draftId: string): Observable<void> {
    return apiArticleDraftsDraftIdDelete(this.http, this.apiConfig.rootUrl, {
      draftId
    }).pipe(map(() => undefined));
  }
}
