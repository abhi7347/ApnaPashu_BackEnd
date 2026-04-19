const http = require('http');
const https = require('https');

const data = JSON.stringify({
  email: 'abhishektiwari7347@gmail.com', // I'll just try to hit login to see if it works, even if 401
  password: 'Password123!'
});

const options = {
  hostname: 'localhost',
  port: 7001,
  path: '/api/authentication/login',
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Content-Length': data.length
  },
  rejectUnauthorized: false
};

const req = https.request(options, res => {
  console.log('Login Status:', res.statusCode);
  let cookie = res.headers['set-cookie'];
  console.log('Set-Cookie:', cookie);
  
  if (cookie) {
    const meOptions = {
        hostname: 'localhost',
        port: 7001,
        path: '/api/authentication/me',
        method: 'GET',
        headers: {
            'Cookie': cookie[0].split(';')[0]
        },
        rejectUnauthorized: false
    };
    
    https.request(meOptions, meRes => {
        console.log('Me Status:', meRes.statusCode);
        let body = '';
        meRes.on('data', d => body += d);
        meRes.on('end', () => console.log('Me Body:', body));
    }).end();
  }
});

req.on('error', error => {
  console.error(error);
});

req.write(data);
req.end();
