const https = require('https');
const { URL } = require('url');

// Bypass SSL certificate validation for localhost
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFkbWluQGlraHRpYmFyLmNvbSIsInVzZXJfaWQiOiIxIiwicm9sZSI6IkFETUlOIiwibmJmIjoxNzM4MDA5OTQ3LCJleHAiOjE3MzgwMTM1NDcsImlhdCI6MTczODAwOTk0NywiaXNzIjoiSWtodGliYXJJc3N1ZXIiLCJhdWQiOiJJa2h0aWJhckF1ZGllbmNlIn0.aYu7Z0yyEH8Y7DZXMJf4oWBkYYYgPd6hLHOgzCxzQzU';

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

console.log('Testing audit logs API endpoint...');
console.log(`URL: ${url.href}`);

const req = https.request(options, (res) => {
  console.log(`Status Code: ${res.statusCode}`);
  console.log(`Headers:`, res.headers);
  
  let data = '';
  res.on('data', (chunk) => {
    data += chunk;
  });
  
  res.on('end', () => {
    console.log('Response:');
    try {
      const parsed = JSON.parse(data);
      console.log(JSON.stringify(parsed, null, 2));
    } catch (e) {
      console.log('Raw response:', data);
    }
  });
});

req.on('error', (error) => {
  console.error('Request error:', error);
});

req.end();
