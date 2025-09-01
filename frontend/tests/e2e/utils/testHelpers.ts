import { Page, expect } from '@playwright/test';
import { testUsers, testCredentials } from '../fixtures/testData';

/**
 * Test Helper Utilities for Authentication Tests
 * Provides common functions for test setup, teardown, and assertions
 */

export class TestHelpers {
  private page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  /**
   * Wait for authentication to complete
   */
  async waitForAuthentication(): Promise<void> {
    // Wait for any loading states to complete
    await this.page.waitForLoadState('networkidle');
    
    // Wait for potential redirects
    await this.page.waitForTimeout(2000);
    
    // Wait for URL to change (indicating redirect)
    await this.page.waitForURL('**/dashboard**', { timeout: 10000 });
  }

  /**
   * Clear browser storage (localStorage, sessionStorage, cookies)
   */
  async clearBrowserStorage(): Promise<void> {
    try {
      await this.page.evaluate(() => {
        try {
          if (typeof localStorage !== 'undefined') {
            localStorage.clear();
          }
          if (typeof sessionStorage !== 'undefined') {
            sessionStorage.clear();
          }
        } catch (e) {
          // Ignore storage access errors
          console.warn('Storage access blocked:', e);
        }
      });
    } catch (error) {
      // Ignore storage clearing errors
      console.warn('Failed to clear browser storage:', error);
    }
    
    await this.page.context().clearCookies();
  }

  /**
   * Set authentication token in localStorage
   */
  async setAuthToken(token: string): Promise<void> {
    try {
      await this.page.evaluate((authToken) => {
        try {
          if (typeof localStorage !== 'undefined') {
            localStorage.setItem('accessToken', authToken);
          }
        } catch (e) {
          console.warn('Failed to set auth token:', e);
        }
      }, token);
    } catch (error) {
      console.warn('Failed to set auth token:', error);
    }
  }

  /**
   * Get authentication token from localStorage
   */
  async getAuthToken(): Promise<string | null> {
    try {
      return await this.page.evaluate(() => {
        try {
          if (typeof localStorage !== 'undefined') {
            return localStorage.getItem('accessToken');
          }
          return null;
        } catch (e) {
          console.warn('Failed to get auth token:', e);
          return null;
        }
      });
    } catch (error) {
      console.warn('Failed to get auth token:', error);
      return null;
    }
  }

  /**
   * Check if user is authenticated
   */
  async isAuthenticated(): Promise<boolean> {
    const token = await this.getAuthToken();
    return token !== null && token !== '';
  }

  /**
   * Simulate network error by setting offline mode
   */
  async simulateNetworkError(): Promise<void> {
    await this.page.route('**/*', route => {
      route.abort('failed');
    });
  }

  /**
   * Restore network connectivity
   */
  async restoreNetwork(): Promise<void> {
    await this.page.unroute('**/*');
  }

  /**
   * Simulate slow network
   */
  async simulateSlowNetwork(delay: number = 2000): Promise<void> {
    await this.page.route('**/*', route => {
      setTimeout(() => route.continue(), delay);
    });
  }

  /**
   * Take screenshot with timestamp
   */
  async takeTimestampedScreenshot(name: string): Promise<void> {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    await this.page.screenshot({ 
      path: `screenshots/${name}-${timestamp}.png`,
      fullPage: true 
    });
  }

  /**
   * Wait for specific network request to complete
   */
  async waitForNetworkRequest(url: string, method: string = 'POST'): Promise<void> {
    await this.page.waitForResponse(
      response => response.url().includes(url) && response.request().method() === method
    );
  }

  /**
   * Check if element has specific CSS class
   */
  async hasClass(locator: string, className: string): Promise<boolean> {
    const element = this.page.locator(locator);
    const classes = await element.getAttribute('class');
    return classes?.includes(className) || false;
  }

  /**
   * Check if element has specific attribute value
   */
  async hasAttribute(locator: string, attribute: string, value: string): Promise<boolean> {
    const element = this.page.locator(locator);
    const attrValue = await element.getAttribute(attribute);
    return attrValue === value;
  }

  /**
   * Wait for element to have specific text
   */
  async waitForElementText(locator: string, text: string, timeout: number = 5000): Promise<void> {
    const element = this.page.locator(locator);
    await element.waitFor({ state: 'visible', timeout });
    
    await expect(element).toHaveText(text, { timeout });
  }

  /**
   * Wait for element to contain specific text
   */
  async waitForElementContainsText(locator: string, text: string, timeout: number = 5000): Promise<void> {
    const element = this.page.locator(locator);
    await element.waitFor({ state: 'visible', timeout });
    
    await expect(element).toContainText(text, { timeout });
  }

  /**
   * Check if page has specific meta tag
   */
  async hasMetaTag(name: string, content: string): Promise<boolean> {
    const metaTag = this.page.locator(`meta[name="${name}"]`);
    if (await metaTag.count() === 0) return false;
    
    const metaContent = await metaTag.getAttribute('content');
    return metaContent === content;
  }

