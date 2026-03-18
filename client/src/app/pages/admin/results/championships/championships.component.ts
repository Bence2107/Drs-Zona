import {ChampionshipRowDto, SeriesLookupDto} from "../../../../api/models";
import {
  ChampionshipCreateDialogComponent
} from '../../../../components/dialogs/championship/championshipcreatedialog/championship-create-dialog.component';
import {Component, inject, OnInit, signal} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {ResultsService} from '../../../../services/results.service';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {FormsModule} from '@angular/forms';
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
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatDivider} from '@angular/material/list';
import {Router, RouterLink} from '@angular/router';
import {MatTooltip} from '@angular/material/tooltip';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption} from '@angular/material/core';
import {MatSelect} from '@angular/material/select';

@Component({
  selector: 'app-championships',
  imports: [
    MatButtonToggleGroup,
    FormsModule,
    MatButtonToggle,
    MatProgressSpinner,
    MatIcon,
    MatCard,
    MatCardHeader,
    MatCardSubtitle,
    MatCardTitle,
    MatButton,
    MatMenu,
    MatCardContent,
    MatMenuItem,
    MatMenuTrigger,
    MatDivider,
    RouterLink,
    MatCardActions,
    MatFabButton,
    MatTooltip,
    MatIconButton,
    MatFormField,
    MatLabel,
    MatOption,
    MatSelect
  ],
  templateUrl: './championships.component.html',
  styleUrl: './championships.component.scss',
})
export class ChampionshipsComponent implements OnInit {
  private dialog = inject(MatDialog);
  private resultService = inject(ResultsService);
  private router = inject(Router);

  seriesList = signal<SeriesLookupDto[]>([]);
  selectedSeriesId = signal<string | null>(null);
  championships = signal<ChampionshipRowDto[]>([]);
  isLoading = signal(false);

  ngOnInit() {
    this.resultService.getAllSeries().subscribe(res => {
      this.seriesList.set(res);
      if (res.length > 0) {
        this.onSeriesChange(res[0].id!);
      }
    });
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.loadChampionships();
  }

  loadChampionships() {
    const seriesId = this.selectedSeriesId();
    if (!seriesId) return;
    this.isLoading.set(true);
    this.resultService.getAllChampionshipsBySeries(seriesId).subscribe({
      next: res => {
        this.championships.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  statusIcon(status: string): string {
    return { Upcoming: 'schedule', Active: 'play_circle', Finished: 'check_circle' }[status] ?? 'help';
  }

  updateStatus(champ: ChampionshipRowDto, status: string) {
    this.resultService.updateChampionship(
      champ.driversChampId!,
      champ.constructorsChampId!,
      status
    ).subscribe({
      next: () => {
        champ.status = status;
      }
    });
  }

  openCreateDialog() {
    const ref = this.dialog.open(ChampionshipCreateDialogComponent, {
      width: '480px',
      data: { seriesList: this.seriesList() }
    });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadChampionships();
    });
  }

  protected goBack() {
    this.router.navigate(["results"]);
  }
}
