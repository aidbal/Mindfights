import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { RolesComponent } from "app/roles/roles.component";
import { MindfightsComponent } from 'app/mindfights/mindfights.component';
import { CreateMindfightComponent } from 'app/mindfights/create-mindfight/create-mindfight.component';
import { EditMindfightComponent } from 'app/mindfights/edit-mindfight/edit-mindfight.component';
import { MindfightDetailsComponent } from 'app/mindfights/mindfight-details/mindfight-details.component';
import { AdministrateMindfightsComponent } from 'app/mindfights/administrate-mindfights/administrate-mindfights.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/create', component: CreateMindfightComponent },
                    { path: 'mindfights/edit/:mindfightId', component: EditMindfightComponent },
                    { path: 'mindfights/administrate', component: AdministrateMindfightsComponent },
                    { path: 'mindfights/:mindfightId', component: MindfightDetailsComponent },
                    { path: 'mindfights', component: MindfightsComponent },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Tenants' }, canActivate: [AppRouteGuard] },
                    { path: 'about', component: AboutComponent }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
