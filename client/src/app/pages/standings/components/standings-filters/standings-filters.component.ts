import {Component, inject} from '@angular/core';
import {StandingsStateService} from '../standings-state.service';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {FormsModule} from '@angular/forms';
import {CountryFlagPipe} from '../../../../pipes/country-flag.pipe';

@Component({
  selector: 'app-results-filters',
  imports: [
    MatButtonToggleGroup,
    MatFormField,
    MatLabel,
    MatSelect,
    FormsModule,
    MatButtonToggle,
    MatOption,
    CountryFlagPipe
  ],
  templateUrl: './standings-filters.component.html',
  styleUrl: './standings-filters.component.scss',
})
export class StandingsFiltersComponent {
  state = inject(StandingsStateService);
}
