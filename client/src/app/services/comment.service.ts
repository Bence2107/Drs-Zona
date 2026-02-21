import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {CommentDetailDto} from '../api/models/comment-detail-dto';
import {
  apiCommentsGetCommentRepliesCommentIdGet$Json,
  apiCommentsGetCommentsWithoutRepliesArticleIdGet$Json, apiCommentsGetUsersCommentsUserIdGet$Json
} from '../api/functions';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) {}

  getCommentsWithoutReplies(id: string): Observable<CommentDetailDto[]> {
    return apiCommentsGetCommentsWithoutRepliesArticleIdGet$Json(this.http, this.apiConfig.rootUrl, {articleId: id}).pipe(
      map(response => {
        return response.body as CommentDetailDto[];
      })
    );
  }

  getCommentsReplies(id: string): Observable<CommentDetailDto[]> {
    return apiCommentsGetCommentRepliesCommentIdGet$Json(this.http, this.apiConfig.rootUrl, {commentId: id}).pipe(
      map(response => {
        return response.body as CommentDetailDto[];
      })
    );
  }


  getUsersComments(userId: string): Observable<CommentDetailDto[]> {
    return apiCommentsGetUsersCommentsUserIdGet$Json(this.http, this.apiConfig.rootUrl, {userId: userId}).pipe(
      map(response => {
        return response.body as CommentDetailDto[];
      })
    );
  }
}
