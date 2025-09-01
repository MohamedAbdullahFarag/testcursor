import * as fs from 'fs';
import * as path from 'path';

export interface TestResult {
  testId: string;
  testName: string;
  status: 'passed' | 'failed' | 'skipped' | 'blocked';
  executionTime: number;
  errorMessage?: string;
  screenshotPath?: string;
  timestamp: Date;
  browser?: string;
  viewport?: { width: number; height: number };
}

export interface PRPStatus {
  phase: string;
  status: 'pending' | 'in-progress' | 'completed' | 'blocked';
  completionDate?: Date;
  notes?: string;
  dependencies?: string[];
  testResults?: TestResult[];
}

export interface PRPProjectStatus {
  project: string;
  currentPhase: string;
  phases: Record<string, PRPStatus>;
  testResults: {
    totalTests: number;
    passedTests: number;
    failedTests: number;
    skippedTests: number;
    blockedTests: number;
    lastUpdated: string;
    coverage: number;
  };
  metadata: {
    version: string;
    lastModified: string;
    testEnvironment: string;
    browser: string;
  };
}

/**
 * PRP Status Management and Test Reporting
 * Handles status updates, test result tracking, and report generation
 */

export class PRPReporting {
  private statusFilePath: string;
  private resultsFilePath: string;
  private projectStatus: PRPProjectStatus;

  constructor(statusDir: string = 'status') {
    this.statusFilePath = path.join(statusDir, 'prp-status.json');
    this.resultsFilePath = path.join(statusDir, 'test-results.json');
    this.projectStatus = this.initializeProjectStatus();
    this.ensureDirectoryExists(statusDir);
  }

  /**
   * Initialize project status with default values
   */
  private initializeProjectStatus(): PRPProjectStatus {
    return {
      project: 'Ikhtibar Educational Exam Management System',
      currentPhase: 'Test Implementation',
      phases: {
        'Requirements Analysis': {
          phase: 'Requirements Analysis',
          status: 'completed',
          completionDate: new Date('2024-01-15T10:00:00Z'),
          notes: 'All test requirements identified and documented',
          testResults: []
        },
        'Test Planning': {
          phase: 'Test Planning',
          status: 'completed',
          completionDate: new Date('2024-01-20T14:30:00Z'),
          notes: 'Test strategy and structure designed',
          testResults: []
        },
        'Test Design': {
          phase: 'Test Design',
          status: 'in-progress',
          notes: 'Implementing Page Object Model framework',
          testResults: []
        },
        'Test Implementation': {
          phase: 'Test Implementation',
          status: 'pending',
          dependencies: ['Test Design'],
          testResults: []
        },
        'Test Execution': {
          phase: 'Test Execution',
          status: 'pending',
          dependencies: ['Test Implementation'],
          testResults: []
        },
        'Test Evaluation': {
          phase: 'Test Evaluation',
          status: 'pending',
          dependencies: ['Test Execution'],
          testResults: []
        }
      },
      testResults: {
        totalTests: 0,
        passedTests: 0,
        failedTests: 0,
        skippedTests: 0,
        blockedTests: 0,
        lastUpdated: new Date().toISOString(),
        coverage: 0
      },
      metadata: {
        version: '1.0.0',
        lastModified: new Date().toISOString(),
        testEnvironment: 'development',
        browser: 'chromium'
      }
    };
  }

  /**
   * Ensure directory exists
   */
  private ensureDirectoryExists(dirPath: string): void {
    if (!fs.existsSync(dirPath)) {
      fs.mkdirSync(dirPath, { recursive: true });
    }
  }

  /**
   * Load existing status file
   */
  private async loadStatusFile(): Promise<PRPProjectStatus> {
    try {
      if (fs.existsSync(this.statusFilePath)) {
        const fileContent = fs.readFileSync(this.statusFilePath, 'utf8');
        return JSON.parse(fileContent);
      }
    } catch (error) {
      console.warn('Could not load existing status file, using default:', error);
    }
    return this.projectStatus;
  }

  /**
   * Save status to file
   */
  private async saveStatusFile(): Promise<void> {
    try {
      this.projectStatus.metadata.lastModified = new Date().toISOString();
      fs.writeFileSync(this.statusFilePath, JSON.stringify(this.projectStatus, null, 2));
    } catch (error) {
      console.error('Error saving status file:', error);
    }
  }

