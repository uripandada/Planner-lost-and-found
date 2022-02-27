import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AssetListResolver } from '../core/resolvers/asset-list.resolver';
import { WarehouseDetailsResolver } from '../core/resolvers/warehouse-details.resolver';
import { WarehouseDetailsComponent } from './warehouse-details/warehouse-details.component';

const routes: Routes = [
  //{
  //  path: '',
  //  component: WarehouseDetailsComponent,
  //  resolve: {
  //  },
  //},
  {
    path: 'warehouse',
    component: WarehouseDetailsComponent,
    resolve: {
      warehouse: WarehouseDetailsResolver
    },
    data: {

    }
  },
  {
    path: 'new-central-warehouse/:hotelId',
    component: WarehouseDetailsComponent,
    resolve: {
      warehouse: WarehouseDetailsResolver
    },
    data: {
      isCentralWarehouse: true,
      assets: [],
    }
  },
  {
    path: 'new-warehouse/:hotelId/:floorId',
    component: WarehouseDetailsComponent,
    resolve: {
      warehouse: WarehouseDetailsResolver
    },
    data: {
      isCentralWarehouse: false,
      assets: [],
    }
  },
  {
    path: 'warehouse-details/:warehouseId',
    component: WarehouseDetailsComponent,
    resolve: {
      warehouse: WarehouseDetailsResolver,
      assets: AssetListResolver
    },
    data: {
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WarehousesManagementRoutingModule { }
