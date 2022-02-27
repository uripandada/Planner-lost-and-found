import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AssetsManagementComponent } from './assets-management.component';
import { TagListResolver } from '../core/resolvers/tag-list.resolver';
import { AssetDetailsComponent } from './asset-details/asset-details.component';
import { AssetDetailsDataResolver } from '../core/resolvers/asset-details.resolver';
import { SystemDefinedAssetActionListResolver } from '../core/resolvers/system-defined-asset-action-list.resolver';
import { AssetImportPreviewComponent } from './asset-import-preview/asset-import-preview.component';
import { AssetActionsImportPreviewComponent } from './asset-actions-import-preview/asset-actions-import-preview.component';

const routes: Routes = [
  {
    path: '',
    component: AssetsManagementComponent,
    resolve: {
      tags: TagListResolver
    },
  },
  {
    path: 'asset',
    component: AssetDetailsComponent,
    resolve: {
      tags: TagListResolver,
      systemDefinedActions: SystemDefinedAssetActionListResolver,
    },
    data: {
      assetDetails: null,
      isCreateNew: true,
    }
  },
  {
    path: 'asset/:assetGroupId',
    component: AssetDetailsComponent,
    resolve: {
      assetDetails: AssetDetailsDataResolver,
      tags: TagListResolver,
      systemDefinedActions: SystemDefinedAssetActionListResolver,
    },
    data: {
      isCreateNew: false
    }
  },
  {
    path: 'asset/:assetGroupId/:assetId',
    component: AssetDetailsComponent,
    resolve: {
      assetDetails: AssetDetailsDataResolver,
      tags: TagListResolver,
      systemDefinedActions: SystemDefinedAssetActionListResolver,
    },
    data: {
      isCreateNew: false
    }
  },
  {
    path: 'asset-import-preview',
    component: AssetImportPreviewComponent
  },
  {
    path: 'asset-actions-import-preview',
    component: AssetActionsImportPreviewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AssetsManagementRoutingModule { }
