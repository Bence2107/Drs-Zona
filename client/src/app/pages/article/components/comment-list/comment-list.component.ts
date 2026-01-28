import {Component, Input, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle} from '@angular/material/card';
import {MatProgressBar} from '@angular/material/progress-bar';
import {DatePipe, NgForOf, NgIf} from '@angular/common';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {MatIcon} from '@angular/material/icon';
import {MatDivider} from '@angular/material/list';
import {CommentService} from '../../../../services/comment.service';
import {UIComment} from '../../../../api/models/ui-comment';
import { ReactiveFormsModule } from '@angular/forms';
import {MatIconButton} from '@angular/material/button';
import {CommentItemComponent} from './comment-item/comment-item.component';

@Component({
  selector: 'app-comment-list',
  imports: [
    MatCard,
    MatProgressBar,
    NgIf,
    MatCardHeader,
    MatCardTitle,
    MatCardSubtitle,
    MatCardContent,
    MatExpansionPanel,
    NgForOf,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle,
    MatIcon,
    MatDivider,
    ReactiveFormsModule,
    DatePipe,
    MatIconButton,
    CommentItemComponent,
  ],
  templateUrl: './comment-list.component.html',
  styleUrl: './comment-list.component.scss'
})
export class CommentListComponent implements OnInit {
  @Input() articleId!: string;
  @Input() articleUrl!: string;

  comments: UIComment[] = [];

  constructor(
    private commentService: CommentService,
  ) {}

  ngOnInit() {
    this.loadMainComments();
  }

  loadMainComments() {
    this.commentService.getCommentsWithoutReplies(this.articleId).subscribe(comments => {
      this.comments = comments as UIComment[];
    });
  }
}
