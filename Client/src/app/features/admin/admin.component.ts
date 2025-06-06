import { CurrencyPipe, DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatLabel, MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../core/services/admin.service';
import { Order } from '../../shared/models/order';
import { OrderParams } from '../../shared/models/orderParams';
import { DialogService } from '../../core/services/dialog.service';

@Component({
  selector: 'app-admin',
  imports: [MatTableModule,
    MatPaginatorModule,
    MatButton,
    MatIcon,
    MatSelectModule,
    DatePipe,
    CurrencyPipe,
    MatLabel,
    MatTooltipModule,
    MatTabsModule,
    RouterLink],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {

  displayedColumns: string[] = ['id', 'buyerEmail', 'orderDate', 'total', 'status', 'action'];  
  dataSource = new MatTableDataSource<Order>([]);

  orderParams = new OrderParams();
  totalCount = 0;
  statusOptions = ['All', 'PaymentReceived', 'PaymentMismatch', 'Refunded', 'Pending'];

  constructor(private adminService :AdminService, private dialogService: DialogService) {}
  
  ngOnInit(): void {
    this.loadOrders();
  }    

  loadOrders() {
    this.adminService.getOrders(this.orderParams).subscribe({
      next: response => {
        if (response.data) {
          this.dataSource.data = response.data;
          this.totalCount = response.totalCount;
        }
      }
    })
  }

  onPageChange(event: PageEvent) {
    this.orderParams.pageIndex = event.pageIndex + 1;
    this.orderParams.pageSize = event.pageSize;
    this.loadOrders();
  }

  onFilterSelect(event: MatSelectChange) {
    this.orderParams.filter = event.value;
    this.orderParams.pageIndex = 1;
    this.loadOrders();
  }

  async openConfirmDialog(id: string) {
    const confirmed = await this.dialogService.confirm(
      'Confirm refund',
      'Are you sure you want to issue this refund? This cannot be undone'
    )

    if (confirmed) {
      this.refundOrder(id);
    }      
  }

  refundOrder(id: string) {
    this.adminService.refundOrder(id).subscribe({
      next: order => {
        this.dataSource.data = this.dataSource.data.map(o => o.id === id ? order : o)
      }
    })
  }

}