  /**
   * Update PRP phase status
   */
  async updatePRPStatus(phase: string, status: string, notes?: string): Promise<void> {
    const currentStatus = await this.loadStatusFile();
    
    if (currentStatus.phases[phase]) {
      currentStatus.phases[phase].status = status as any;
      currentStatus.phases[phase].completionDate = status === 'completed' ? new Date() : undefined;
      currentStatus.phases[phase].notes = notes || `Updated at ${new Date().toISOString()}`;
      
      if (status === 'completed') {
        currentStatus.currentPhase = this.getNextPhase(phase);
      }
    }

    this.projectStatus = currentStatus;
    await this.saveStatusFile();
  }

  /**
   * Get next phase after current one
   */
  private getNextPhase(currentPhase: string): string {
    const phaseOrder = [
      'Requirements Analysis',
      'Test Planning',
      'Test Design',
      'Test Implementation',
      'Test Execution',
      'Test Evaluation'
    ];
    
    const currentIndex = phaseOrder.indexOf(currentPhase);
    if (currentIndex < phaseOrder.length - 1) {
      return phaseOrder[currentIndex + 1];
    }
    return currentPhase;
  }

  /**
   * Add test result to specific phase
   */
  async addTestResult(phase: string, testResult: TestResult): Promise<void> {
    const currentStatus = await this.loadStatusFile();
    
    if (currentStatus.phases[phase]) {
      if (!currentStatus.phases[phase].testResults) {
        currentStatus.phases[phase].testResults = [];
      }
      currentStatus.phases[phase].testResults!.push(testResult);
      
      // Update overall test results
      this.updateOverallTestResults(currentStatus, testResult);
    }

    this.projectStatus = currentStatus;
    await this.saveStatusFile();
  }

  /**
   * Update overall test results statistics
   */
  private updateOverallTestResults(status: PRPProjectStatus, testResult: TestResult): void {
    status.testResults.totalTests++;
    
    switch (testResult.status) {
      case 'passed':
        status.testResults.passedTests++;
        break;
      case 'failed':
        status.testResults.failedTests++;
        break;
      case 'skipped':
        status.testResults.skippedTests++;
        break;
      case 'blocked':
        status.testResults.blockedTests++;
        break;
    }
    
    status.testResults.lastUpdated = new Date().toISOString();
    status.testResults.coverage = this.calculateCoverage(status.testResults);
  }

  /**
   * Calculate test coverage percentage
   */
  private calculateCoverage(testResults: PRPProjectStatus['testResults']): number {
    if (testResults.totalTests === 0) return 0;
    return Math.round((testResults.passedTests / testResults.totalTests) * 100);
  }

  /**
   * Get current phase status
   */
  async getCurrentPhaseStatus(): Promise<PRPStatus | undefined> {
    const currentStatus = await this.loadStatusFile();
    return currentStatus.phases[currentStatus.currentPhase];
  }

  /**
   * Get all phases status
   */
  async getAllPhasesStatus(): Promise<Record<string, PRPStatus>> {
    const currentStatus = await this.loadStatusFile();
    return currentStatus.phases;
  }

  /**
   * Get overall test results
   */
  async getOverallTestResults(): Promise<PRPProjectStatus['testResults']> {
    const currentStatus = await this.loadStatusFile();
    return currentStatus.testResults;
  }

