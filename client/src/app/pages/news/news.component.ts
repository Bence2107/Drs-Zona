import { Component } from '@angular/core';
import {NewsListComponent} from './components/news-list/news-list.component';

@Component({
  selector: 'app-news',
  imports: [
    NewsListComponent
  ],
  templateUrl: './news.component.html',
  styleUrl: './news.component.scss'
})
export class NewsComponent {

}
