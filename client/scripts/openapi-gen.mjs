import { spawn } from 'node:child_process';
import { loadEnv } from './load-env.mjs';

loadEnv();

// Swagger is served before the HTTPS redirect, so HTTP avoids the self-signed certificate
const apiPort = process.env.API_PORT || '5244';
const args = [`--input=http://localhost:${apiPort}/swagger/v1/swagger.json`, '--output=src/app/api/'];

spawn('ng-openapi-gen', args, { stdio: 'inherit', shell: true }).on('exit', (code) => process.exit(code ?? 1));
