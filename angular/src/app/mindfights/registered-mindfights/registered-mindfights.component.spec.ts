import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisteredMindfightsComponent } from './registered-mindfights.component';

describe('RegisteredMindfightsComponent', () => {
  let component: RegisteredMindfightsComponent;
  let fixture: ComponentFixture<RegisteredMindfightsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RegisteredMindfightsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisteredMindfightsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
