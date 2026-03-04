import {Component, OnInit, signal} from '@angular/core';
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
import {ResultsService} from '../../../../services/results.service';
import {YearLookupDto} from '../../../../api/models/year-lookup-dto';
import {GrandPrixLookupDto} from '../../../../api/models/grand-prix-lookup-dto';

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
export class EntryComponent implements OnInit {
  seriesList = signal<SeriesLookupDto[]>([]);
  selectedSeriesId = signal<string | null>(null);

  yearLookups = signal<YearLookupDto[]>([]);
  years = signal<number[]>([]);
  selectedYear = signal<number | null>(null);
  selectedChampionship = signal<string | null>(null);
  grandsPrix = signal<GrandPrixLookupDto[]>([]);
  isLoading = signal(false);

  constructor(private resultService: ResultsService) {}

  ngOnInit() {
    this.loadSeries();
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.loadYears(seriesId);
  }


  onYearChange(year: number) {
    this.selectedYear.set(year);

    const match = this.yearLookups().find(y => parseInt(y.season!) === year);
    if (match) {
      this.selectedChampionship.set(match.driversChampId ?? null);
      this.loadGrandsPrix(match.driversChampId!);
    }
  }

  loadSeries() {
    this.resultService.getAllSeries().subscribe(res => {
      this.seriesList.set(res);
      if (res.length > 0) this.onSeriesChange(res[0].id!);
    });
  }

  loadYears(seriesId: string) {
    this.resultService.getSeasonsBySeries(seriesId).subscribe(res => {
      this.yearLookups.set(res);

      const years = res
        .map(y => parseInt(y.season!))
        .filter(y => !isNaN(y))
        .sort((a, b) => b - a);

      this.years.set(years);

      if (years.length > 0) {
        this.onYearChange(years[0]);
      }
    });
  }

  loadGrandsPrix(champId: string) {
    this.isLoading.set(true);
    this.resultService.getGrandPrixByChampionship(champId).subscribe(res => {
      this.grandsPrix.set(res);
      this.isLoading.set(false);
    });
  }



}
