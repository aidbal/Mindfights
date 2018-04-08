import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MindfightDetailsComponent } from './mindfight-details.component';

describe('MindfightDetailsComponent', () => {
  let component: MindfightDetailsComponent;
  let fixture: ComponentFixture<MindfightDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MindfightDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MindfightDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
