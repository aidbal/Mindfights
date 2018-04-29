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
    isTeamLeader = false;
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

    tourCountDownSubscriber: any;
    mindfightCountDownSubscriber: any;
    questionCountDownSubscriber: any;
    answersCountDownSubscriber: any;

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

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
        if (this.mindfightCountDownSubscriber) {
            this.mindfightCountDownSubscriber.unsubscribe();
        }
        if (this.tourCountDownSubscriber) {
            this.tourCountDownSubscriber.unsubscribe();
        }
        if (this.questionCountDownSubscriber) {
            this.questionCountDownSubscriber.unsubscribe();
        }
        if (this.answersCountDownSubscriber) {
            this.answersCountDownSubscriber.unsubscribe();
        }
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

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            this.secondsLeftToStartMindfight = this.getSecondsLeftToStart(result);
            if (this.secondsLeftToStartMindfight > 0) {
                this.startMindfightCountdownTimer();
            } else {
                this.notify.error("Protmūšis jau prasidėjo!", "Klaida");
                this.router.navigate(['../'], { relativeTo: this.activatedRoute });
            }
            console.log(result);
            this.getPlayerInfo();
        });
    }

    getPlayerInfo(): void {
        let userId = abp.session.userId;
        this.playerService.getPlayerInfo(userId).subscribe((result) => {
            this.playerInfo = result;
            if (result.teamId !== null) {
                this.getRegistration(result.teamId);
                if (result.isTeamLeader) {
                    this.isTeamLeader = true;
                }
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
        let mindfightCountDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToStartMindfight)
            .map(() => --this.secondsLeftToStartMindfight);

        this.mindfightCountDownSubscriber = mindfightCountDown.subscribe(
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
        let tourCountDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToStartTour)
            .map(() => --this.secondsLeftToStartTour);

        this.tourCountDownSubscriber = tourCountDown.subscribe(
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
        let questionCountDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToStartQuestion)
            .map(() => --this.secondsLeftToStartQuestion);

        this.questionCountDownSubscriber = questionCountDown.subscribe(
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
        let that = this;
        console.log(this.teamAnswers);
        let answersCountDown = Observable.timer(0, 1000)
            .take(this.secondsLeftToEnterAnswers)
            .map(() => --this.secondsLeftToEnterAnswers);

        this.answersCountDownSubscriber = answersCountDown.subscribe(
            (seconds) => {
                that.timeLeftToEnterAnswers = that.getMinutesAndSecondsString(seconds);
                console.log(seconds);
            },
            () => { },
            () => {
                console.log(that.teamAnswers);
                that.teamAnswerService.createTeamAnswer(that.teamAnswers, that.mindfightId).subscribe(
                    () => {
                        that.notify.success("Atsakymai išsaugoti sėkmingai!");
                    }
                );
                if (that.currentQuestionIsLast && that.currentTourIsLast) {
                    // team finished mindfight
                    that.notify.success("Protmūšis sėkmingai pabaigtas!", "Sveikiname");
                    that.router.navigate(['../results'], { relativeTo: that.activatedRoute });
                } else {
                    that.getNextTour(that.mindfightId, that.playerInfo.teamId);
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