  /**
   * Generate test execution report
   */
  async generateTestReport(): Promise<string> {
    const currentStatus = await this.loadStatusFile();
    const timestamp = new Date().toISOString();
    
    let report = `# Test Execution Report\n\n`;
    report += `**Generated:** ${timestamp}\n`;
    report += `**Project:** ${currentStatus.project}\n`;
    report += `**Current Phase:** ${currentStatus.currentPhase}\n\n`;
    
    // Overall Results
    report += `## Overall Test Results\n\n`;
    report += `- **Total Tests:** ${currentStatus.testResults.totalTests}\n`;
    report += `- **Passed:** ${currentStatus.testResults.passedTests}\n`;
    report += `- **Failed:** ${currentStatus.testResults.failedTests}\n`;
    report += `- **Skipped:** ${currentStatus.testResults.skippedTests}\n`;
    report += `- **Blocked:** ${currentStatus.testResults.blockedTests}\n`;
    report += `- **Coverage:** ${currentStatus.testResults.coverage}%\n\n`;
    
    // Phase Status
    report += `## Phase Status\n\n`;
    for (const [phaseName, phaseStatus] of Object.entries(currentStatus.phases)) {
      const statusIcon = phaseStatus.status === 'completed' ? '✅' : 
                        phaseStatus.status === 'in-progress' ? '🔄' : 
                        phaseStatus.status === 'blocked' ? '🚫' : '⏳';
      
      report += `### ${statusIcon} ${phaseName}\n`;
      report += `- **Status:** ${phaseStatus.status}\n`;
      if (phaseStatus.completionDate) {
        report += `- **Completed:** ${phaseStatus.completionDate}\n`;
      }
      if (phaseStatus.notes) {
        report += `- **Notes:** ${phaseStatus.notes}\n`;
      }
      if (phaseStatus.testResults && phaseStatus.testResults.length > 0) {
        report += `- **Tests:** ${phaseStatus.testResults.length} executed\n`;
      }
      report += `\n`;
    }
    
    return report;
  }

  /**
   * Save test report to file
   */
  async saveTestReport(): Promise<void> {
    const report = await this.generateTestReport();
    const reportPath = path.join(path.dirname(this.statusFilePath), 'test-report.md');
    
    try {
      fs.writeFileSync(reportPath, report);
      console.log(`Test report saved to: ${reportPath}`);
    } catch (error) {
      console.error('Error saving test report:', error);
    }
  }

  /**
   * Export status to JSON
   */
  async exportStatusToJson(): Promise<string> {
    const currentStatus = await this.loadStatusFile();
    return JSON.stringify(currentStatus, null, 2);
  }

  /**
   * Import status from JSON
   */
  async importStatusFromJson(jsonData: string): Promise<void> {
    try {
      const importedStatus = JSON.parse(jsonData);
      this.projectStatus = importedStatus;
      await this.saveStatusFile();
      console.log('Status imported successfully');
    } catch (error) {
      console.error('Error importing status:', error);
    }
  }

  /**
   * Reset all phases to pending
   */
  async resetAllPhases(): Promise<void> {
    for (const phase of Object.keys(this.projectStatus.phases)) {
      this.projectStatus.phases[phase].status = 'pending';
      this.projectStatus.phases[phase].completionDate = undefined;
      this.projectStatus.phases[phase].testResults = [];
    }
    
    this.projectStatus.currentPhase = 'Requirements Analysis';
    this.projectStatus.testResults = {
      totalTests: 0,
      passedTests: 0,
      failedTests: 0,
      skippedTests: 0,
      blockedTests: 0,
      lastUpdated: new Date().toISOString(),
      coverage: 0
    };
    
    await this.saveStatusFile();
  }

  /**
   * Get phase dependencies
   */
  async getPhaseDependencies(phase: string): Promise<string[]> {
    const currentStatus = await this.loadStatusFile();
    return currentStatus.phases[phase]?.dependencies || [];
  }

  /**
   * Check if phase can be started
   */
  async canStartPhase(phase: string): Promise<boolean> {
    const dependencies = await this.getPhaseDependencies(phase);
    
    if (dependencies.length === 0) return true;
    
    const currentStatus = await this.loadStatusFile();
    return dependencies.every(dep => 
      currentStatus.phases[dep]?.status === 'completed'
    );
  }
}

/**
 * Create PRP reporting instance
 */
export function createPRPReporting(statusDir?: string): PRPReporting {
  return new PRPReporting(statusDir);
}

/**
 * Helper function to update PRP status
 */
export async function updatePRPStatus(
  phase: string, 
  status: string, 
  notes?: string,
  statusDir?: string
): Promise<void> {
  const reporting = createPRPReporting(statusDir);
  await reporting.updatePRPStatus(phase, status, notes);
}

/**
 * Helper function to add test result
 */
export async function addTestResult(
  phase: string,
  testResult: TestResult,
  statusDir?: string
): Promise<void> {
  const reporting = createPRPReporting(statusDir);
  await reporting.addTestResult(phase, testResult);
}
