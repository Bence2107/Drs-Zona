import {Component, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from "@angular/material/dialog";
import { DriverService } from "../../../../services/api/driver.service";
import {ConstructorsService} from '../../../../services/api/constructors.service';
import {ConstructorListDto, ContractListDto, DriverListDto} from "../../../../api/models";
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {ContractsService} from '../../../../services/api/contracts.service';

@Component({
  selector: 'app-contract-edit-dialog',
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
  templateUrl: './contract-edit-dialog.component.html',
  styleUrl: './contract-edit-dialog.component.scss',
})
export class ContractEditDialogComponent implements OnInit{
  drivers = signal<DriverListDto[]>([]);
  constructors = signal<ConstructorListDto[]>([]);

  data = inject(MAT_DIALOG_DATA) as { contract: ContractListDto };
  isSubmitting = false;
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ContractEditDialogComponent>,
    private constructorService: ConstructorsService,
    private contractsService: ContractsService,
    private driverService: DriverService
  ) {
  }

  ngOnInit() {
    const c = this.data.contract;
    this.form = this.fb.group({
      driverId: [c.driverId ?? null],
      teamId:   [c.teamId ?? null],
    });

    this.driverService.getAllDrivers().subscribe(res => this.drivers.set(res));
    this.constructorService.getAllConstructors().subscribe(res => this.constructors.set(res));
  }

  submit() {
    this.isSubmitting = true;
    const v = this.form.value;

    this.contractsService.updateContract(
      this.data.contract.id!,
      v.driverId ?? '00000000-0000-0000-0000-000000000000',
      v.teamId   ?? '00000000-0000-0000-0000-000000000000'
    ).subscribe({
      next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
      error: () => { this.isSubmitting = false; }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
