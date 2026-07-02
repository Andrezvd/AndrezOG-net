#!/usr/bin/env node

// ─────────────────────────────────────────────────────────────────
// AndrezOG — Healthcheck para entorno distroless
// Verifica que el servidor SSR responda 200 en /health.
// Se usa desde docker-compose.yml como test de healthcheck.
// Compatible con imágenes distroless (sin shell, sin curl/wget).
// ─────────────────────────────────────────────────────────────────

import http from 'http';

const PORT = process.env['PORT'] || 4000;
const HOST = 'localhost';
const PATH = '/health';
const TIMEOUT_SECONDS = 10;

const options = {
  host: HOST,
  port: Number(PORT),
  path: PATH,
  timeout: TIMEOUT_SECONDS * 1000,
};

const req = http.get(options, (res) => {
  process.exit(res.statusCode === 200 ? 0 : 1);
});

req.on('error', () => {
  process.exit(1);
});

req.on('timeout', () => {
  req.destroy();
  process.exit(1);
});