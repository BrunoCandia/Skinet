import { Component, OnInit } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { ProductItemComponent } from './product-item/product-item.component';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ShopParams } from '../../shared/models/shopParams';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';
import { EmptyStateComponent } from "../../shared/components/empty-state/empty-state.component";

@Component({
  selector: 'app-shop',
  imports: [ProductItemComponent, MatButton, MatIcon, MatMenu, MatMenuTrigger, MatSelectionList, MatListOption, MatPaginator, FormsModule, EmptyStateComponent],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  products?: Pagination<Product>;
  types: string[] = [];
  brands: string[] = [];
  // selectedTypes: string[] = [];
  // selectedBrands: string[] = [];
  // selectedSort: string = 'name';
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low-High', value: 'priceAsc' },
    { name: 'Price: High-Low', value: 'priceDesc' },
  ];
  shopParams = new ShopParams();
  pageSizeOptions = [5, 10, 15, 20];
  
  constructor(private shopService: ShopService, private dialogService: MatDialog) {}
  
  ngOnInit(): void {
   this.loadShop();
  }

  loadShop() {
    this.getProducts();

    this.shopService.getTypes().subscribe({
      next: response => {this.types = response, console.log(this.types)}
    })

    this.shopService.getBrands().subscribe({
      next: response => {this.brands = response, console.log(this.brands)}
    })
  }

  getProducts() {
    this.shopService.getProducts(this.shopParams).subscribe({
      next: response => {this.products = response},
      error: error => console.error(error)
    })
  }

  openFilersDialog() {
    console.log('Opening dialog with:', {
      brands: this.brands,
      types: this.types,
      selectedBrands: this.shopParams.brands,
      selectedTypes: this.shopParams.types
      // selectedBrands: this.selectedBrands,
      // selectedTypes: this.selectedTypes
    });

    const dialogRef = this.dialogService.open(
      FiltersDialogComponent, 
      {        
        minWidth: '500px',
        data: {
          brands: this.brands,                  
          types: this.types,
          selectedBrands: this.shopParams.brands,
          selectedTypes: this.shopParams.types      
          // selectedBrands: this.selectedBrands,
          // selectedTypes: this.selectedTypes
        }
      });

    dialogRef.afterClosed().subscribe({
      next: result => {
        if (result) {
          console.log(result);
          this.shopParams.brands = result.selectedBrands;
          this.shopParams.types = result.selectedTypes;
          // this.selectedBrands = result.selectedBrands;
          // this.selectedTypes = result.selectedTypes;
          this.shopParams.pageIndex = 1; // Reset to first page
          this.getProducts();
        }
      }
    })
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];

    if (selectedOption) {
      //this.selectedSort = selectedOption.value;
      this.shopParams.sort = selectedOption.value;
      this.shopParams.pageIndex = 1; // Reset to first page
      this.getProducts();
    }
  }

  onSearchChange() {
    this.shopParams.pageIndex = 1; // Reset to first page
    this.getProducts();
  }

  handlePageEvent($event: PageEvent) {
    this.shopParams.pageIndex = $event.pageIndex + 1;
    this.shopParams.pageSize = $event.pageSize;
    this.getProducts()
  }

  resetFilters() {
    this.shopParams = new ShopParams();
    this.getProducts();
  }
}
