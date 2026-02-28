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
import { SeasonOverviewDto } from '../../api/models/season-overview-dto';


type ViewMode = 'results' | 'drivers' | 'constructors';
const ALL_GP_ID = 'all-season-overview';

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

  viewMode = signal<ViewMode>('results');
  seriesList = signal<SeriesLookupDto[]>([]);
  seasons = signal<YearLookupDto[]>([]);
  grandsPrix = signal<GrandPrixLookupDto[]>([]);
  sessions = signal<string[]>([]);
  seasonOverview = signal<SeasonOverviewDto[]>([]);

  selectedSeriesId = signal<string | null>(null);
  selectedDriversChampId = signal<string | null>(null);
  selectedConstructorsChampId = signal<string | null>(null);
  selectedGrandPrixId = signal<string | null>(null);
  selectedSession = signal<string | null>(null);
  selectedSeason = signal<YearLookupDto | null>(null);

  raceResults = signal<GrandRrixResultDto[]>([]);
  driverStandings = signal<DriverStandingsResultDto[]>([]);
  constructorStandings = signal<ConstructorStandingsResultDto[]>([]);

  isLoading = signal(false);

  raceColumns = ['position', 'driver', 'constructor', 'time', 'points'];
  driverColumns = ['position', 'driver', 'constructor', 'points'];
  constructorColumns = ['position', 'constructor', 'points'];
  overviewColumns = ['grandPrixName', 'winnerName', 'teamName', 'laps', 'time'];

  ngOnInit() {
    this.loadAllSeries();
  }

  loadAllSeries() {
    this.standingsService.getAllSeries().subscribe(res => {
      this.seriesList.set(res);
      if (res.length > 0) this.onSeriesChange(res[0].id!);
    });
  }

  compareSeasons(o1: YearLookupDto, o2: YearLookupDto): boolean {
    return o1 && o2 ? o1.driversChampId === o2.driversChampId : o1 === o2;
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.standingsService.getSeasonsBySeries(seriesId).subscribe(res => {
      this.seasons.set(res);
      if (res.length > 0) {
        const latestSeason = res[res.length - 1];
        this.selectedSeason.set(latestSeason);
        this.onSeasonChange(latestSeason);
      }
    });
  }

  onViewModeChange(mode: ViewMode) {
    this.viewMode.set(mode);
    const drChampId = this.selectedDriversChampId();

    if (mode === 'results' && drChampId && !this.selectedGrandPrixId()) {
      this.loadLatestGrandPrix(drChampId);
    } else {
      this.refreshData();
    }
  }

  onSeasonChange(season: YearLookupDto) {
    this.selectedSeason.set(season);
    this.selectedDriversChampId.set(season.driversChampId!);
    this.selectedConstructorsChampId.set(season.constructorsChampId!);

    this.selectedGrandPrixId.set(ALL_GP_ID);
    this.selectedSession.set(null);

    if (this.viewMode() === 'results') {
      this.loadSeasonOverview(season.driversChampId!);

      this.standingsService.getGrandPrixByChampionship(season.driversChampId!).subscribe(res => {
        this.grandsPrix.set(res);
      });
    } else {
      this.refreshData();
    }
  }

  loadSeasonOverview(drChampId: string) {
    this.isLoading.set(true);
    this.selectedGrandPrixId.set(ALL_GP_ID);
    this.selectedSession.set(null);

    this.standingsService.getSeasonOverview(drChampId).subscribe({
      next: (res) => {
        this.seasonOverview.set(res || []);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  private loadLatestGrandPrix(drChampId: string) {
    this.isLoading.set(true);
    this.standingsService.getGrandPrixByChampionship(drChampId).subscribe(res => {
      this.grandsPrix.set(res);
      if (res.length > 0) {
        const latestGP = res[res.length - 1];
        this.selectedGrandPrixId.set(latestGP.id!);

        this.standingsService.getSessionsByGrandPrix(latestGP.id!).subscribe(sessions => {
          this.sessions.set(sessions);
          if (sessions.length > 0) {
            let defaultSession = sessions.find(s => s === 'Race') ||
              sessions.find(s => s === 'Sprint') ||
              sessions[sessions.length - 1];

            this.loadResults(defaultSession);
          } else {
            this.isLoading.set(false);
          }
        });
      } else {
        this.isLoading.set(false);
      }
    });
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
      this.standingsService.getConstructorStandings(coChampId).subscribe(res => {
        this.constructorStandings.set(res.results || []);
        this.isLoading.set(false);
      });
    }
  }

  onGrandPrixChange(gpId: string) {
    if (gpId === ALL_GP_ID) {
      this.loadSeasonOverview(this.selectedDriversChampId()!);
      return;
    }

    this.selectedGrandPrixId.set(gpId);
    this.selectedSession.set(null);
    this.isLoading.set(true);

    this.standingsService.getSessionsByGrandPrix(gpId).subscribe({
      next: (sessions) => {
        this.sessions.set(sessions);
        if (sessions.length > 0) {
          const preferredSession = sessions.find(s => s === 'Race') ||
            sessions.find(s => s === 'Sprint') ||
            sessions[sessions.length - 1];

          this.loadResults(preferredSession);
        } else {
          this.isLoading.set(false);
        }
      },
      error: () => this.isLoading.set(false)
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

  readonly ALL_GP_ID = ALL_GP_ID;
}
