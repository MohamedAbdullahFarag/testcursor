const https = require('https');
const { URL } = require('url');

// Bypass SSL certificate validation for localhost
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

async function authenticate() {
  const loginData = JSON.stringify({
    email: 'admin@ikhtibar.com',
    password: 'Admin123!'
  });

  const loginOptions = {
    hostname: 'localhost',
    port: 7001,
    path: '/api/auth/login',
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Content-Length': Buffer.byteLength(loginData)
    },
    rejectUnauthorized: false
  };

  return new Promise((resolve, reject) => {
    const req = https.request(loginOptions, (res) => {
      console.log(`Login Status Code: ${res.statusCode}`);
      
      let data = '';
      res.on('data', (chunk) => {
        data += chunk;
      });
      
      res.on('end', () => {
        try {
          const parsed = JSON.parse(data);
          if (res.statusCode === 200 && parsed.token) {
            console.log('Login successful!');
            resolve(parsed.token);
          } else {
            console.log('Login failed:', parsed);
            reject(new Error('Login failed'));
          }
        } catch (e) {
          console.log('Login response parsing error:', e);
          console.log('Raw response:', data);
          reject(e);
        }
      });
    });

    req.on('error', (error) => {
      console.error('Login request error:', error);
      reject(error);
    });

    req.write(loginData);
    req.end();
  });
}

async function testAuditLogs(token) {
  const url = new URL('https://localhost:7001/api/audit-logs?page=0&pageSize=25&sortBy=timestamp&sortDirection=desc&searchText=');

  const options = {
    hostname: url.hostname,
    port: url.port,
    path: url.pathname + url.search,
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    rejectUnauthorized: false
  };

  return new Promise((resolve, reject) => {
    const req = https.request(options, (res) => {
      console.log(`Audit Logs Status Code: ${res.statusCode}`);
      console.log(`Headers:`, res.headers);
      
      let data = '';
      res.on('data', (chunk) => {
        data += chunk;
      });
      
      res.on('end', () => {
        console.log('Audit Logs Response:');
        try {
          const parsed = JSON.parse(data);
          console.log(JSON.stringify(parsed, null, 2));
          resolve(parsed);
        } catch (e) {
          console.log('Raw response:', data);
          resolve(data);
        }
      });
    });

    req.on('error', (error) => {
      console.error('Audit logs request error:', error);
      reject(error);
    });

    req.end();
  });
}

async function main() {
  try {
    console.log('Step 1: Authenticating...');
    const token = await authenticate();
    
    console.log('\nStep 2: Testing audit logs API...');
    const result = await testAuditLogs(token);
    
    console.log('\n✅ Test completed successfully!');
  } catch (error) {
    console.error('❌ Test failed:', error);
  }
}

main();
