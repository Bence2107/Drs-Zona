import {Component, Input} from '@angular/core';
import {MatProgressBar} from '@angular/material/progress-bar';
import {MatProgressSpinner} from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-state',
  imports: [
    MatProgressBar,
    MatProgressSpinner,
  ],
  templateUrl: './loading-state.component.html',
  styleUrl: './loading-state.component.scss'
})
export class LoadingStateComponent {
  @Input() variant: 'spinner' | 'bar' = 'spinner';
  @Input() label?: string;
  @Input() diameter: number = 48;
  @Input() compact: boolean = false;
}
