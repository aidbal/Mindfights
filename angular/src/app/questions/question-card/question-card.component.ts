import { Component, OnInit, Injector, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { QuestionDto, QuestionServiceProxy } from 'shared/service-proxies/service-proxies';

@Component({
  selector: 'app-question-card',
  templateUrl: './question-card.component.html',
  styleUrls: ['./question-card.component.css']
})
export class QuestionCardComponent extends AppComponentBase implements OnInit {
    @Output() notifyOrderChange: EventEmitter<any> = new EventEmitter();
    @Input() question: QuestionDto;
    @Input() mindfightId: number;
    @Input() tourId: number;
    saving = false;
    orderChangeObject = {
        questionId: null,
        currentOrderNumber: null,
        newOrderNumber: null
    };

    constructor(
        injector: Injector,
        private questionService: QuestionServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.orderChangeObject.questionId = this.question.id;
        this.orderChangeObject.currentOrderNumber = this.question.orderNumber;
    }

    moveOrderDown(): void {
        this.orderChangeObject.currentOrderNumber = this.question.orderNumber;
        var newOrderNumber = this.question.orderNumber + 1;
        this.saving = true;
        this.questionService.updateOrderNumber(this.question.id, newOrderNumber).subscribe(
            () => {
                this.orderChangeObject.newOrderNumber = newOrderNumber;
                abp.message.success("Eilės numeris sėkmingai atnaujintas!", "Atlikta");
                this.notifyOrderChange.emit(this.orderChangeObject);
                this.saving = false;
            });
    }

    moveOrderUp(): void {
        if (this.question.orderNumber - 1 != 0) {
            this.orderChangeObject.currentOrderNumber = this.question.orderNumber;
            var newOrderNumber = this.question.orderNumber - 1;
            this.saving = true;
            this.questionService.updateOrderNumber(this.question.id, newOrderNumber).subscribe(
                () => {
                    this.orderChangeObject.newOrderNumber = newOrderNumber;
                    abp.message.success("Eilės numeris sėkmingai atnaujintas!", "Atlikta");
                    this.notifyOrderChange.emit(this.orderChangeObject);
                    this.saving = false;
                });
        }
    }
}
