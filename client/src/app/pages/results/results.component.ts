import {Component, OnInit, signal, inject, computed} from '@angular/core';
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
import { StandingsService } from '../../services/standings.service';
import { SeasonOverviewDto } from '../../api/models/season-overview-dto';
import { DriverLookUpDto } from '../../api/models/driver-look-up-dto';
import { ConstructorLookUpDto } from '../../api/models/constructor-look-up-dto';
import { DriverSeasonResultDto } from '../../api/models/driver-season-result-dto';
import { ConstructorSeasonResultDto } from '../../api/models/constructor-season-result-dto';
import {CountryFlagPipe} from '../../pipes/country-flag.pipe';
import {toSignal} from '@angular/core/rxjs-interop';
import {map} from 'rxjs/operators';
import {BreakpointObserver} from '@angular/cdk/layout';
import {MatFabButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {RouterLink} from '@angular/router';
import {AuthService} from '../../services/auth.service';
import {MatTooltip} from '@angular/material/tooltip';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatDivider} from '@angular/material/list';
import {ChampionshipService} from '../../services/championship.service';

type ViewMode = 'results' | 'drivers' | 'constructors';
const ALL_GP_ID = 'all-season-overview';
const AGGREGATED_ID = 'aggregated';


@Component({
  selector: 'app-results',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatButtonToggleModule,
    MatFormFieldModule, MatSelectModule, MatTableModule, MatProgressSpinnerModule, CountryFlagPipe, MatIcon, RouterLink, MatFabButton, MatTooltip, MatMenuTrigger, MatMenu, MatMenuItem, MatDivider
  ],
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.scss']
})
export class ResultsComponent implements OnInit {
  constructor(
    private standingsService: StandingsService,
    private authService: AuthService,
    private championshipService: ChampionshipService,
  ) {}

