import { spawn } from 'node:child_process';
import { loadEnv } from './load-env.mjs';

loadEnv();

// Without FRONTEND_PORT, ng serve keeps its own default (4200)
const portArgs = process.env.FRONTEND_PORT ? ['--port', process.env.FRONTEND_PORT] : [];
// Arguments after `pnpm start --` come last, so they still win
const args = ['serve', ...portArgs, ...process.argv.slice(2)];

spawn('ng', args, { stdio: 'inherit', shell: true }).on('exit', (code) => process.exit(code ?? 1));
