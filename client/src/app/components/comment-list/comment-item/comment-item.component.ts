import {Component, Input} from '@angular/core';
import {UIComment} from '../../../models/ui-comment';
import {CommentService} from '../../../services/comment.service';
import { DatePipe } from '@angular/common';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';

@Component({
  selector: 'app-comment-item',
  imports: [
    DatePipe,
    MatIconButton,
    MatIcon,
    MatExpansionPanel,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle
],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss'
})
export class CommentItemComponent {
  @Input() comment!: UIComment;
  @Input() isReply: boolean = false;

  constructor(private commentService: CommentService) {}

  loadReplies() {
    if (this.comment.loaded || !this.comment.id) return;

    this.comment.loadingReplies = true;
    this.commentService.getCommentsReplies(this.comment.id).subscribe({
      next: (replies) => {
        this.comment.replies = replies as UIComment[];
        this.comment.loaded = true;
        this.comment.loadingReplies = false;
      },
      error: () => this.comment.loadingReplies = false
    });
  }

  get avatarUrl(): string | null {
    const url = this.comment.userAvatarUrl;
    if (url == null) return "img/user/avatars/avatar.jpg";
    return `${url}`;
  }
}
