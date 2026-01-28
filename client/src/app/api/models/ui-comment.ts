import {CommentDetailDto} from './comment-detail-dto';

export interface UIComment extends CommentDetailDto {
  replies?: CommentDetailDto[];
  loadingReplies?: boolean;
  loaded?: boolean;
}
