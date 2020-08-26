import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { PlaceListComponent } from './components/place-list/place-list.component';
import { PlaceAddComponent } from './components/place-add/place-add.component';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { PlaceEditComponent } from './components/place-edit/place-edit.component';

const routes: Routes = [
  {
    path: '',
    component: PlaceListComponent
  },
  {
    path: 'create',
    component: PlaceAddComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'edit/:id',
    component: PlaceEditComponent,
    canActivate: [AuthorizeGuard]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class PlaceRoutingModule { }
