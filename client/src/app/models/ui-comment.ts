import { CommentDetailDto } from '../api/models/comment-detail-dto';

export interface UIComment extends CommentDetailDto {
  upVotes: number;
  downVotes: number;

  replies?: UIComment[];
  loaded?: boolean;
  loadingReplies: boolean;

  currentUserVote?: number;
}
