import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MindfightsComponent } from './mindfights.component';

describe('MindfightsComponent', () => {
  let component: MindfightsComponent;
  let fixture: ComponentFixture<MindfightsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MindfightsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MindfightsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