  /**
   * Check if page has specific title
   */
  async hasPageTitle(title: string): Promise<boolean> {
    const pageTitle = await this.page.title();
    return pageTitle.includes(title);
  }

  /**
   * Check if URL contains specific path
   */
  async urlContains(path: string): Promise<boolean> {
    const currentUrl = this.page.url();
    return currentUrl.includes(path);
  }

  /**
   * Wait for URL to change to specific path
   */
  async waitForUrlChange(path: string, timeout: number = 5000): Promise<void> {
    await this.page.waitForURL(url => url.includes(path), { timeout });
  }

  /**
   * Check if form field has validation error
   */
  async hasValidationError(fieldLocator: string, errorMessage: string): Promise<boolean> {
    const field = this.page.locator(fieldLocator);
    const parent = field.locator('xpath=..');
    
    // Look for error message near the field
    const errorElement = parent.locator(`text=${errorMessage}`);
    return await errorElement.count() > 0;
  }

  /**
   * Check if form field is required
   */
  async isFieldRequired(fieldLocator: string): Promise<boolean> {
    const field = this.page.locator(fieldLocator);
    const isRequired = await field.getAttribute('required');
    return isRequired !== null;
  }

  /**
   * Check if form field is disabled
   */
  async isFieldDisabled(fieldLocator: string): Promise<boolean> {
    const field = this.page.locator(fieldLocator);
    return await field.isDisabled();
  }

  /**
   * Check if form field is readonly
   */
  async isFieldReadonly(fieldLocator: string): Promise<boolean> {
    const field = this.page.locator(fieldLocator);
    const isReadonly = await field.getAttribute('readonly');
    return isReadonly !== null;
  }

  /**
   * Get form field value
   */
  async getFieldValue(fieldLocator: string): Promise<string> {
    const field = this.page.locator(fieldLocator);
    return await field.inputValue();
  }

  /**
   * Set form field value
   */
  async setFieldValue(fieldLocator: string, value: string): Promise<void> {
    const field = this.page.locator(fieldLocator);
    await field.fill(value);
  }

  /**
   * Clear form field
   */
  async clearField(fieldLocator: string): Promise<void> {
    const field = this.page.locator(fieldLocator);
    await field.clear();
  }

  /**
   * Check if button is enabled
   */
  async isButtonEnabled(buttonLocator: string): Promise<boolean> {
    const button = this.page.locator(buttonLocator);
    return await button.isEnabled();
  }

  /**
   * Check if button is disabled
   */
  async isButtonDisabled(buttonLocator: string): Promise<boolean> {
    const button = this.page.locator(buttonLocator);
    return await button.isDisabled();
  }

  /**
   * Click button and wait for action
   */
  async clickButtonAndWait(buttonLocator: string): Promise<void> {
    const button = this.page.locator(buttonLocator);
    await button.click();
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Generate random test data
   */
  generateRandomEmail(): string {
    const timestamp = Date.now();
    return `test-${timestamp}@example.com`;
  }

  generateRandomPassword(): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*';
    let password = '';
    for (let i = 0; i < 12; i++) {
      password += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return password;
  }

  /**
   * Log test information
   */
  async logTestInfo(testName: string, step: string, details?: any): Promise<void> {
    const timestamp = new Date().toISOString();
    const logMessage = `[${timestamp}] ${testName} - ${step}`;
    
    if (details) {
      console.log(logMessage, details);
    } else {
      console.log(logMessage);
    }
  }
}

/**
 * Create test helper instance
 */
export function createTestHelper(page: Page): TestHelpers {
  return new TestHelpers(page);
}

/**
 * Common test assertions
 */
export const testAssertions = {
  /**
   * Assert element is visible
   */
  async elementIsVisible(page: Page, locator: string, message?: string): Promise<void> {
    const element = page.locator(locator);
    await expect(element).toBeVisible({ timeout: 10000 });
  },

  /**
   * Assert element is not visible
   */
  async elementIsNotVisible(page: Page, locator: string, message?: string): Promise<void> {
    const element = page.locator(locator);
    await expect(element).not.toBeVisible({ timeout: 10000 });
  },

  /**
   * Assert element has text
   */
  async elementHasText(page: Page, locator: string, text: string, message?: string): Promise<void> {
    const element = page.locator(locator);
    await expect(element).toHaveText(text, { timeout: 10000 });
  },

  /**
   * Assert element contains text
   */
  async elementContainsText(page: Page, locator: string, text: string, message?: string): Promise<void> {
    const element = page.locator(locator);
    await expect(element).toContainText(text, { timeout: 10000 });
  },

  /**
   * Assert URL contains path
   */
  async urlContainsPath(page: Page, path: string, message?: string): Promise<void> {
    await expect(page).toHaveURL(new RegExp(path), { timeout: 10000 });
  },

  /**
   * Assert page has title
   */
  async pageHasTitle(page: Page, title: string, message?: string): Promise<void> {
    await expect(page).toHaveTitle(new RegExp(title), { timeout: 10000 });
  }
};
