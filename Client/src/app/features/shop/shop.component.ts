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

@Component({
  selector: 'app-shop',
  imports: [ProductItemComponent, MatButton, MatIcon, MatMenu, MatMenuTrigger, MatSelectionList, MatListOption],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {

  products: Product[] = [];
  types: string[] = [];
  brands: string[] = [];
  selectedTypes: string[] = [];
  selectedBrands: string[] = [];
  selectedSort: string = 'name';
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low-High', value: 'priceAsc' },
    { name: 'Price: High-Low', value: 'priceDesc' },
  ];
  
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
    this.shopService.getProducts(this.selectedBrands, this.selectedTypes, this.selectedSort).subscribe({
      next: response => {this.products = response.data},
      error: error => console.error(error)
    })
  }

  openFilersDialog() {
    console.log('Opening dialog with:', {
      brands: this.brands,
      types: this.types,
      selectedBrands: this.selectedBrands,
      selectedTypes: this.selectedTypes
    });

    const dialogRef = this.dialogService.open(
      FiltersDialogComponent, 
      {        
        minWidth: '500px',
        data: {
          brands: this.brands,                  
          types: this.types,                    
          selectedBrands: this.selectedBrands,
          selectedTypes: this.selectedTypes
        }
      });

    dialogRef.afterClosed().subscribe({
      next: result => {
        if (result) {
          console.log(result);
          this.selectedBrands = result.selectedBrands;
          this.selectedTypes = result.selectedTypes;

          this.getProducts();
        }
      }
    })
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];

    if (selectedOption) {
      this.selectedSort = selectedOption.value;      
      this.getProducts();
    }
  }
}
