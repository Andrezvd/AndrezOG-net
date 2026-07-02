import {
  AngularNodeAppEngine,
  createNodeRequestHandler,
  isMainModule,
  writeResponseToNodeResponse,
} from '@angular/ssr/node';
import express from 'express';
import { join } from 'node:path';
import { Readable } from 'node:stream';

const browserDistFolder = join(import.meta.dirname, '../browser');
const backendBaseUrl = process.env['BACKEND_BASE_URL'] ?? 'http://localhost:5201';

const app = express();
const angularApp = new AngularNodeAppEngine();

const proxyPaths = ['/api', '/uploads'];

app.get('/health', (_req, res) => {
  res.status(200).json({ status: 'ok' });
});

app.use(async (req, res, next) => {
  const shouldProxy = proxyPaths.some((path) => req.originalUrl === path || req.originalUrl.startsWith(`${path}/`));

  if (!shouldProxy) {
    next();
    return;
  }

  try {
    const targetUrl = new URL(req.originalUrl, backendBaseUrl);
    const headers = new Headers();
    let requestBody: Buffer | undefined;

    for (const [key, value] of Object.entries(req.headers)) {
      if (value === undefined || key === 'host' || key === 'connection' || key === 'content-length') {
        continue;
      }

      headers.set(key, Array.isArray(value) ? value.join(',') : value);
    }

    const hasBody = req.method !== 'GET' && req.method !== 'HEAD';
    if (hasBody) {
      const chunks: Buffer[] = [];

      for await (const chunk of req) {
        chunks.push(Buffer.isBuffer(chunk) ? chunk : Buffer.from(chunk));
      }

      requestBody = Buffer.concat(chunks);
    }

    const response = await fetch(targetUrl, {
      method: req.method,
      headers,
      body: requestBody ? (requestBody as unknown as BodyInit) : undefined,
    } as RequestInit & { duplex?: string });

    res.status(response.status);

    response.headers.forEach((value, key) => {
      if (key.toLowerCase() !== 'transfer-encoding') {
        res.setHeader(key, value);
      }
    });

    if (!response.body) {
      res.end();
      return;
    }

    Readable.fromWeb(response.body as any).pipe(res);
  } catch (error) {
    next(error);
  }
});

/**
 * Example Express Rest API endpoints can be defined here.
 * Uncomment and define endpoints as necessary.
 *
 * Example:
 * ```ts
 * app.get('/api/{*splat}', (req, res) => {
 *   // Handle API request
 * });
 * ```
 */

/**
 * Serve static files from /browser
 */
app.use(
  express.static(browserDistFolder, {
    maxAge: '1y',
    index: false,
    redirect: false,
  }),
);

/**
 * Handle all other requests by rendering the Angular application.
 */
app.use((req, res, next) => {
  angularApp
    .handle(req)
    .then((response) =>
      response ? writeResponseToNodeResponse(response, res) : next(),
    )
    .catch(next);
});

/**
 * Start the server if this module is the main entry point, or it is ran via PM2.
 * The server listens on the port defined by the `PORT` environment variable, or defaults to 4000.
 */
if (isMainModule(import.meta.url) || process.env['pm_id']) {
  const port = process.env['PORT'] || 4000;
  app.listen(port, (error) => {
    if (error) {
      throw error;
    }

    console.log(`Node Express server listening on http://localhost:${port}`);
  });
}

/**
 * Request handler used by the Angular CLI (for dev-server and during build) or Firebase Cloud Functions.
 */
export const reqHandler = createNodeRequestHandler(app);
