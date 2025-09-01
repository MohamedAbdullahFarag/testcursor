import { Page } from '@playwright/test';
import { BasePage } from './BasePage';

export class DashboardPage extends BasePage {
  // Locators
  private readonly dashboardTitle = '[data-testid="dashboard-title"]';
  private readonly userMenu = '[data-testid="user-menu"]';
  private readonly logoutButton = '[data-testid="logout-button"]';
  private readonly sidebar = '[data-testid="sidebar"]';
  private readonly mainContent = '[data-testid="main-content"]';
  private readonly welcomeMessage = '[data-testid="welcome-message"]';
  private readonly quickActions = '[data-testid="quick-actions"]';
  private readonly notifications = '[data-testid="notifications"]';
  private readonly userProfile = '[data-testid="user-profile"]';

  constructor(page: Page, baseUrl: string) {
    super(page, baseUrl);
  }

  /**
   * Navigate to dashboard
   */
  async navigateToDashboard(): Promise<void> {
    await this.navigate('/dashboard');
    await this.waitForPageLoad();
  }

  /**
   * Get dashboard title
   */
  async getDashboardTitle(): Promise<string> {
    return await this.getText(this.dashboardTitle);
  }

  /**
   * Check if dashboard is loaded
   */
  async isDashboardLoaded(): Promise<boolean> {
    return await this.isVisible(this.dashboardTitle) && 
           await this.isVisible(this.sidebar) && 
           await this.isVisible(this.mainContent);
  }

  /**
   * Open user menu
   */
  async openUserMenu(): Promise<void> {
    await this.page.click(this.userMenu);
  }

  /**
   * Click logout button
   */
  async logout(): Promise<void> {
    await this.page.click(this.logoutButton);
  }

  /**
   * Get welcome message
   */
  async getWelcomeMessage(): Promise<string> {
    return await this.getText(this.welcomeMessage);
  }

  /**
   * Check if quick actions are visible
   */
  async areQuickActionsVisible(): Promise<boolean> {
    return await this.isVisible(this.quickActions);
  }

  /**
   * Check if notifications are visible
   */
  async areNotificationsVisible(): Promise<boolean> {
    return await this.isVisible(this.notifications);
  }

  /**
   * Get user profile information
   */
  async getUserProfileInfo(): Promise<string> {
    return await this.getText(this.userProfile);
  }

  /**
   * Wait for dashboard to fully load
   */
  async waitForDashboardLoad(): Promise<void> {
    await this.waitForElement(this.dashboardTitle);
    await this.waitForElement(this.sidebar);
    await this.waitForElement(this.mainContent);
  }

  /**
   * Check if user is authenticated (dashboard accessible)
   */
  async isUserAuthenticated(): Promise<boolean> {
    try {
      await this.navigateToDashboard();
      return await this.isDashboardLoaded();
    } catch {
      return false;
    }
  }

  /**
   * Take dashboard screenshot
   */
  async takeDashboardScreenshot(): Promise<void> {
    await this.takeScreenshot('dashboard');
  }
}
