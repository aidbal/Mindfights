import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/app-component-base';
import { UserServiceProxy, UserDto } from 'shared/service-proxies/service-proxies';

@Component({
  selector: 'change-password-modal',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent extends AppComponentBase  {
    @ViewChild('changePasswordModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;

    @Output() modalSave = new EventEmitter<any>();

    active = false;
    saving = false;
    user: UserDto = null;
    password: string;

    constructor(
        injector: Injector,
        private userService: UserServiceProxy
    ) {
        super(injector);
    }

    show(id: number): void {
        this.userService.get(id)
            .subscribe(
                (result) => {
                    this.user = result;
                    this.active = true;
                    this.modal.show();
                }
            );
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {
        this.saving = true;
        this.userService.changePassword(this.user.id, this.password)
            .finally(() => { this.saving = false; })
            .subscribe(() => {
                this.password = null;
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

}
