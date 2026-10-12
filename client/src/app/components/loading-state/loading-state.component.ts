import {booleanAttribute, Component, Input} from '@angular/core';
import {MatProgressBar} from '@angular/material/progress-bar';
import {MatProgressSpinner} from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-state',
  imports: [
    MatProgressBar,
    MatProgressSpinner,
  ],
  templateUrl: './loading-state.component.html',
  styleUrl: './loading-state.component.scss',
  host: {
    '[class.overlay]': 'overlay',
  }
})
export class LoadingStateComponent {
  @Input() variant: 'spinner' | 'bar' = 'spinner';
  @Input() label?: string;
  @Input() diameter: number = 48;
  @Input({transform: booleanAttribute}) compact: boolean = false;
  /** Covers already rendered content (parent needs the global `.loading-host` class) instead of replacing it. */
  @Input({transform: booleanAttribute}) overlay: boolean = false;
}
