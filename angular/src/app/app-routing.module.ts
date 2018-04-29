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
import { ToursComponent } from 'app/tours/tours.component';
import { CreateTourComponent } from 'app/tours/create-tour/create-tour.component';
import { TourDetailsComponent } from 'app/tours/tour-details/tour-details.component';
import { EditTourComponent } from 'app/tours/edit-tour/edit-tour.component';
import { QuestionsComponent } from 'app/questions/questions.component';
import { QuestionDetailsComponent } from 'app/questions/question-details/question-details.component';
import { CreateQuestionComponent } from 'app/questions/create-question/create-question.component';
import { EditQuestionComponent } from 'app/questions/edit-question/edit-question.component';
import { TeamComponent } from 'app/team/team.component';
import { CreateTeamComponent } from 'app/team/create-team/create-team.component';
import { EditTeamComponent } from 'app/team/edit-team/edit-team.component';
import { PlayMindfightComponent } from 'app/mindfights/play-mindfight/play-mindfight.component';
import { EvaluateMindfightsComponent } from 'app/mindfights/evaluate-mindfights/evaluate-mindfights.component';
import { EvaluateMindfightDetailsComponent } from 'app/mindfights/evaluate-mindfights/evaluate-mindfight-details/evaluate-mindfight-details.component';
import { EvaluateTeamComponent } from 'app/mindfights/evaluate-mindfights/evaluate-team/evaluate-team.component';
import { EvaluateAnswerComponent } from 'app/mindfights/evaluate-mindfights/evaluate-answer/evaluate-answer.component';
import { MindfightResultComponent } from 'app/mindfights/mindfight-result/mindfight-result.component';
import { TeamAnswersComponent } from 'app/mindfights/mindfight-result/team-answers/team-answers.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/create', component: CreateMindfightComponent },
                    { path: 'mindfights/administrate', component: AdministrateMindfightsComponent },
                    { path: 'mindfights/evaluate', component: EvaluateMindfightsComponent },
                    { path: 'mindfights/evaluate/:mindfightId/details', component: EvaluateMindfightDetailsComponent },
                    { path: 'mindfights/evaluate/:mindfightId/team/:teamId', component: EvaluateTeamComponent },
                    { path: 'mindfights/evaluate/:mindfightId/team/:teamId/question/:questionId', component: EvaluateAnswerComponent },
                    { path: 'mindfights/:mindfightId', component: MindfightDetailsComponent },
                    { path: 'mindfights/:mindfightId/results', component: MindfightResultComponent },
                    { path: 'mindfights/:mindfightId/results/team/:teamId', component: TeamAnswersComponent },
                    { path: 'mindfights/:mindfightId/play', component: PlayMindfightComponent },
                    { path: 'mindfights/:mindfightId/edit', component: EditMindfightComponent },
                    { path: 'mindfights/:mindfightId/edit/tours', component: ToursComponent },
                    { path: 'mindfights/:mindfightId/edit/tours/create', component: CreateTourComponent },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId', component: TourDetailsComponent },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/edit', component: EditTourComponent },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions', component: QuestionsComponent },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions/create', component: CreateQuestionComponent },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions/:questionId', component: QuestionDetailsComponent },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions/:questionId/edit', component: EditQuestionComponent },
                    { path: 'mindfights', component: MindfightsComponent },
                    { path: 'team', component: TeamComponent },
                    { path: 'team/create', component: CreateTeamComponent },
                    { path: 'team/:teamId/edit', component: EditTeamComponent },
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
