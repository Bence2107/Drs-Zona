import {Component, inject, OnInit, signal} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {DriverService} from '../../../../services/driver.service';
import {DriverListDto} from '../../../../api/models/driver-list-dto';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatIcon} from '@angular/material/icon';
import {MatCard, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle} from '@angular/material/card';
import {MatFabButton, MatIconButton} from '@angular/material/button';
import {MatTooltip} from '@angular/material/tooltip';
import {
  DriverCreateDialogComponent
} from '../../../../components/dialogs/driver/driver-create-dialog/driver-create-dialog.component';
import {
  DriverEditDialogComponent
} from '../../../../components/dialogs/driver/driver-edit-dialog/driver-edit-dialog.component';

@Component({
  selector: 'app-drivers',
  imports: [
    MatProgressSpinner,
    MatIcon,
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardSubtitle,
    MatIconButton,
    MatTooltip,
    MatFabButton,
    MatCardContent
  ],
  templateUrl: './drivers.component.html',
  styleUrl: './drivers.component.scss',
})
export class DriversComponent implements OnInit{
  private dialog = inject(MatDialog);
  private driverService = inject(DriverService);

  drivers = signal<DriverListDto[]>([]);
  isLoading = signal(false);

  ngOnInit() {
    this.loadDrivers();
  }

  loadDrivers() {
    this.isLoading.set(true);
    this.driverService.getAllDrivers().subscribe({
      next: res => {
        this.drivers.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  openCreateDialog() {
    const ref = this.dialog.open(DriverCreateDialogComponent, { width: '560px' });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadDrivers();
    });
  }

  openEditDialog(driver: DriverListDto) {
    this.driverService.getDriverById(driver.id!).subscribe(detail => {
      const ref = this.dialog.open(DriverEditDialogComponent, {
        width: '560px',
        data: { driver: detail }
      });
      ref.afterClosed().subscribe(result => {
        if (result) this.loadDrivers();
      });
    });
  }
}
