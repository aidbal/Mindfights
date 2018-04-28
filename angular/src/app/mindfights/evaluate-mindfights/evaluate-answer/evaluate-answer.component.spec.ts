import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluateAnswerComponent } from './evaluate-answer.component';

describe('EvaluateAnswerComponent', () => {
  let component: EvaluateAnswerComponent;
  let fixture: ComponentFixture<EvaluateAnswerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EvaluateAnswerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EvaluateAnswerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
