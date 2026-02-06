import {CommentDetailDto} from '../api/models/comment-detail-dto';

export interface UIComment extends CommentDetailDto {
  replies?: CommentDetailDto[];
  loadingReplies?: boolean;
  loaded?: boolean;
}
