import {Component, EventEmitter, Input, Output} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';

@Component({
  selector: 'app-error-display',
  imports: [
    MatButton,
    MatIcon,
  ],
  templateUrl: './error-display.component.html',
  styleUrl: './error-display.component.scss'
})
export class ErrorDisplayComponent {
  @Input() title: string = 'A szerver jelenleg nem elérhető';
  @Input() message: string = 'Kérjük, próbálkozzon később vagy ellenőrizze a kapcsolatot.';
  @Output() retry = new EventEmitter<void>();

  onRetry() {
    window.location.reload();
    this.retry.emit();
  }
}
