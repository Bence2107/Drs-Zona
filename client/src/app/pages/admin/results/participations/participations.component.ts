import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {ResultsService} from '../../../../services/results.service';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {YearLookupDto} from '../../../../api/models/year-lookup-dto';
import {ConfirmDialogComponent} from '../../../../components/dialogs/confirmdialog/confirmdialog.component';
import {
  ParticipationAddDialogComponent
} from '../../../../components/dialogs/participations/participation-add-dialog/participation-add-dialog.component';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {FormsModule} from '@angular/forms';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatIcon} from '@angular/material/icon';
import {MatDivider} from '@angular/material/list';
import {MatFabButton, MatIconButton} from '@angular/material/button';
import {MatTooltip} from '@angular/material/tooltip';
import {ParticipationListDto} from '../../../../api/models/participation-list-dto';
import {Router} from '@angular/router';
import {
  WecParticipationAddDialogComponent
} from '../../../../components/dialogs/participations/wec-participation-add-dialog/wec-participation-add-dialog.component';


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
    MatProgressSpinner,
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
export class ParticipationsComponent implements OnInit {
  private dialog = inject(MatDialog);
  private resultService = inject(ResultsService);
  private router = inject(Router);

  seriesList = signal<SeriesLookupDto[]>([]);
  selectedSeriesId = signal<string | null>(null);
  yearLookups = signal<YearLookupDto[]>([]);
  years = signal<number[]>([]);
  selectedYear = signal<number | null>(null);
  selectedYearLookup = signal<YearLookupDto | null>(null);
  participations = signal<ParticipationListDto | null>(null);
  isLoading = signal(false);
  selectedCategory = signal<string | null>(null);

  ngOnInit() {
    this.resultService.getAllSeries().subscribe(res => {
      this.seriesList.set(res);
      if (res.length > 0) this.onSeriesChange(res[0].id!);
    });
  }

  processedParticipations = computed(() => {
    const all = this.participations();
    const cat = this.selectedCategory();
    if (!all) return null;

    // Ha nem WEC, visszaadjuk az eredetit
    if (this.selectedYearLookup()?.pointSystem !== 'WEC') return all;

    return {
      ...all,
      drivers: cat ? all.drivers?.filter(d => (d as any).category === cat) : all.drivers
    };
  });

  wecCars = computed(() => {
    const data = this.processedParticipations();
    if (!data?.drivers) return [];

    const map = new Map<number, any>();
    data.drivers.forEach(d => {
      if (!map.has(d.driverNumber!)) {
        map.set(d.driverNumber!, { number: d.driverNumber, team: d.teamName, drivers: [] });
      }
      map.get(d.driverNumber!).drivers.push(d.name);
    });
    return Array.from(map.values());
  });

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.resultService.getSeasonsBySeries(seriesId).subscribe(res => {
      this.yearLookups.set(res);
      const years = res.map(y => parseInt(y.season!)).filter(y => !isNaN(y)).sort((a, b) => b - a);
      this.years.set(years);
      if (years.length > 0) this.onYearChange(years[0]);
    });
  }

  onYearChange(year: number) {
    this.selectedYear.set(year);
    const match = this.yearLookups().find(y => parseInt(y.season!) === year);
    if (match) {
      this.selectedYearLookup.set(match);
      this.loadParticipations(match);
    }
  }

  loadParticipations(lookup: YearLookupDto) {
    if (!lookup.driversChampId || !lookup.constructorsChampId) return;
    this.isLoading.set(true);
    this.resultService.getParticipations(lookup.driversChampId, lookup.constructorsChampId).subscribe({
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
      this.resultService.removeDriverParticipation(driverId, lookup.driversChampId!).subscribe({
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
      this.resultService.removeConstructorParticipation(constructorId, lookup.constructorsChampId!).subscribe({
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

  openWecAddDialog() {
    const lookup = this.selectedYearLookup();
    if (lookup) this.dialog.open(WecParticipationAddDialogComponent, { width: '600px', data: { driversChampId: lookup.driversChampId, constructorsChampId: lookup.constructorsChampId } }).afterClosed().subscribe(res => { if (res) this.loadParticipations(lookup); });
  }
}
