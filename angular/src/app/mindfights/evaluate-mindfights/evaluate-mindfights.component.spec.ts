import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { EvaluateMindfightsComponent } from './evaluate-mindfights.component';

describe('EvaluateMindfightsComponent', () => {
    let component: EvaluateMindfightsComponent;
    let fixture: ComponentFixture<EvaluateMindfightsComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [EvaluateMindfightsComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(EvaluateMindfightsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
