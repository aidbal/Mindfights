import { Component, OnInit, Injector, Input, EventEmitter, Output } from '@angular/core';
import { MindfightDto, RegistrationDto, RegistrationServiceProxy, PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css']
})
export class RegisterComponent extends AppComponentBase implements OnInit {
    @Output() notifyRegisterChange: EventEmitter<any> = new EventEmitter();
    @Input() mindfight: MindfightDto;
    playerInfo: PlayerDto;
    registration: RegistrationDto = null;
    registrationChangeObject = {
        registrationId: null,
        createEvent: false
    };
    saving = false;
    registrationFull = false;

    constructor(
        injector: Injector,
        private registrationService: RegistrationServiceProxy,
        private playerService: PlayerServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getPlayerInfo(abp.session.userId);
        this.checkRegistrationLimits();
    }

    checkRegistrationLimits() {
        if (this.mindfight.teamsLimit > 0 && this.mindfight.registeredTeamsCount >= this.mindfight.teamsLimit) {
            this.registrationFull = true;
        }
    }

    getRegistration(mindfightId, teamId): void {
        this.registrationService.getMindfightTeamRegistration(mindfightId, teamId).subscribe((result) => {
            if (result.teamId === teamId) {
                this.registration = result;
            }
        });
    }

    getPlayerInfo(userId): void {
        this.playerService.getPlayerInfo(userId).subscribe((result) => {
            this.playerInfo = result;
            if (this.playerInfo.teamId != null) {
                this.getRegistration(this.mindfight.id, this.playerInfo.teamId);
            }
        });
    };

    deleteRegistration(): void {
        abp.message.confirm(
            'Registracija bus atšaukta.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.saving = true;
                    this.registrationService.deleteRegistration(this.mindfight.id, this.playerInfo.teamId).subscribe((result) => {
                        this.notify.success("Registracija sėkmingai atšaukta!", "Atlikta");
                        this.registrationChangeObject.registrationId = this.registration.id;
                        this.registrationChangeObject.createEvent = false;
                        this.notifyRegisterChange.emit(this.registrationChangeObject);
                        this.registration = null;
                        this.saving = false;
                    });
                }
            }
        );
    }

    createRegistration(): void {
        this.saving = true;
        this.registrationService.createRegistration(this.mindfight.id, this.playerInfo.teamId).subscribe((result) => {
            this.getRegistration(this.mindfight.id, this.playerInfo.teamId);
            this.notify.success("Registracija sėkmingai sukurta!", "Atlikta");
            this.registrationChangeObject.registrationId = result;
            this.registrationChangeObject.createEvent = true;
            this.notifyRegisterChange.emit(this.registrationChangeObject);
            this.saving = false;
        });
    }
}
