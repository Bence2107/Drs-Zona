import {Component, Input, OnInit} from '@angular/core';
import {UIComment} from '../../../../../../models/ui-comment';
import {CommentService} from '../../../../../../services/comment.service';
import {UserProfileResponse} from '../../../../../../api/models/user-profile-response';
import {MatCard} from '@angular/material/card';
import {CommentItemComponent} from '../../../../../../components/comment-list/comment-item/comment-item.component';
import {MatIcon} from '@angular/material/icon';

@Component({
  selector: 'app-profile-comments',
  imports: [
    MatCard,
    CommentItemComponent,
    MatIcon
  ],
  templateUrl: './profile-comments.component.html',
  styleUrl: './profile-comments.component.scss',
})
export class ProfileCommentsComponent implements OnInit {
  @Input() userData: UserProfileResponse | null = null;

  comments: UIComment[] = [];
  isLoading = true;

  constructor(private commentService: CommentService) {}

  ngOnInit(): void {
    this.loadUserComments();
  }

  loadUserComments() {
    if (!this.userData?.userId) return;

    this.isLoading = true;
    this.commentService.getUsersComments(this.userData.userId).subscribe({
      next: (data) => {
        this.comments = data.map(comment => ({
          ...comment,
          upVotes: comment.upVotes ?? 0,
          downVotes: comment.downVotes ?? 0,
          currentUserVote: comment.currentUserVote ?? null,
          replies: [],
          loaded: false,
          loadingReplies: false
        } as UIComment));

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Hiba történt a letöltéskor:', err);
        this.isLoading = false;
      }
    });
  }
}
