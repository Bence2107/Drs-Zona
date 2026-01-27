import {Component, EventEmitter, Input, Output} from '@angular/core';
import {MatButton} from '@angular/material/button';

@Component({
  selector: 'app-error-display',
  imports: [
    MatButton
  ],
  templateUrl: './error-display.component.html',
  styleUrl: './error-display.component.scss'
})
export class ErrorDisplayComponent {
  @Input() message: string = 'Csatlakozási hiba! Próbálkozzon újra!';
  @Output() retry = new EventEmitter<void>();

  onRetry() {
    this.retry.emit();
  }
}
