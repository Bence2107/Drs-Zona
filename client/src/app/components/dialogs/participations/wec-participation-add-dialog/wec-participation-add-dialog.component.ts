import { Component, inject, OnInit, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormField, MatLabel } from '@angular/material/input';
import { MatOption, MatSelect } from '@angular/material/select';
import { MatButton } from '@angular/material/button';
import { ResultsService } from '../../../../services/results.service';
import { DriverLookUpDto } from '../../../../api/models/driver-look-up-dto';
import { ConstructorLookUpDto } from '../../../../api/models/constructor-look-up-dto';
import { MatInput } from '@angular/material/input';
import {DriverService} from '../../../../services/driver.service';
import {ConstructorsService} from '../../../../services/constructors.service';

@Component({
  selector: 'app-wec-participation-add-dialog',
  standalone: true,
  imports: [
    MatDialogTitle,
    MatDialogContent,
    ReactiveFormsModule,
    MatFormField,
    MatLabel,
    MatSelect,
    MatOption,
    MatDialogActions,
    MatButton,
    MatDialogClose,
    MatInput
  ],
  templateUrl: './wec-participation-add-dialog.component.html',
  styleUrl: './wec-participation-add-dialog.component.scss',
})
export class WecParticipationAddDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private resultsService = inject(ResultsService);
  private driverService = inject(DriverService);
  private constructorsService = inject(ConstructorsService);

  private dialogRef = inject(MatDialogRef<WecParticipationAddDialogComponent>);
  public data = inject(MAT_DIALOG_DATA);

  form: FormGroup;
  // A service-edben lévő metódusokat hívd, amik a teljes listát adják vissza
  constructors = signal<ConstructorLookUpDto[]>([]);
  drivers = signal<DriverLookUpDto[]>([]);

  constructor() {
    this.form = this.fb.group({
      constructorId: ['', Validators.required],
      carNumber: ['', [Validators.required, Validators.min(1)]],
      driverIds: [[], Validators.required]
    });
  }

  ngOnInit() {
    this.constructorsService.getAllConstructors().subscribe(res => this.constructors.set(res));
    this.driverService.getAllDrivers().subscribe(res => this.drivers.set(res));
  }

  save() {
    if (this.form.valid) {
      const payload = {
        driversChampId: this.data.driversChampId,
        constructorsChampId: this.data.constructorsChampId,
        constructorIds: [this.form.value.constructorId],
        drivers: this.form.value.driverIds.map((id: string) => ({
          driverId: id,
          driverNumber: Number(this.form.value.carNumber)
        }))
      };

      this.resultsService.addParticipation(payload).subscribe(() => {
        this.dialogRef.close(true);
      });
    }
  }
}
