import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';

import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AbpModule } from '@abp/abp.module';

import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';

import { HomeComponent } from '@app/home/home.component';
import { AboutComponent } from '@app/about/about.component';
import { UsersComponent } from '@app/users/users.component';
import { CreateUserComponent } from '@app/users/create-user/create-user.component';
import { EditUserComponent } from './users/edit-user/edit-user.component';
import { RolesComponent } from '@app/roles/roles.component';
import { CreateRoleComponent } from '@app/roles/create-role/create-role.component';
import { EditRoleComponent } from './roles/edit-role/edit-role.component';
import { TenantsComponent } from '@app/tenants/tenants.component';
import { CreateTenantComponent } from './tenants/create-tenant/create-tenant.component';
import { EditTenantComponent } from './tenants/edit-tenant/edit-tenant.component';
import { TopBarComponent } from '@app/layout/topbar.component';
import { TopBarLanguageSwitchComponent } from '@app/layout/topbar-languageswitch.component';
import { SideBarUserAreaComponent } from '@app/layout/sidebar-user-area.component';
import { SideBarNavComponent } from '@app/layout/sidebar-nav.component';
import { SideBarFooterComponent } from '@app/layout/sidebar-footer.component';
import { RightSideBarComponent } from '@app/layout/right-sidebar.component';
import { MindfightsComponent } from './mindfights/mindfights.component';
import { CreateMindfightComponent } from './mindfights/create-mindfight/create-mindfight.component';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MindfightDetailsComponent } from './mindfights/mindfight-details/mindfight-details.component';
import { Daterangepicker } from 'ng2-daterangepicker';
import { DatepickerOptionsService } from './services/datepickerOptions.service';
import { AdministrateMindfightsComponent } from './mindfights/administrate-mindfights/administrate-mindfights.component';
import { MindfightsListComponent } from './mindfights/mindfights-list/mindfights-list.component';
import { MindfightCardComponent } from './mindfights/mindfight-card/mindfight-card.component';
import { EditMindfightComponent } from './mindfights/edit-mindfight/edit-mindfight.component';
import { ToursComponent } from './tours/tours.component';
import { CreateTourComponent } from './tours/create-tour/create-tour.component';
import { TourCardComponent } from './tours/tour-card/tour-card.component';
import { TourDetailsComponent } from './tours/tour-details/tour-details.component';
import { EditTourComponent } from './tours/edit-tour/edit-tour.component';
import { QuestionsComponent } from './questions/questions.component';
import { QuestionCardComponent } from './questions/question-card/question-card.component';
import { QuestionDetailsComponent } from './questions/question-details/question-details.component';
import { EditQuestionComponent } from './questions/edit-question/edit-question.component';
import { CreateQuestionComponent } from './questions/create-question/create-question.component';
import { TeamComponent } from './team/team.component';
import { CreateTeamComponent } from './team/create-team/create-team.component';
import { EditTeamComponent } from './team/edit-team/edit-team.component';
import { RegisterComponent } from './mindfights/register/register.component';
import { PlayMindfightComponent } from './mindfights/play-mindfight/play-mindfight.component';
import { MindfightResultComponent } from './mindfights/mindfight-result/mindfight-result.component';
import { EvaluateMindfightsComponent } from 'app/mindfights/evaluate-mindfights/evaluate-mindfights.component';
import { EvaluateMindfightDetailsComponent } from './mindfights/evaluate-mindfights/evaluate-mindfight-details/evaluate-mindfight-details.component';
import { EvaluateTeamComponent } from './mindfights/evaluate-mindfights/evaluate-team/evaluate-team.component';
import { EvaluateAnswerComponent } from './mindfights/evaluate-mindfights/evaluate-answer/evaluate-answer.component';
import { TeamAnswersComponent } from './mindfights/mindfight-result/team-answers/team-answers.component';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        AboutComponent,
        TenantsComponent,
		CreateTenantComponent,
		EditTenantComponent,
        UsersComponent,
		CreateUserComponent,
		EditUserComponent,
      	RolesComponent,        
		CreateRoleComponent,
		EditRoleComponent,
        TopBarComponent,
        TopBarLanguageSwitchComponent,
        SideBarUserAreaComponent,
        SideBarNavComponent,
        SideBarFooterComponent,
        RightSideBarComponent,
        MindfightsComponent,
        CreateMindfightComponent,
        MindfightDetailsComponent,
        AdministrateMindfightsComponent,
        MindfightsListComponent,
        MindfightCardComponent,
        EditMindfightComponent,
        ToursComponent,
        CreateTourComponent,
        TourCardComponent,
        TourDetailsComponent,
        EditTourComponent,
        QuestionsComponent,
        QuestionCardComponent,
        QuestionDetailsComponent,
        EditQuestionComponent,
        CreateQuestionComponent,
        TeamComponent,
        CreateTeamComponent,
        EditTeamComponent,
        RegisterComponent,
        PlayMindfightComponent,
        MindfightResultComponent,
        EvaluateMindfightsComponent,
        EvaluateMindfightDetailsComponent,
        EvaluateTeamComponent,
        EvaluateAnswerComponent,
        TeamAnswersComponent
    
    ],
    imports: [
        MatSelectModule,
        MatInputModule,
        CommonModule,
        FormsModule,
        HttpModule,
        JsonpModule,
        ModalModule.forRoot(),
        AbpModule,
        AppRoutingModule,
        ServiceProxyModule,
        SharedModule,
        NgxPaginationModule,
        MatDatepickerModule,
        Daterangepicker 
    ],
    providers: [
        DatepickerOptionsService
    ]
})
export class AppModule { }
