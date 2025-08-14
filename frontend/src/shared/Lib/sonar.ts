import { createRequire } from 'module'

const require = createRequire(import.meta.url)
const { default: sonarqubeScanner } = require('sonarqube-scanner')

sonarqubeScanner(
    {
        serverUrl: '#{sonarQubeServerUrl}#',
        token: '#{sonarQubeToken}#',
        options: {
            'sonar.projectName': '#{sonarQubeProjectName}#',
            'sonar.projectKey': '#{sonarQubeProjectKey}#',
            'sonar.projectVersion': '#{sonarQubeProjectVersion}#',
            'sonar.sources': './src',
            'sonar.exclusions': '**/*.test.*,**/__snapshots__/**,**/sonar.ts',
        },
    },
    () => process.exit(),
)
