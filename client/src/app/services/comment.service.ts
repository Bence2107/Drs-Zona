import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {CommentDetailDto} from '../api/models/comment-detail-dto';
import {
  apiCommentsCreateUserIdPost,
  apiCommentsGetCommentRepliesCommentIdGet$Json,
  apiCommentsGetCommentsWithoutRepliesArticleIdGet$Json, apiCommentsGetUsersCommentsUserIdGet$Json,
  apiCommentsUpdateContentPost
} from '../api/functions';
import {map} from 'rxjs/operators';
import {CommentCreateDto} from '../api/models/comment-create-dto';
import {CommentContentUpdateDto} from '../api/models/comment-content-update-dto';

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

  createComment(dto: CommentCreateDto, userId: string): Observable<void> {
    return apiCommentsCreateUserIdPost(this.http, this.apiConfig.rootUrl, {
      userId,
      body: dto
    }).pipe(map(() => void 0));
  }

  updateComment(dto: CommentContentUpdateDto): Observable<void> {
    return apiCommentsUpdateContentPost(this.http, this.apiConfig.rootUrl, {
      body: dto
    }).pipe(map(() => void 0));
  }
}
