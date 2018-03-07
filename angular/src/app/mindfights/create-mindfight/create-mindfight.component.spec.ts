import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMindfightComponent } from './create-mindfight.component';

describe('CreateMindfightComponent', () => {
  let component: CreateMindfightComponent;
  let fixture: ComponentFixture<CreateMindfightComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateMindfightComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMindfightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
