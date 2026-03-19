import {Component, inject, OnInit, signal} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatDividerModule} from '@angular/material/divider';
import {YearLookupDto} from '../../../../api/models/year-lookup-dto';
import {DriverListDto} from '../../../../api/models/driver-list-dto';
import {ConstructorListDto} from '../../../../api/models/constructor-list-dto';
import {DriverService} from '../../../../services/api/driver.service';
import {ConstructorsService} from '../../../../services/api/constructors.service';
import {MatTooltip} from '@angular/material/tooltip';
import {ChampionshipService} from '../../../../services/api/championship.service';

@Component({
  selector: 'app-participation-add-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatTooltip
  ],
  templateUrl: './participation-add-dialog.component.html',
  styleUrl: './participation-add-dialog.component.scss',
})
export class ParticipationAddDialogComponent implements OnInit {
  data = inject(MAT_DIALOG_DATA) as { lookup: YearLookupDto };
  allDrivers = signal<DriverListDto[]>([]);
  allConstructors = signal<ConstructorListDto[]>([]);
  isSubmitting = false;
  form!: FormGroup;

  constructor(
    private dialogRef: MatDialogRef<ParticipationAddDialogComponent>,
    private fb: FormBuilder,
    private constructorService: ConstructorsService,
    private championshipService: ChampionshipService,
    private driverService: DriverService,
  ) {}


  get driverRows(): FormArray {
    return this.form.get('drivers') as FormArray;
  }

  get constructorRows(): FormArray {
    return this.form.get('constructors') as FormArray;
  }

  ngOnInit() {
    this.form = this.fb.group({
      drivers: this.fb.array([]),
      constructors: this.fb.array([])
    });

    this.driverService.getAllDrivers().subscribe(res => this.allDrivers.set(res));
    this.constructorService.getAllConstructors().subscribe(res => this.allConstructors.set(res));
  }

  addDriverRow() {
    this.driverRows.push(this.fb.group({
      driverId:     ['', Validators.required],
      driverNumber: [null, [Validators.required, Validators.min(1), Validators.max(99)]]
    }));
  }

  addConstructorRow() {
    this.constructorRows.push(this.fb.group({
      constructorId: ['', Validators.required]
    }));
  }

  removeDriverRow(index: number) {
    this.driverRows.removeAt(index);
  }

  removeConstructorRow(index: number) {
    this.constructorRows.removeAt(index);
  }

  submit() {
    if (this.form.invalid) return;
    if (this.driverRows.length === 0 && this.constructorRows.length === 0) return;
    this.isSubmitting = true;

    const dto = {
      driversChampId: this.data.lookup.driversChampId,
      constructorsChampId: this.data.lookup.constructorsChampId,
      drivers: this.driverRows.value.map((r: any) => ({
        driverId: r.driverId,
        driverNumber: r.driverNumber
      })),
      constructorIds: this.constructorRows.value.map((r: any) => r.constructorId)
    };

    this.championshipService.addParticipation(dto).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.dialogRef.close(true);
      },
      error: () => { this.isSubmitting = false; }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
