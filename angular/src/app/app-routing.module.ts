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
import { PlayerDetailsComponent } from 'app/player/player-details/player-details.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/player', component: PlayerDetailsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/create', component: CreateMindfightComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/administrate', component: AdministrateMindfightsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/evaluate', component: EvaluateMindfightsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/evaluate/:mindfightId/details', component: EvaluateMindfightDetailsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/evaluate/:mindfightId/team/:teamId', component: EvaluateTeamComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/evaluate/:mindfightId/team/:teamId/question/:questionId', component: EvaluateAnswerComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId', component: MindfightDetailsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/results', component: MindfightResultComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/results/team/:teamId', component: TeamAnswersComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/play', component: PlayMindfightComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit', component: EditMindfightComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours', component: ToursComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours/create', component: CreateTourComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId', component: TourDetailsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/edit', component: EditTourComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions', component: QuestionsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions/create', component: CreateQuestionComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions/:questionId', component: QuestionDetailsComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights/:mindfightId/edit/tours/:tourId/questions/:questionId/edit', component: EditQuestionComponent, canActivate: [AppRouteGuard] },
                    { path: 'mindfights', component: MindfightsComponent, canActivate: [AppRouteGuard] },
                    { path: 'team', component: TeamComponent, canActivate: [AppRouteGuard] },
                    { path: 'team/create', component: CreateTeamComponent, canActivate: [AppRouteGuard] },
                    { path: 'team/:teamId/edit', component: EditTeamComponent, canActivate: [AppRouteGuard] },
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
