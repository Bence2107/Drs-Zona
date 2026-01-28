import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {
  apiArticleGetAllArticlesGet$Json,
  apiArticleGetAllSummaryGet$Json,
  apiArticleGetRecentNumberGet$Json
} from '../api/functions';
import {map} from 'rxjs/operators';
import {ArticleListDto} from '../api/models/article-list-dto';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {

  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) { }

  getAllArticles() : Observable<ArticleListDto[]> {
    return apiArticleGetAllArticlesGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as ArticleListDto[] ?? []
      })
    )
  }

  getAllSummary() : Observable<ArticleListDto[]> {
    return apiArticleGetAllSummaryGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as ArticleListDto[] ?? []
      })
    )
  }

  getRecent(count: number): Observable<ArticleListDto[]> {
    return apiArticleGetRecentNumberGet$Json(this.http, this.apiConfig.rootUrl, { number: count }).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as ArticleListDto[] ?? [];
      })
    );
  }
}
