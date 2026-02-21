import {Component, Input, OnInit} from '@angular/core';
import { MatCard } from '@angular/material/card';
import {CommentService} from '../../services/comment.service';
import {UIComment} from '../../models/ui-comment';
import { ReactiveFormsModule } from '@angular/forms';
import {CommentItemComponent} from './comment-item/comment-item.component';
import {MatIcon} from '@angular/material/icon';

@Component({
  selector: 'app-comment-list',
  imports: [
    MatCard,
    ReactiveFormsModule,
    CommentItemComponent,
    MatIcon
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
