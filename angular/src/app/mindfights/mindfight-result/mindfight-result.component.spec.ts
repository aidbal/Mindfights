import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MindfightResultComponent } from './mindfight-result.component';

describe('MindfightResultComponent', () => {
  let component: MindfightResultComponent;
  let fixture: ComponentFixture<MindfightResultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MindfightResultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MindfightResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
