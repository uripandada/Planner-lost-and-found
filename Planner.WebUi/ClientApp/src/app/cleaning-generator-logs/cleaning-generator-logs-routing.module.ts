import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CleaningGeneratorLogsComponent } from './cleaning-generator-logs.component';

const routes: Routes = [
  {
    path: '',
    component: CleaningGeneratorLogsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CleaningGeneratorLogsRoutingModule { }
