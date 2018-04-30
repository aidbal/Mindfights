import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MindfightStatusLabelComponent } from './mindfight-status-label.component';

describe('MindfightStatusLabelComponent', () => {
  let component: MindfightStatusLabelComponent;
  let fixture: ComponentFixture<MindfightStatusLabelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MindfightStatusLabelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MindfightStatusLabelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
