import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { DragToSelectModule } from 'ngx-drag-to-select';
import { SharedModule } from '../shared/shared.module';
import { WarehouseAssetGroupsComponent } from './warehouse-asset-groups/warehouse-asset-groups.component';
import { WarehouseDetailsComponent } from './warehouse-details/warehouse-details.component';
import { WarehouseEditFormComponent } from './warehouse-edit-form/warehouse-edit-form.component';
import { WarehouseHistoryComponent } from './warehouse-history/warehouse-history.component';
import { WarehouseInventoryArchiveComponent } from './warehouse-inventory-archive/warehouse-inventory-archive.component';
import { WarehousesManagementRoutingModule } from './warehouses-management-routing.module';


@NgModule({
  imports: [
    SharedModule,
    WarehousesManagementRoutingModule,
    ConfirmationPopoverModule,
    DragToSelectModule
  ],
  declarations: [
    WarehouseDetailsComponent,
    WarehouseEditFormComponent,
    WarehouseAssetGroupsComponent,
    WarehouseHistoryComponent,
    WarehouseInventoryArchiveComponent,
  ]
})
export class WarehousesManagementModule { }
