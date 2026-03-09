import { NextTechPage } from './app.po';

describe('NextTech App', () => {
  let page: NextTechPage;

  beforeEach(() => {
    page = new NextTechPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
