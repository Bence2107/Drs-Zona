import { existsSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const repoRoot = resolve(dirname(fileURLToPath(import.meta.url)), '..', '..');

// The dev server reads the backend's .env, so each port is set in one place for both sides
export const loadEnv = () => {
  const explicitPath = process.env.ENV_FILE;
  const path = explicitPath ? resolve(explicitPath) : resolve(repoRoot, '.env');

  if (existsSync(path)) {
    process.loadEnvFile(path);
    return;
  }
  // Same rule as the API: a wrong ENV_FILE must not silently fall back to the default ports
  if (explicitPath) {
    throw new Error(`ENV_FILE points to ${path}, but that file does not exist.`);
  }
};
