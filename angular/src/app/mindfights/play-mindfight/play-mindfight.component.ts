import { Component, OnInit, Injector } from '@angular/core';
import {
    MindfightDto, MindfightServiceProxy, PlayerServiceProxy,
    PlayerDto, TourDto, TourServiceProxy, RegistrationServiceProxy, RegistrationDto,
    QuestionServiceProxy, QuestionDto, TeamAnswerDto, TeamAnswerServiceProxy
} from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/take';
import "rxjs/add/observable/of";
import * as moment from 'moment';

@Component({
    selector: 'app-play-mindfight',
    templateUrl: './play-mindfight.component.html',
    styleUrls: ['./play-mindfight.component.css']
})
export class PlayMindfightComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfight: MindfightDto;
    mindfightId: number;
    currentTour: TourDto;
    currentQuestion: QuestionDto;
    playerInfo: PlayerDto;
    registration: RegistrationDto;
    teamAnswers: TeamAnswerDto[] = [];

    secondsLeftToStartMindfight: number;
    timeLeftToStartMindfight: any;
    secondsLeftToStartTour: number;
    timeLeftToStartTour: any;
    secondsLeftToStartQuestion: number;
    timeLeftToStartQuestion: any;
    secondsLeftToEnterAnswers: number;
    timeLeftToEnterAnswers: any;

    showTourLabel = false;
    showQuestionLabel = false;
    showAnswersLabel = false;
    currentTourIsLast = false;
    currentQuestionIsLast = false;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private playerService: PlayerServiceProxy,
        private tourService: TourServiceProxy,
        private registrationService: RegistrationServiceProxy,
        private questionService: QuestionServiceProxy,
        private teamAnswerService: TeamAnswerServiceProxy,
        private activatedRoute: ActivatedRoute,
        private location: Location,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.activatedRoute.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
            this.getMindfight(this.mindfightId);
        });
        this.timeLeftToStartMindfight = "00:00";
    }

    getNextTour(mindfightId, teamId): void {
        this.tourService.getNextTour(mindfightId, teamId).subscribe(
            (result) => {
                this.currentTour = result;
                console.log(result);
                this.showTourLabel = true;
                this.showQuestionLabel = false;
                this.showAnswersLabel = false;
                this.secondsLeftToStartTour = result.introTimeInSeconds;
                this.secondsLeftToEnterAnswers = result.timeToEnterAnswersInSeconds;
                this.startTourCountdownTimer();
                this.teamAnswers = [];
                if (result.isLastTour) {
                    this.currentTourIsLast = true;
                }
            }
        );
    }

    getNextQuestion(mindfightId, teamId): void {
        this.questionService.getNextQuestion(mindfightId, teamId).subscribe(
            (result) => {
                this.currentQuestion = result;
                console.log(result);
                this.showTourLabel = false;
                this.showQuestionLabel = true;
                this.showAnswersLabel = false;

                let teamAnswer = new TeamAnswerDto();
                teamAnswer.questionId = result.id;
                teamAnswer.questionTitle = result.title;
                this.teamAnswers.push(teamAnswer);

                this.secondsLeftToStartQuestion = result.timeToAnswerInSeconds;
                this.startQuestionCountdownTimer();
                if (result.isLastQuestion) {
                    this.currentQuestionIsLast = true;
                }
            }
        );
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            this.secondsLeftToStartMindfight = this.getSecondsLeftToStart(result);
            if (this.secondsLeftToStartMindfight > 0) {
                this.startMindfightCountdownTimer();
            } else {
                //Get next tour
            }
            console.log(result);
            this.getPlayerInfo();
        });
    }

    getPlayerInfo(): void {
        let userId = abp.session.userId;
        this.playerService.getPlayerInfo(userId).subscribe((result) => {
            this.playerInfo = result;
            if (result.teamId !== null && this.secondsLeftToStartMindfight <= 0) {
                this.getRegistration(result.teamId);
            } else {
                console.log("No team - redirecting");
            }
            console.log(result);
        });
    };

    getRegistration(teamId): void {
        let mindfightId = this.mindfightId;
        this.registrationService.getMindfightTeamRegistration(mindfightId, teamId).subscribe((result) => {
            if (result.teamId === teamId) {
                this.registration = result;
                if (!result.isConfirmed) {
                    console.log("Registration not confirmed - redirect");
                }
            } else {
                console.log("No registration - redirect");
            }
        });
    }

    startMindfightCountdownTimer(): void {
        let countDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToStartMindfight)
            .map(() => --this.secondsLeftToStartMindfight);

        countDown.subscribe(
            (seconds) => {
                this.timeLeftToStartMindfight = this.getMinutesAndSecondsString(seconds);
                console.log(seconds);
            },
            () => { },
            () => {
                this.getNextTour(this.mindfightId, this.playerInfo.teamId);
                console.log("completed");
            }
        );
    }

    startTourCountdownTimer(): void {
        let countDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToStartTour)
            .map(() => --this.secondsLeftToStartTour);

        countDown.subscribe(
            (seconds) => {
                this.timeLeftToStartTour = this.getMinutesAndSecondsString(seconds);
                console.log(seconds);
            },
            () => { },
            () => {
                this.getNextQuestion(this.mindfightId, this.playerInfo.teamId);
                console.log("completed");
            }
        );
    }

    startQuestionCountdownTimer(): void {
        let countDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToStartQuestion)
            .map(() => --this.secondsLeftToStartQuestion);

        countDown.subscribe(
            (seconds) => {
                this.timeLeftToStartQuestion = this.getMinutesAndSecondsString(seconds);
                console.log(seconds);
            },
            () => { },
            () => {
                if (this.currentQuestionIsLast) {
                    console.log("team finished tour");
                    this.startAnswersCountdownTimer();
                    this.showAnswersLabel = true;
                    this.showQuestionLabel = false;
                    this.showTourLabel = false;
                } else {
                    this.getNextQuestion(this.mindfightId, this.playerInfo.teamId);
                }
                console.log("completed");
            }
        );
    }

    startAnswersCountdownTimer(): void {
        console.log(this.teamAnswers);
        let countDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToEnterAnswers)
            .map(() => --this.secondsLeftToEnterAnswers);

        countDown.subscribe(
            (seconds) => {
                this.timeLeftToEnterAnswers = this.getMinutesAndSecondsString(seconds);
                console.log(seconds);
            },
            () => { },
            () => {
                console.log(this.teamAnswers);
                this.teamAnswerService.createTeamAnswer(this.teamAnswers, this.mindfightId).subscribe(
                    (result) => {
                        this.notify.success("Atsakymai išsaugoti sėkmingai!");
                    }
                );
                if (this.currentQuestionIsLast && this.currentTourIsLast) {
                    // team finished mindfight
                    console.log("team finished mindfight");
                } else {
                    this.getNextTour(this.mindfightId, this.playerInfo.teamId);
                }
                console.log("completed");
            }
        );
    }

    getMinutesAndSecondsString(seconds): string {
        return (seconds - (seconds %= 60)) / 60 + (9 < seconds ? ':' : ':0') + seconds;
    }

    getSecondsLeftToStart(mindfight): number {
        let startTime = moment(mindfight.startTime);
        if (mindfight.prepareTime && mindfight.prepareTime > 0) {
            startTime.add(mindfight.prepareTime, 'minutes');
        }
        let currentTime = moment(abp.clock.now());
        let secondsToStart = startTime.diff(currentTime, 'seconds');
        return secondsToStart;
    }

    goBack() {
        this.location.back();
    }
}
