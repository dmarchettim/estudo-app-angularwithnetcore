import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member/member-list/member-list.component';
import { MessageComponent } from './message/message.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './member/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './member/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-chaves.guard';


const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'members', component: MemberListComponent, canActivate: [AuthGuard], resolve: { usersResolver: MemberListResolver }  },
  { 
    path: 'members/edit',
    component: MemberEditComponent,
    canActivate: [AuthGuard],
    resolve: { currentUser: MemberEditResolver },
    canDeactivate: [PreventUnsavedChanges]
  },
  { path: 'messages', component: MessageComponent, canActivate: [AuthGuard] },
  { path: 'lists', component: ListsComponent, canActivate: [AuthGuard], resolve: { usersResolver: MemberListResolver } },
  { path: 'members/:id', component: MemberDetailComponent, canActivate: [AuthGuard], resolve: { userResolver: MemberDetailResolver } },
  { path: '**', redirectTo: 'home', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
