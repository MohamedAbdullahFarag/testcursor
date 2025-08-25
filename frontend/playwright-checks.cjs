const { firefox, chromium } = require('playwright');

(async () => {
  // Launch a browser by resolving installed Playwright browser paths (avoids version-specific path issues)
  const fs = require('fs');
  const path = require('path');
  const local = process.env.LOCALAPPDATA || path.join(require('os').homedir(), 'AppData', 'Local');
  const msPlaywright = path.join(local, 'ms-playwright');

  function findExecutable(dirPatterns) {
    for (const p of dirPatterns) {
      const full = path.join(msPlaywright, p);
      if (fs.existsSync(full) && fs.statSync(full).isDirectory()) {
        // search for known binary locations
        const candidates = [];
        // firefox
        candidates.push(path.join(full, 'firefox', 'firefox.exe'));
        // chromium headless shell
        candidates.push(path.join(full, 'chrome-win', 'headless_shell.exe'));
        candidates.push(path.join(full, 'chrome-win', 'chrome.exe'));
        for (const c of candidates) {
          if (fs.existsSync(c)) return c;
        }
      }
    }
    return null;
  }

  let browser;
  const fxPath = findExecutable(['firefox-1429', 'firefox-1490', 'firefox-*/']);
  const chPath = findExecutable(['chromium-1091', 'chromium-*/', 'mcp-chrome-9048f19']);
  try {
    if (fxPath) {
      browser = await firefox.launch({ headless: true, executablePath: fxPath });
    } else if (chPath) {
      browser = await chromium.launch({ headless: true, executablePath: chPath });
    } else {
      // fall back to playwright's default launcher
      browser = await firefox.launch({ headless: true });
    }
  } catch (e) {
    console.warn('Browser launch failed:', e.message);
    throw e;
  }
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();

  const base = 'https://localhost:5173';
  const routes = ['/', '/login', '/faq', '/support', '/eparticipation', '/dashboard', '/dashboard/media', '/dashboard/user-management'];

  for (const r of routes) {
    const url = base + r;
    try {
      const resp = await page.goto(url, { waitUntil: 'domcontentloaded', timeout: 15000 });
      const title = await page.title();
      console.log(`${url} -> status=${resp ? resp.status() : 'no-response'} title=${title}`);
    } catch (e) {
      console.log(`${url} -> ERROR: ${e.message}`);
    }
  }

  await browser.close();
})();
