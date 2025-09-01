const http = require('http');

console.log('Testing backend connectivity...');

const options = {
  hostname: 'localhost',
  port: 5000,
  path: '/health',
  method: 'GET'
};

const req = http.request(options, (res) => {
  console.log(`Backend Health Check - Status: ${res.statusCode}`);
  
  let data = '';
  res.on('data', (chunk) => {
    data += chunk;
  });
  
  res.on('end', () => {
    console.log(`Response: ${data}`);
    console.log('✅ Backend is accessible');
  });
});

req.on('error', (e) => {
  console.error(`❌ Backend connection failed: ${e.message}`);
});

req.end();
