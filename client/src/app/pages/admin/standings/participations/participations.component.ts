import {Component, OnInit, signal} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {StandingsService} from '../../../../services/api/standings.service';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {YearLookupDto} from '../../../../api/models/year-lookup-dto';
import {ConfirmDialogComponent} from '../../../../components/dialogs/confirmdialog/confirm-dialog.component';
import {
  ParticipationAddDialogComponent
} from '../../../../components/dialogs/participations/participation-add-dialog/participation-add-dialog.component';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {FormsModule} from '@angular/forms';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {LoadingStateComponent} from '../../../../components/loading-state/loading-state.component';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatIcon} from '@angular/material/icon';
import {MatDivider} from '@angular/material/list';
import {MatFabButton, MatIconButton} from '@angular/material/button';
import {MatTooltip} from '@angular/material/tooltip';
import {ParticipationListDto} from '../../../../api/models/participation-list-dto';
import {ActivatedRoute, Router} from '@angular/router';
import {ChampionshipService} from '../../../../services/api/championship.service';


@Component({
  selector: 'app-participations',
  imports: [
    MatButtonToggleGroup,
    FormsModule,
    MatButtonToggle,
    MatFormField,
    MatLabel,
    MatSelect,
    MatOption,
    LoadingStateComponent,
    MatCard,
    MatIcon,
    MatDivider,
    MatCardContent,
    MatIconButton,
    MatTooltip,
    MatFabButton
  ],
  templateUrl: './participations.component.html',
  styleUrl: './participations.component.scss',
})
export class ParticipationsComponent implements OnInit{

  seriesList = signal<SeriesLookupDto[]>([]);
  selectedSeriesId = signal<string | null>(null);
  yearLookups = signal<YearLookupDto[]>([]);
  years = signal<number[]>([]);
  selectedYear = signal<number | null>(null);
  selectedYearLookup = signal<YearLookupDto | null>(null);
  participations = signal<ParticipationListDto | null>(null);
  // Starts true: the list is loaded after the series lookup, the empty state must not flash before it
  isLoading = signal(true);

  constructor(
    public dialog: MatDialog,
    private route: ActivatedRoute,
    private router: Router,
    private championshipService: ChampionshipService,
    private standingsService: StandingsService
  ) {}

  ngOnInit() {
    const champIdFromRoute = this.route.snapshot.paramMap.get('champId');

    this.standingsService.getAllSeries().subscribe({
      next: res => {
        const filtered = res.filter(s => {
          const name = s.name?.toLowerCase() ?? '';
          return !name.includes('wec') && !name.includes('nascar');
        });
        this.seriesList.set(filtered);
        if (filtered.length === 0) {
          this.isLoading.set(false);
          return;
        }

        if (champIdFromRoute) {
          this.preselectByChampId(champIdFromRoute, filtered);
        } else {
          this.onSeriesChange(filtered[0].id!);
        }
      },
      error: () => this.isLoading.set(false)
    });
  }

  private preselectByChampId(champId: string, series: SeriesLookupDto[]) {
    const tryNext = (index: number) => {
      if (index >= series.length) {
        this.onSeriesChange(series[0].id!);
        return;
      }
      this.championshipService.getSeasonsBySeries(series[index].id!).subscribe(lookups => {
        const match = lookups.find(
          l => l.driversChampId === champId || l.constructorsChampId === champId
        );
        if (match) {
          this.selectedSeriesId.set(series[index].id!);
          this.yearLookups.set(lookups);
          const years = lookups.map(y => parseInt(y.season!)).filter(y => !isNaN(y)).sort((a, b) => b - a);
          this.years.set(years);
          this.selectedYear.set(parseInt(match.season!));
          this.selectedYearLookup.set(match);
          this.loadParticipations(match);
        } else {
          tryNext(index + 1);
        }
      });
    };
    tryNext(0);
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.championshipService.getSeasonsBySeries(seriesId).subscribe({
      next: res => {
        this.yearLookups.set(res);
        const years = res.map(y => parseInt(y.season!)).filter(y => !isNaN(y)).sort((a, b) => b - a);
        this.years.set(years);
        if (years.length > 0) this.onYearChange(years[0]);
        else this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onYearChange(year: number) {
    this.selectedYear.set(year);
    const match = this.yearLookups().find(y => parseInt(y.season!) === year);
    if (match) {
      this.selectedYearLookup.set(match);
      this.loadParticipations(match);
    } else {
      this.isLoading.set(false);
    }
  }

  loadParticipations(lookup: YearLookupDto) {
    if (!lookup.driversChampId || !lookup.constructorsChampId) {
      this.isLoading.set(false);
      return;
    }
    this.isLoading.set(true);
    this.championshipService.getParticipations(lookup.driversChampId, lookup.constructorsChampId).subscribe({
      next: res => {
        this.participations.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  removeDriver(driverId: string) {
    const lookup = this.selectedYearLookup();
    if (!lookup?.driversChampId) return;

    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Versenyző eltávolítása',
        message: 'Biztosan eltávolítod ezt a versenyzőt a bajnokságból?',
        confirmText: 'Eltávolítás'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.championshipService.removeDriverParticipation(driverId, lookup.driversChampId!).subscribe({
        next: () => this.loadParticipations(lookup)
      });
    });
  }

  removeConstructor(constructorId: string) {
    const lookup = this.selectedYearLookup();
    if (!lookup?.constructorsChampId) return;

    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Konstruktőr eltávolítása',
        message: 'Biztosan eltávolítod ezt a konstruktőrt a bajnokságból?',
        confirmText: 'Eltávolítás'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.championshipService.removeConstructorParticipation(constructorId, lookup.constructorsChampId!).subscribe({
        next: () => this.loadParticipations(lookup)
      });
    });
  }

  openAddDialog() {
    const lookup = this.selectedYearLookup();
    if (!lookup) return;

    const ref = this.dialog.open(ParticipationAddDialogComponent, {
      width: '600px',
      data: { lookup }
    });

    ref.afterClosed().subscribe(result => {
      if (result) this.loadParticipations(lookup);
    });
  }

  protected goBack() {
    this.router.navigate(["results"]);
  }
}
