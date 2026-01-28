import { Component } from '@angular/core';
import {ReviewsListComponent} from './components/reviews-list/reviews-list.component';

@Component({
  selector: 'app-reviews',
  imports: [
    ReviewsListComponent
  ],
  templateUrl: './reviews.component.html',
  styleUrl: './reviews.component.scss'
})
export class ReviewsComponent {

}
