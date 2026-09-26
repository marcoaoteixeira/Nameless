// proxy.conf.js  (note: .js, not .json — required for process.env access)
const target = process.env['BFF_HTTPS'] || process.env['BFF_HTTP'];

if (!target) {
  throw new Error(
    'No BFF endpoint found. Expected BFF_HTTPS or BFF_HTTP environment variables. Make sure .WithReference(bff) is set in the AppHost.'
  );
}

module.exports = {
  '/bff': {
    target,
    changeOrigin: true,
    secure: false,
    logLevel: 'debug'
  },
  '/connect': {
    target,
    changeOrigin: true,
    secure: false,
    logLevel: 'debug'
  }
};
