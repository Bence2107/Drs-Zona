import {Component, signal} from '@angular/core';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {FormsModule} from '@angular/forms';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatIcon} from '@angular/material/icon';
import {
  MatCard,
  MatCardActions,
  MatCardContent,
  MatCardHeader,
  MatCardSubtitle,
  MatCardTitle
} from '@angular/material/card';
import {MatButton} from '@angular/material/button';
import {RouterLink} from '@angular/router';
import {DatePipe} from '@angular/common';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {CountryFlagPipe} from '../../../../pipes/country-flag.pipe';

@Component({
  selector: 'app-entry',
  imports: [
    MatButtonToggleGroup,
    FormsModule,
    MatButtonToggle,
    MatFormField,
    MatLabel,
    MatSelect,
    MatOption,
    MatProgressSpinner,
    MatIcon,
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardSubtitle,
    MatCardContent,
    MatCardActions,
    MatButton,
    RouterLink,
    DatePipe,
    CountryFlagPipe
  ],
  templateUrl: './entry.component.html',
  styleUrl: './entry.component.scss',
})
export class EntryComponent {
  seriesList = signal<SeriesLookupDto[]>([]);
  selectedSeriesId = signal<string | null>(null);
  years = signal<number[]>([]);
  selectedYear = signal<number | null>(null);
  grandsPrix = signal<any[]>([]);
  isLoading = signal(false);

  ngOnInit() {
    this.loadSeries();
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.loadYears(seriesId);
  }

  onYearChange(year: number) {
    this.selectedYear.set(year);
    this.loadGrandsPrix();
  }
}
