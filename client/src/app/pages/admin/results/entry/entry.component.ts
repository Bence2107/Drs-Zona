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
import {MatButton, MatFabButton, MatIconButton} from '@angular/material/button';
import {Router, RouterLink} from '@angular/router';
import {DatePipe} from '@angular/common';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {CountryFlagPipe} from '../../../../pipes/country-flag.pipe';
import {StandingsService} from '../../../../services/api/standings.service';
import {YearLookupDto} from '../../../../api/models/year-lookup-dto';
import {GrandPrixLookupDto} from '../../../../api/models/grand-prix-lookup-dto';
import {MatTooltip} from '@angular/material/tooltip';
import {
  GrandPrixManageDialogComponent
} from '../../../../components/dialogs/grand-prix/grand-prix-creation-dialog/grand-prix-creation-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {ChampionshipService} from '../../../../services/api/championship.service';

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
    CountryFlagPipe,
    MatFabButton,
    MatTooltip,
    MatIconButton
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

  constructor(
    private dialog: MatDialog,
    private router: Router,
    private championshipService: ChampionshipService,
    private resultService: StandingsService,
  ) {}

  private loadSeries() {
    this.resultService.getAllSeries().subscribe(res => {
      const filtered = res.filter(s => {
        const name = s.name?.toLowerCase() ?? '';
        return !name.includes('wec') && !name.includes('nascar');
      });
      this.seriesList.set(filtered);
      if (res.length > 0) this.onSeriesChange(filtered[0].id!);
    });
  }

  ngOnInit() {
    this.loadSeries();
  }

  loadYears(seriesId: string) {
    this.championshipService.getSeasonsBySeries(seriesId).subscribe(res => {
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
    this.championshipService.getGrandPrixByChampionship(champId).subscribe(res => {
      this.grandsPrix.set(res);
      this.isLoading.set(false);
    });
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

  openCreateDialog() {
    const ref = this.dialog.open(GrandPrixManageDialogComponent, {
      width: '560px',
      data: {
        seriesList: this.seriesList(),
        preselectedSeriesId: this.selectedSeriesId(),
        preselectedYear: this.selectedYear()
      }
    });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadGrandsPrix(this.selectedChampionship()!);
    });
  }

  protected goBack() {
    this.router.navigate(['/results']);
  }
}
