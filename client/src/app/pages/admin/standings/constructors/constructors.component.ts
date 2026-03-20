import {Component, OnInit, signal} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {ConstructorsService} from '../../../../services/api/constructors.service';
import {ConstructorListDto} from '../../../../api/models/constructor-list-dto';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatIcon} from '@angular/material/icon';
import {MatCard, MatCardHeader, MatCardTitle} from '@angular/material/card';
import {MatFabButton, MatIconButton} from '@angular/material/button';
import {MatTooltip} from '@angular/material/tooltip';
import {
  ConstructorCreateDialogComponent
} from '../../../../components/dialogs/constructor/constructor-create-dialog/constructor-create-dialog.component';
import {Router} from '@angular/router';
import {
  ConstructorEditDialogComponent
} from '../../../../components/dialogs/constructor/constructor-edit-dialog/constructor-edit-dialog.component';

@Component({
  selector: 'app-constructors',
  imports: [
    MatProgressSpinner,
    MatIcon,
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatIconButton,
    MatTooltip,
    MatFabButton
  ],
  templateUrl: './constructors.component.html',
  styleUrl: './constructors.component.scss',
})
export class ConstructorsComponent implements OnInit{
  constructors = signal<ConstructorListDto[]>([]);
  isLoading = signal(false);

  constructor(
    private dialog: MatDialog,
    private constructorService: ConstructorsService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadConstructors();
  }

  loadConstructors() {
    this.isLoading.set(true);
    this.constructorService.getAllConstructors().subscribe({
      next: res => {
        this.constructors.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  openCreateDialog() {
    const ref = this.dialog.open(ConstructorCreateDialogComponent, { width: '600px' });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadConstructors();
    });
  }

  openEditDialog(constructor: ConstructorListDto) {
    this.constructorService.getConstructorById(constructor.id!).subscribe(detail => {
      const ref = this.dialog.open(ConstructorEditDialogComponent, {
        width: '600px',
        data: { constructor: detail }
      });
      ref.afterClosed().subscribe(result => {
        if (result) this.loadConstructors();
      });
    });
  }

  protected goBack() {
    this.router.navigate(["results"]);
  }
}
