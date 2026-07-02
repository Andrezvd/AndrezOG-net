# ─────────────────────────────────────────────────────
# Dockerfile — Frontend Angular 21 (AndrezOG)
# Multi-stage build con SSR + Express
# ─────────────────────────────────────────────────────

# === ETAPA 1: BUILD ===
FROM node:22-bookworm-slim AS build
WORKDIR /app

# Instalar pnpm
RUN corepack enable && corepack prepare pnpm@11.1.2 --activate

# Copiar archivos de dependencias (cachea layers)
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
RUN pnpm install --frozen-lockfile

# Copiar el resto del código y compilar
COPY . .
RUN pnpm build

# === ETAPA 2: PRODUCCIÓN ===
FROM gcr.io/distroless/nodejs22-debian12 AS runtime
WORKDIR /app

ENV NODE_ENV=production

# Copiar solo lo necesario desde la etapa de build
COPY --from=build /app/dist/frontend-angular ./dist

# Copiar healthcheck (compatible con distroless: sin shell, sin curl/wget)
COPY healthcheck.mjs ./

# Puerto del servidor SSR
EXPOSE 4000

# Ejecutar el servidor SSR de Angular
CMD ["dist/server/server.mjs"]
