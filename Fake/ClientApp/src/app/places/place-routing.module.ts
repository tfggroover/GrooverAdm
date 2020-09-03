import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { PlaceListComponent } from './components/place-list/place-list.component';
import { PlaceAddComponent } from './components/place-add/place-add.component';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { PlaceEditComponent } from './components/place-edit/place-edit.component';
import { OwnerGuard } from './services/owner.guard';
import { PlaceDetailComponent } from './components/place-detail/place-detail.component';

const routes: Routes = [
  {
    path: '',
    component: PlaceListComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'create',
    component: PlaceAddComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'detail/:id',
    component: PlaceDetailComponent,
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'edit/:id',
    component: PlaceEditComponent,
    canActivate: [AuthorizeGuard, OwnerGuard]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule],
  providers: [OwnerGuard]
})
export class PlaceRoutingModule { }
