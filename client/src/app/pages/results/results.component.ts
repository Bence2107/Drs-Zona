import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { SeriesLookupDto } from '../../api/models/series-lookup-dto';
import { YearLookupDto } from '../../api/models/year-lookup-dto';
import { GrandPrixLookupDto } from '../../api/models/grand-prix-lookup-dto';
import { GrandRrixResultDto } from '../../api/models/grand-rrix-result-dto';
import { DriverStandingsResultDto } from '../../api/models/driver-standings-result-dto';
import { ConstructorStandingsResultDto } from '../../api/models/constructor-standings-result-dto';
import { ResultsService } from '../../services/results.service';

type ViewMode = 'results' | 'drivers' | 'constructors';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatButtonToggleModule,
    MatFormFieldModule, MatSelectModule, MatTableModule, MatProgressSpinnerModule
  ],
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.scss']
})
export class ResultsComponent implements OnInit {
  private standingsService = inject(ResultsService);

  // --- State Signals ---
  viewMode = signal<ViewMode>('results');
  seriesList = signal<SeriesLookupDto[]>([]);
  seasons = signal<YearLookupDto[]>([]);
  grandsPrix = signal<GrandPrixLookupDto[]>([]);
  sessions = signal<string[]>([]);

  selectedSeriesId = signal<string | null>(null);
  selectedDriversChampId = signal<string | null>(null);
  selectedConstructorsChampId = signal<string | null>(null);
  selectedGrandPrixId = signal<string | null>(null);
  selectedSession = signal<string | null>(null);

  raceResults = signal<GrandRrixResultDto[]>([]);
  driverStandings = signal<DriverStandingsResultDto[]>([]);
  constructorStandings = signal<ConstructorStandingsResultDto[]>([]);

  isLoading = signal(false);

  raceColumns = ['position', 'driver', 'constructor', 'time', 'points'];
  driverColumns = ['position', 'driver', 'constructor', 'points'];
  constructorColumns = ['position', 'constructor', 'points'];

  ngOnInit() {
    this.loadAllSeries();
  }

  loadAllSeries() {
    this.standingsService.getAllSeries().subscribe(res => {
      this.seriesList.set(res);
      if (res.length > 0) this.onSeriesChange(res[0].id!);
    });
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.selectedDriversChampId.set(null);
    this.standingsService.getSeasonsBySeries(seriesId).subscribe(res => {
      this.seasons.set(res);
    });
  }

  onViewModeChange(mode: ViewMode) {
    this.viewMode.set(mode);
    this.refreshData();
  }

  onSeasonChange(season: YearLookupDto) {
    this.selectedDriversChampId.set(season.driversChampId!);
    this.selectedConstructorsChampId.set(season.constructorsChampId!);

    this.selectedGrandPrixId.set(null);
    this.selectedSession.set(null);
    this.refreshData();
  }

  refreshData() {
    const drChampId = this.selectedDriversChampId();
    const coChampId = this.selectedConstructorsChampId();

    if (this.viewMode() === 'results' && drChampId) {
      this.standingsService.getGrandPrixByChampionship(drChampId).subscribe(res => {
        this.grandsPrix.set(res);
      });
    } else if (this.viewMode() === 'drivers' && drChampId) {
      this.isLoading.set(true);
      this.standingsService.getDriverStandings(drChampId).subscribe(res => {
        this.driverStandings.set(res.results || []);
        this.isLoading.set(false);
      });
    } else if (this.viewMode() === 'constructors' && coChampId) {
      this.isLoading.set(true);
      // Itt most már a helyes, különálló konstruktőri ID-t használjuk!
      this.standingsService.getConstructorStandings(coChampId).subscribe(res => {
        this.constructorStandings.set(res.results || []);
        this.isLoading.set(false);
      });
    }
  }

  onGrandPrixChange(gpId: string) {
    this.selectedGrandPrixId.set(gpId);
    this.selectedSession.set(null);
    this.standingsService.getSessionsByGrandPrix(gpId).subscribe(res => {
      this.sessions.set(res);
    });
  }

  loadResults(session: string) {
    this.selectedSession.set(session);
    const gpId = this.selectedGrandPrixId();
    if (!gpId || !session) return;

    this.isLoading.set(true);
    this.standingsService.getGrandPrixResults(gpId, session).subscribe({
      next: (res) => {
        this.raceResults.set(res.results || []);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}
