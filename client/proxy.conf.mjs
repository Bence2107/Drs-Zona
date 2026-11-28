import { loadEnv } from './scripts/load-env.mjs';

// Also loaded here, so a plain `ng serve` (without pnpm start) follows .env too
loadEnv();

const httpPort = process.env.API_PORT || '5244';
const httpsPort = process.env.API_HTTPS_PORT || '7221';

export default {
  // Static files are served before the API's HTTPS redirect, so plain HTTP is enough here
  '/uploads': {
    target: `http://localhost:${httpPort}`,
    secure: false,
    changeOrigin: true,
  },
  '/api': {
    target: `https://localhost:${httpsPort}`,
    // The local dev certificate is self-signed
    secure: false,
    changeOrigin: true,
  },
};
