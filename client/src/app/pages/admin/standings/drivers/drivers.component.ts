import {Component, OnInit, signal} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {DriverService} from '../../../../services/api/driver.service';
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
import {Router} from '@angular/router';

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
  drivers = signal<DriverListDto[]>([]);
  isLoading = signal(false);

  constructor(
    private dialog: MatDialog,
    private router: Router,
    private driverService: DriverService
  ) {}

  ngOnInit() {
    this.loadDrivers();
  }

  private loadDrivers() {
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

  protected goBack() {
    this.router.navigate(["results"]);
  }
}
