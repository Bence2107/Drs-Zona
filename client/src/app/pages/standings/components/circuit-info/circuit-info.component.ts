import {Component, input, OnChanges} from '@angular/core';
import {GrandPrixDetailDto} from '../../../../api/models/grand-prix-detail-dto';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {signal} from '@angular/core';
import {MatIcon} from '@angular/material/icon';
import {GrandsPrixService} from '../../../../services/api/grands-prix.service';

@Component({
  selector: 'app-circuit-info',
  imports: [MatProgressSpinner, MatIcon],
  templateUrl: './circuit-info.component.html',
  styleUrl: './circuit-info.component.scss',
})
export class CircuitInfoComponent implements OnChanges {
  grandPrixId = input.required<string>();

  constructor(private grandPrixService: GrandsPrixService) {}

  detail = signal<GrandPrixDetailDto | null>(null);
  isLoading = signal(false);

  ngOnChanges() {
    const gpId = this.grandPrixId();
    if (!gpId) return;
    this.isLoading.set(true);
    this.grandPrixService.getGrandPrixById(gpId).subscribe({
      next: (res) => {
        this.detail.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false),
    });
  }

  get circuitImageUrl(): string | null {
    const c = this.detail()?.circuitDetail;
    if (!c) return null;
    const isDark = document.documentElement.getAttribute('data-theme') === 'dark';
    if (isDark) {
      return c.darkImageUrl ?? c.lightImageUrl ?? null;
    }
    return c.lightImageUrl ?? c.darkImageUrl ?? null;
  }

  formatLength(length: number): string {
    return length.toFixed(3) + ' km';
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('hu-HU', {month: 'short', day: 'numeric'});
  }
}
