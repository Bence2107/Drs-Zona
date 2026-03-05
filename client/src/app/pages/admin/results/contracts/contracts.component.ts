import {Component, inject, OnInit, signal} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {ContractListDto} from '../../../../api/models/contract-list-dto';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatIcon} from '@angular/material/icon';
import {MatCard} from '@angular/material/card';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable
} from '@angular/material/table';
import {MatFabButton, MatIconButton} from '@angular/material/button';
import {MatTooltip} from '@angular/material/tooltip';
import {
  ContractCreateDialogComponent
} from '../../../../components/dialogs/contract/contract-create-dialog/contract-create-dialog.component';
import {
  ContractEditDialogComponent
} from '../../../../components/dialogs/contract/contract-edit-dialog/contract-edit-dialog.component';
import {ConfirmDialogComponent} from '../../../../components/dialogs/confirmdialog/confirmdialog.component';
import {ContractsService} from '../../../../services/contracts.service';

@Component({
  selector: 'app-contracts',
  imports: [
    MatProgressSpinner,
    MatIcon,
    MatCard,
    MatTable,
    MatHeaderCell,
    MatCell,
    MatColumnDef,
    MatCellDef,
    MatHeaderCellDef,
    MatIconButton,
    MatTooltip,
    MatHeaderRow,
    MatRow,
    MatFabButton,
    MatRowDef,
    MatHeaderRowDef
  ],
  templateUrl: './contracts.component.html',
  styleUrl: './contracts.component.scss',
})
export class ContractsComponent implements OnInit{
  private dialog = inject(MatDialog);
  private contractsService = inject(ContractsService);

  contracts = signal<ContractListDto[]>([]);
  isLoading = signal(false);

  columns = ['driver', 'team', 'actions'];

  ngOnInit() {
    this.loadContracts();
  }

  loadContracts() {
    this.isLoading.set(true);
    this.contractsService.getAllContracts().subscribe({
      next: res => {
        this.contracts.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  openCreateDialog() {
    const ref = this.dialog.open(ContractCreateDialogComponent, { width: '480px' });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadContracts();
    });
  }

  openEditDialog(contract: ContractListDto) {
    const ref = this.dialog.open(ContractEditDialogComponent, {
      width: '480px',
      data: { contract }
    });
    ref.afterClosed().subscribe(result => {
      if (result) this.loadContracts();
    });
  }

  deleteContract(contract: ContractListDto) {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Szerződés törlése',
        message: `Biztosan törlöd a(z) ${contract.driverName} — ${contract.teamName} szerződést?`,
        confirmText: 'Törlés'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.contractsService.deleteContract(contract.id!).subscribe({
        next: () => this.loadContracts()
      });
    });
  }
}