  private breakpointObserver = inject(BreakpointObserver);

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 768px)').pipe(
      map(result => result.matches)
    )
  );

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

  driverList = signal<DriverLookUpDto[]>([]);
  constructorList = signal<ConstructorLookUpDto[]>([]);
  selectedDriverId = signal<string>(AGGREGATED_ID);
  selectedConstructorId = signal<string>(AGGREGATED_ID);

  driverSeasonResults = signal<DriverSeasonResultDto[]>([]);
  constructorSeasonResults = signal<ConstructorSeasonResultDto[]>([]);

  isLoading = signal(false);

  raceColumns = computed(() => {
    const session = this.selectedSession()?.toLowerCase() || '';
    const isQualy = session.includes('időmérő');

    const isF1 = this.seriesList().find(s => s.id === this.selectedSeriesId())?.name?.includes('Formula 1');

    if (isQualy) {
      return isF1
        ? ['position', 'driver', 'q1', 'q2', 'q3']
        : ['position', 'driver', 'time', 'laps'];
    }

    return ['position', 'driver', 'time', 'points'];
  });
  driverColumns = ['position', 'driver', 'points'];
  constructorColumns = ['position', 'constructor', 'points'];
  overviewColumns = ['grandPrixName', 'winnerName', 'teamName', 'laps', 'time'];
  driverSeasonColumns = ['grandPrixName', 'date', 'teamName', 'position', 'points'];
  constructorSeasonColumns = ['grandPrixName', 'date', 'points'];

  readonly ALL_GP_ID = ALL_GP_ID;
  readonly AGGREGATED_ID = AGGREGATED_ID;

  ngOnInit() {
    this.loadAllSeries();
  }

  loadAllSeries() {
    this.standingsService.getAllSeries().subscribe(res => {
      const filtered = res.filter(s => {
        const name = s.name?.toLowerCase() ?? '';
        return !name.includes('wec') && !name.includes('nascar');
      });
      this.seriesList.set(filtered);
      if (filtered.length > 0) this.onSeriesChange(filtered[0].id!);
    });
  }

  compareSeasons(o1: YearLookupDto, o2: YearLookupDto): boolean {
    return o1 && o2 ? o1.driversChampId === o2.driversChampId : o1 === o2;
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.championshipService.getSeasonsBySeries(seriesId).subscribe(res => {
      this.seasons.set(res);
      if (res.length > 0) {
        const latestSeason = res[0];
        this.selectedSeason.set(latestSeason);
        this.onSeasonChange(latestSeason);
      }
    });
  }

  onViewModeChange(mode: ViewMode) {
    this.viewMode.set(mode);
    const drChampId = this.selectedDriversChampId();

    this.selectedDriverId.set(AGGREGATED_ID);
    this.selectedConstructorId.set(AGGREGATED_ID);
    this.driverSeasonResults.set([]);
    this.constructorSeasonResults.set([]);

    this.isLoading.set(true);

    if (mode === 'drivers' && drChampId) {
      this.loadDriverList(drChampId);
    } else if (mode === 'constructors') {
      const coChampId = this.selectedConstructorsChampId();
      if (coChampId) this.loadConstructorList(coChampId);
    }

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
    this.selectedDriverId.set(AGGREGATED_ID);
    this.selectedConstructorId.set(AGGREGATED_ID);
    this.driverSeasonResults.set([]);
    this.constructorSeasonResults.set([]);

    if (this.viewMode() === 'results') {
      this.loadSeasonOverview(season.driversChampId!);

      this.championshipService.getGrandPrixByChampionship(season.driversChampId!).subscribe(res => {
        this.grandsPrix.set(res);
      });
    } else if (this.viewMode() === 'drivers') {
      this.loadDriverList(season.driversChampId!);
      this.refreshData();
    } else if (this.viewMode() === 'constructors') {
      this.loadConstructorList(season.constructorsChampId!);
      this.refreshData();
    } else {
      this.refreshData();
    }
  }

  private loadDriverList(drChampId: string) {
    this.championshipService.getDriversByDriversChampionship(drChampId).subscribe(res => {
      this.driverList.set(res);
    });
  }

  private loadConstructorList(coChampId: string) {
    this.championshipService.getConstructorsByConstChampionship(coChampId).subscribe(res => {
      this.constructorList.set(res);
    });
  }

  onDriverFilterChange(driverId: string) {
    this.selectedDriverId.set(driverId);
    if (driverId === AGGREGATED_ID) {
      this.driverSeasonResults.set([]);
      this.refreshData();
    } else {
      const drChampId = this.selectedDriversChampId();
      if (!drChampId) return;
      this.isLoading.set(true);
      this.standingsService.getDriverResultsBySeason(driverId, drChampId).subscribe({
        next: (res) => {
          this.driverSeasonResults.set(res || []);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
    }
  }

  onConstructorFilterChange(constructorId: string) {
    this.selectedConstructorId.set(constructorId);
    if (constructorId === AGGREGATED_ID) {
      this.constructorSeasonResults.set([]);
      this.refreshData();
    } else {
      const coChampId = this.selectedConstructorsChampId();
      if (!coChampId) return;
      this.isLoading.set(true);
      this.standingsService.getConstructorsResultsBySeason(constructorId, coChampId).subscribe({
        next: (res) => {
          this.constructorSeasonResults.set(res || []);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
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
    this.championshipService.getGrandPrixByChampionship(drChampId).subscribe(res => {
      this.grandsPrix.set(res);
      if (res.length > 0) {
        const latestGP = res[res.length - 1];
        this.selectedGrandPrixId.set(latestGP.id!);

        this.standingsService.getSessionsByGrandPrix(latestGP.id!).subscribe(sessions => {
          this.sessions.set(sessions);
          if (sessions.length > 0) {
            let defaultSession = sessions.find(s => s === 'Futam') ||
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
      this.championshipService.getGrandPrixByChampionship(drChampId).subscribe(res => {
        this.grandsPrix.set(res);
        this.isLoading.set(false);
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
          const preferredSession = sessions.find(s => s === 'Futam') ||
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

  isAdmin(): boolean {
      return this.authService.currentProfile()?.role === 'Admin' || this.authService.currentProfile()?.role === 'Manager';
  }
}
