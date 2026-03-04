import {Component, inject, signal} from '@angular/core';
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
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatButton, MatFabButton} from '@angular/material/button';
import {MatDivider} from '@angular/material/list';
import {MatTooltip} from '@angular/material/tooltip';
import {RouterLink} from '@angular/router';
import {MatDialog} from '@angular/material/dialog';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {YearLookupDto} from '../../../../api/models/year-lookup-dto';
import {ResultsService} from '../../../../services/results.service';

@Component({
  selector: 'app-championships',
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
    MatMenu,
    MatMenuItem,
    MatButton,
    MatMenuTrigger,
    MatCardContent,
    MatDivider,
    MatCardActions,
    MatTooltip,
    MatFabButton,
    RouterLink
  ],
  templateUrl: './championships.component.html',
  styleUrl: './championships.component.scss',
})
export class ChampionshipsComponent {
  private dialog = inject(MatDialog);
  private resultsService = inject(ResultsService);

  seriesList = signal<SeriesLookupDto[]>([]);
  selectedSeriesId = signal<string | null>(null);

  seasons = signal<YearLookupDto[]>([]);
  selectedSeason = signal<number | null>(null);

  championships = signal<ChampionshipRowDto[]>([]);
  isLoading = signal(false);

  statusIcon(status: string): string {
    return { Upcoming: 'schedule', Active: 'play_circle', Finished: 'check_circle' }[status] ?? 'help';
  }

  onSeriesChange(seriesId: string) {
    this.selectedSeriesId.set(seriesId);
    this.loadYears(seriesId);
  }


  onSeasonChange(year: number) {
    this.selectedSeason.set(year);

    const match = this.seasons().find(y => parseInt(y.season!) === year);
    if (match) {
      this.loadChampionships(match.driversChampId!);
    }
  }

  updateStatus(champ: ChampionshipRowDto, status: string) {
    this.champService.updateStatus(champ.driversChampId, champ.constructorsChampId, status).subscribe(() => {
      champ.status = status;
    });
  }

  openCreateDialog() {
    const ref = this.dialog.open(ChampionshipCreateDialogComponent, {
      width: '480px',
      data: {seriesList: this.seriesList()}
    });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadChampionships();
    });
  }
}
