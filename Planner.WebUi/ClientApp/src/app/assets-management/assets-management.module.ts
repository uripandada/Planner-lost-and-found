import { NgModule } from '@angular/core';
import { ConfirmationPopoverModule } from 'angular-confirmation-popover';
import { SharedModule } from '../shared/shared.module';
import { AssetsManagementRoutingModule } from './assets-management-routing.module';
import { AssetsManagementComponent } from './assets-management.component';
import { AssetEditFormComponent } from './asset-edit-form/asset-edit-form.component';
import { AssetDetailsComponent } from './asset-details/asset-details.component';
import { AssetModelComponent } from './asset-model/asset-model.component';
import { RoomAssignmentsComponent } from './room-assignments/room-assignments.component';
import { AssetActionComponent } from './asset-action/asset-action.component';
import { AssetActionItemComponent } from './asset-action-item/asset-action-item.component';
import { AssetModelEditFormComponent } from './asset-model-edit-form/asset-model-edit-form.component';
import { AssetGroupComponent } from './asset-group/asset-group.component';
import { AssetSelectListComponent } from './asset-select-list/asset-select-list.component';
import { AssetGroupsAvailabilityComponent } from './asset-groups-availability/asset-groups-availability.component';
import { AssetImportPreviewComponent } from './asset-import-preview/asset-import-preview.component';
import { AssetActionsImportPreviewComponent } from './asset-actions-import-preview/asset-actions-import-preview.component';


@NgModule({
  imports: [
    SharedModule,
    AssetsManagementRoutingModule,
    ConfirmationPopoverModule,
  ],
  declarations: [
    AssetsManagementComponent,
    AssetEditFormComponent,
    AssetDetailsComponent,
    AssetModelComponent,
    RoomAssignmentsComponent,
    AssetActionComponent,
    AssetActionItemComponent,
    AssetModelEditFormComponent,
    AssetGroupComponent,
    AssetSelectListComponent,
    AssetGroupsAvailabilityComponent,
    AssetImportPreviewComponent,
    AssetActionsImportPreviewComponent,
  ]
})
export class AssetsManagementModule { }
