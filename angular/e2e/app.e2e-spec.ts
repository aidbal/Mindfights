import { SkautatinklisTemplatePage } from './app.po';

describe('Skautatinklis App', function() {
  let page: SkautatinklisTemplatePage;

  beforeEach(() => {
    page = new SkautatinklisTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
