import { MindfightsTemplatePage } from './app.po';

describe('Mindfights App', function() {
  let page: MindfightsTemplatePage;

  beforeEach(() => {
    page = new MindfightsTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
