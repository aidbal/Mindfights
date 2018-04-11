import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MindfightCardComponent } from './mindfight-card.component';

describe('MindfightCardComponent', () => {
  let component: MindfightCardComponent;
  let fixture: ComponentFixture<MindfightCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MindfightCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MindfightCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
