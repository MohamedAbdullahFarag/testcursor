import { Page, Locator } from '@playwright/test';

export abstract class BasePage {
  protected page: Page;
  protected baseUrl: string;

  constructor(page: Page, baseUrl: string) {
    this.page = page;
    this.baseUrl = baseUrl;
  }

  /**
   * Navigate to a specific path relative to base URL
   */
  async navigate(path: string): Promise<void> {
    await this.page.goto(`${this.baseUrl}${path}`);
  }

  /**
   * Wait for page to fully load
   */
  async waitForPageLoad(): Promise<void> {
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Wait for specific element to be visible
   */
  async waitForElement(locator: string | Locator): Promise<void> {
    const element = typeof locator === 'string' ? this.page.locator(locator) : locator;
    await element.waitFor({ state: 'visible' });
  }

  /**
   * Get page title
   */
  async getPageTitle(): Promise<string> {
    return await this.page.title();
  }

  /**
   * Get current URL
   */
  async getCurrentUrl(): Promise<string> {
    return this.page.url();
  }

  /**
   * Take screenshot for debugging
   */
  async takeScreenshot(name: string): Promise<void> {
    await this.page.screenshot({ path: `screenshots/${name}-${Date.now()}.png` });
  }

  /**
   * Wait for navigation to complete
   */
  async waitForNavigation(): Promise<void> {
    await this.page.waitForURL('**');
  }

  /**
   * Check if element exists on page
   */
  async elementExists(locator: string): Promise<boolean> {
    const element = this.page.locator(locator);
    return await element.count() > 0;
  }

  /**
   * Get text content of element
   */
  async getText(locator: string): Promise<string> {
    const element = this.page.locator(locator);
    return await element.textContent() || '';
  }

  /**
   * Check if element is visible
   */
  async isVisible(locator: string): Promise<boolean> {
    const element = this.page.locator(locator);
    return await element.isVisible();
  }

  /**
   * Wait for element to disappear
   */
  async waitForElementToDisappear(locator: string): Promise<void> {
    const element = this.page.locator(locator);
    await element.waitFor({ state: 'hidden' });
  }
}
