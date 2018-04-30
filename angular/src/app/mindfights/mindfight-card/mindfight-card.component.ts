import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MindfightDto, PlayerDto } from 'shared/service-proxies/service-proxies';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-mindfight-card',
    templateUrl: './mindfight-card.component.html',
    styleUrls: ['./mindfight-card.component.css'],
    animations: [appModuleAnimation()]
})
export class MindfightCardComponent extends AppComponentBase implements OnInit {
    @Input() mindfight: MindfightDto;
    @Input() playerInfo: PlayerDto;
    canEdit = false;
    canEvaluate = false;

    constructor(injector: Injector) {
        super(injector);
    }

    ngOnInit() {
        this.canEditMindfight();
        this.canEvaluateMindfight();
    }

    onRegisterChange(registrationChangeObject): void {
        if (registrationChangeObject.createEvent) {
            this.mindfight.registeredTeamsCount += 1;
        } else {
            this.mindfight.registeredTeamsCount -= 1;
        }
    }

    isMindfightCreator(): boolean {
        return abp.session.userId === this.mindfight.creatorId;
    }

    canEditMindfight() {
        if (this.isMindfightCreator() || abp.auth.isGranted("ManageMindfights")) {
            this.canEdit = true;
        }
    }

    canEvaluateMindfight() {
        var that = this;
        if (this.canEditMindfight()) {
            this.canEvaluate = true;
        } else {
            if (this.mindfight.usersAllowedToEvaluate) {
                this.mindfight.usersAllowedToEvaluate.forEach((userEmail) => {
                    if (userEmail.toUpperCase() === that.playerInfo.emailAddress.toUpperCase()) {
                        that.canEvaluate = true;
                        return false;
                    }
                });
            }
        }
    }
}
