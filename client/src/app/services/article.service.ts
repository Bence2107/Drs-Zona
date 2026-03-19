import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {
  apiArticleCreatePost, apiArticleDeleteIdDelete,
  apiArticleGetAllArticlesGet$Json,
  apiArticleGetAllSummaryGet$Json,
  apiArticleGetRecentNumberGet$Json, apiArticleGetSlugGet$Json, apiArticleUpdatePost
} from '../api/functions';
import {map} from 'rxjs/operators';
import {ArticleListDto} from '../api/models/article-list-dto';
import {ArticleDetailDto} from '../api/models/article-detail-dto';
import {ArticleCreateDto} from '../api/models/article-create-dto';
import {ArticleUpdateDto} from '../api/models/article-update-dto';
import {ArticleListDtoPagedResult} from '../api/models/article-list-dto-paged-result';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {

  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) { }

  getBySlug(slug: string): Observable<ArticleDetailDto> {
    return apiArticleGetSlugGet$Json(this.http, this.apiConfig.rootUrl, {slug: slug}).pipe(
      map(response => {
        return response.body as ArticleDetailDto ?? [];
      })
    )
  }

  getAllArticles(page: number, pageSize:number, tag: string | undefined) : Observable<ArticleListDtoPagedResult> {
    return apiArticleGetAllArticlesGet$Json(this.http, this.apiConfig.rootUrl, {page: page, pageSize: pageSize, tag: tag}).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as ArticleListDtoPagedResult
      })
    )
  }

  getAllSummary(page: number, pageSize:number, tag: string | undefined) : Observable<ArticleListDtoPagedResult> {
    return apiArticleGetAllSummaryGet$Json(this.http, this.apiConfig.rootUrl, {page: page, pageSize: pageSize, tag: tag}).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as ArticleListDtoPagedResult
      })
    )
  }

  getRecent(count: number, tag: string | undefined): Observable<ArticleListDto[]> {
    return apiArticleGetRecentNumberGet$Json(this.http, this.apiConfig.rootUrl, { number: count, tag: tag }).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as ArticleListDto[] ?? [];
      })
    );
  }

  create(payload: ArticleCreateDto): Observable<void> {
    return apiArticleCreatePost(this.http, this.apiConfig.rootUrl, {body: payload}).pipe(map(() => void 0));
  }

  update(payload: ArticleUpdateDto): Observable<void> {
    return apiArticleUpdatePost(this.http, this.apiConfig.rootUrl, {body: payload}).pipe(map(() => void 0));
  }

  delete(id: string): Observable<void> {
    return apiArticleDeleteIdDelete(this.http, this.apiConfig.rootUrl, {id}).pipe(map(() => void 0));
  }
}
