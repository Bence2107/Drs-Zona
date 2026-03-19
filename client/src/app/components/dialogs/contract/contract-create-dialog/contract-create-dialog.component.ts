import {Component, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {DriverService} from '../../../../services/driver.service';
import {ConstructorsService} from '../../../../services/constructors.service';
import {ConstructorListDto} from '../../../../api/models/constructor-list-dto';
import {DriverListDto} from '../../../../api/models/driver-list-dto';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {ContractsService} from '../../../../services/contracts.service';
import {ContractCreateDto} from '../../../../api/models/contract-create-dto';

@Component({
  selector: 'app-contract-create-dialog',
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
    MatIcon
  ],
  templateUrl: './contract-create-dialog.component.html',
  styleUrl: './contract-create-dialog.component.scss',
})
export class ContractCreateDialogComponent implements OnInit{
  drivers = signal<DriverListDto[]>([]);
  constructors = signal<ConstructorListDto[]>([]);

  isSubmitting = false;
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ContractCreateDialogComponent>,
    private constructorService: ConstructorsService,
    private contractsService: ContractsService,
    private driverService: DriverService
  )
  {}

  ngOnInit() {
    this.form = this.fb.group({
      driverId: [null],
      teamId:   [null],
    });

    this.driverService.getAllDrivers().subscribe(res => this.drivers.set(res));
    this.constructorService.getAllConstructors().subscribe(res => this.constructors.set(res));
  }

  submit() {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const v = this.form.value;
    const dto: ContractCreateDto = {
      driverId: v.driverId,
      teamId: v.teamId
    }

    this.contractsService.createContract(dto).subscribe({
      next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
      error: () => { this.isSubmitting = false; }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
