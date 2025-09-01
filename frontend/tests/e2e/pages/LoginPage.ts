import { Page } from '@playwright/test';
import { BasePage } from './BasePage';

export interface LoginCredentials {
  email: string;
  password: string;
}

export class LoginPage extends BasePage {
  // Locators - using actual application selectors
  private readonly emailInput = 'input[name="email"]';
  private readonly passwordInput = 'input[name="password"]';
  private readonly loginButton = 'button[type="submit"]';
  private readonly errorMessage = 'div[class*="text-red-600"]';
  private readonly successMessage = 'div[class*="text-green-600"]';
  private readonly loadingSpinner = 'button:has-text("Signing in...")';
  private readonly loginForm = 'form';
  private readonly pageTitle = 'h1';
  private readonly welcomeText = 'p';

  constructor(page: Page, baseUrl: string) {
    super(page, baseUrl);
  }

  /**
   * Navigate to login page
   */
  async navigateToLogin(): Promise<void> {
    await this.navigate('/login');
    await this.waitForPageLoad();
  }

  /**
   * Fill login form with credentials
   */
  async fillLoginForm(credentials: LoginCredentials): Promise<void> {
    await this.page.fill(this.emailInput, credentials.email);
    await this.page.fill(this.passwordInput, credentials.password);
  }

  /**
   * Submit login form
   */
  async submitLoginForm(): Promise<void> {
    await this.page.click(this.loginButton);
  }

  /**
   * Complete login process
   */
  async login(credentials: LoginCredentials): Promise<void> {
    await this.fillLoginForm(credentials);
    await this.submitLoginForm();
  }

  /**
   * Get error message text
   */
  async getErrorMessage(): Promise<string> {
    await this.waitForElement(this.errorMessage);
    return await this.getText(this.errorMessage);
  }

  /**
   * Get success message text
   */
  async getSuccessMessage(): Promise<string> {
    await this.waitForElement(this.successMessage);
    return await this.getText(this.successMessage);
  }

  /**
   * Check if error message is visible
   */
  async isErrorMessageVisible(): Promise<boolean> {
    return await this.isVisible(this.errorMessage);
  }

  /**
   * Check if loading spinner is visible
   */
  async isLoadingSpinnerVisible(): Promise<boolean> {
    return await this.isVisible(this.loadingSpinner);
  }

  /**
   * Wait for loading spinner to disappear
   */
  async waitForLoadingToComplete(): Promise<void> {
    if (await this.isLoadingSpinnerVisible()) {
      await this.waitForElementToDisappear(this.loadingSpinner);
    }
  }

  /**
   * Get page title text
   */
  async getLoginPageTitle(): Promise<string> {
    return await this.getText(this.pageTitle);
  }

  /**
   * Get welcome text
   */
  async getWelcomeText(): Promise<string> {
    return await this.getText(this.welcomeText);
  }

  /**
   * Check if login form is visible
   */
  async isLoginFormVisible(): Promise<boolean> {
    return await this.isVisible(this.loginForm);
  }

  /**
   * Clear form fields
   */
  async clearForm(): Promise<void> {
    await this.page.fill(this.emailInput, '');
    await this.page.fill(this.passwordInput, '');
  }

  /**
   * Get email input value
   */
  async getEmailInputValue(): Promise<string> {
    return await this.page.inputValue(this.emailInput);
  }

  /**
   * Get password input value
   */
  async getPasswordInputValue(): Promise<string> {
    return await this.page.inputValue(this.passwordInput);
  }

  /**
   * Check if login button is enabled
   */
  async isLoginButtonEnabled(): Promise<boolean> {
    const button = this.page.locator(this.loginButton);
    return await button.isEnabled();
  }

  /**
   * Check if login button is disabled
   */
  async isLoginButtonDisabled(): Promise<boolean> {
    const button = this.page.locator(this.loginButton);
    return await button.isDisabled();
  }

  /**
   * Wait for navigation after successful login
   */
  async waitForLoginRedirect(): Promise<void> {
    await this.waitForNavigation();
  }

  /**
   * Check if user is redirected to dashboard
   */
  async isRedirectedToDashboard(): Promise<boolean> {
    const currentUrl = await this.getCurrentUrl();
    return currentUrl.includes('/dashboard');
  }

  /**
   * Take screenshot of login form
   */
  async takeLoginFormScreenshot(): Promise<void> {
    await this.takeScreenshot('login-form');
  }

  /**
   * Take screenshot of error state
   */
  async takeErrorScreenshot(): Promise<void> {
    await this.takeScreenshot('login-error');
  }
}
