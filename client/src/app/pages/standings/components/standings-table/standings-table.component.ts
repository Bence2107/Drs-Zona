import {Component, computed, inject} from '@angular/core';
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

  /** Rows of the table that is currently shown, based on the selected view and filters. */
  readonly currentRows = computed<readonly unknown[]>(() => {
    switch (this.state.viewMode()) {
      case 'results':
        return this.state.selectedGrandPrixId() === ALL_GP_ID ? this.state.seasonOverview() : this.state.raceResults();
      case 'drivers':
        return this.state.selectedDriverId() === AGGREGATED_ID ? this.state.driverStandings() : this.state.driverSeasonResults();
      case 'constructors':
        return this.state.selectedConstructorId() === AGGREGATED_ID ? this.state.constructorStandings() : this.state.constructorSeasonResults();
    }
  });
}
