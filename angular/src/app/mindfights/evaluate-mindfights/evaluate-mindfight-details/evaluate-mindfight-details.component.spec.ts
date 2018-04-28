import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluateMindfightDetailsComponent } from './evaluate-mindfight-details.component';

describe('EvaluateMindfightDetailsComponent', () => {
  let component: EvaluateMindfightDetailsComponent;
  let fixture: ComponentFixture<EvaluateMindfightDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EvaluateMindfightDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EvaluateMindfightDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
