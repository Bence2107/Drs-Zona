import { Component } from '@angular/core';
import {MatCard, MatCardContent, MatCardHeader, MatCardImage, MatCardTitle} from '@angular/material/card';
import {RecentNewsComponent} from './components/recent-news/recent-news.component';

@Component({
  selector: 'app-home',
  imports: [
    MatCard,
    MatCardHeader,
    MatCardImage,
    MatCardTitle,
    MatCardContent,
    RecentNewsComponent
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

}
