import {Component, inject} from '@angular/core';
import {AGGREGATED_ID, ALL_GP_ID, StandingsStateService} from '../standings-state.service';
import {
  MatCell, MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow,
  MatRowDef,
  MatTable
} from '@angular/material/table';
import {LoadingStateComponent} from '../../../../components/loading-state/loading-state.component';
import {DatePipe} from '@angular/common';
import {CountryFlagPipe} from '../../../../pipes/country-flag.pipe';
import {CircuitInfoComponent} from '../circuit-info/circuit-info.component';

@Component({
  selector: 'app-results-table',
  imports: [
    MatTable,
    LoadingStateComponent,
    MatColumnDef,
    MatHeaderRow,
    MatRow,
    MatHeaderCell,
    MatCell,
    MatRowDef,
    MatHeaderRowDef,
    DatePipe,
    MatCellDef,
    MatHeaderCellDef,
    CountryFlagPipe,
    CircuitInfoComponent
  ],
  templateUrl: './standings-table.component.html',
  styleUrl: './standings-table.component.scss',
})
export class StandingsTableComponent {
  state = inject(StandingsStateService);
  protected readonly ALL_GP_ID = ALL_GP_ID;
  protected readonly AGGREGATED_ID = AGGREGATED_ID;
}
