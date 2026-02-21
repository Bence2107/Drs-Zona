import {Component, Input, OnInit} from '@angular/core';
import {UIComment} from '../../../../../../models/ui-comment';
import {CommentService} from '../../../../../../services/comment.service';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {DatePipe} from '@angular/common';
import {UserProfileResponse} from '../../../../../../api/models/user-profile-response';
import {MatCard} from '@angular/material/card';
import {CommentItemComponent} from '../../../../../../components/comment-list/comment-item/comment-item.component';

@Component({
  selector: 'app-profile-comments',
  imports: [
    MatIconButton,
    MatIcon,
    DatePipe,
    MatCard,
    CommentItemComponent
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
        this.comments = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Hiba történt a letöltéskor:', err);
        this.isLoading = false;
      }
    });
  }
}
