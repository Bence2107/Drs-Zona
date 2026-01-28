import {Component, Input} from '@angular/core';
import {UIComment} from '../../../../../api/models/ui-comment';
import {CommentService} from '../../../../../services/comment.service';
import {DatePipe, NgForOf, NgIf} from '@angular/common';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatProgressBar} from '@angular/material/progress-bar';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';

@Component({
  selector: 'app-comment-item',
  imports: [
    NgIf,
    DatePipe,
    MatIconButton,
    MatIcon,
    MatProgressBar,
    MatExpansionPanel,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle,
    NgForOf
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
}
