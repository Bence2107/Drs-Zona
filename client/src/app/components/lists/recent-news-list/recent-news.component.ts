import {Component, Input} from '@angular/core';
import {DatePipe, NgClass} from '@angular/common';
import {RouterLink} from '@angular/router';
import {MatCard, MatCardContent, MatCardImage} from '@angular/material/card';
import {ArticleListDto} from '../../../api/models/article-list-dto';

@Component({
  selector: 'app-recent-news-list',
  imports: [
    RouterLink,
    MatCard,
    MatCardContent,
    DatePipe,
    MatCardImage,
    NgClass
  ],
  templateUrl: './recent-news.component.html',
  styleUrl: './recent-news.component.scss'
})
export class RecentNewsComponent {
  @Input() articles: ArticleListDto[] = []
}
