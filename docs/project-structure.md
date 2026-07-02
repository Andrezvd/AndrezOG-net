# Estructura del Proyecto — AndrezOG

## Visión General

```
andrezog-net/
├── backend-net/           # API REST .NET 10
├── frontend-angular/      # Aplicación web Angular (stack principal)
├── frontend-react/        # Experimentos con React
├── docs/                  # Documentación del proyecto
├── scripts/               # Scripts de utilidad
├── .github/workflows/     # Pipelines CI/CD (GitHub Actions)
├── .gitignore
├── docker-compose.yml     # Orquestación de servicios
├── package.json           # Dependencias compartidas (axios para React)
└── AGENT.MD               # Contexto, roadmap y reglas del proyecto
```

## Backend (`backend-net/`)

API REST con arquitectura en capas siguiendo principios DDD simplificado:

```
backend-net/
├── Program.cs                        # Punto de entrada, configuración DI, middleware
├── appsettings.json                  # Configuración general
├── Properties/                       # Configuración de launchSettings
│
└── src/
    ├── api/                          # Capa de presentación (Controllers)
    │   └── rest/
    │       ├── AuthController.cs     # Autenticación (Google OAuth + JWT)
    │       ├── ProfileController.cs  # Perfil de portafolio
    │       ├── SkillController.cs    # Habilidades técnicas
    │       ├── dto/                  # Objetos de transferencia (request/response)
    │       └── mapper/               # Mapeo entre entidades y DTOs
    │
    ├── application/                  # Capa de aplicación (casos de uso)
    │       ├── AuthService.cs
    │       ├── ProfileService.cs
    │       ├── SkillService.cs
    │       ├── commands/             # Commandos (CQRS)
    │       ├── dto/                  # DTOs de aplicación
    │       ├── Iservices/            # Interfaces de servicios
    │       └── result/               # Objetos resultado
    │
    ├── domain/                       # Capa de dominio (entidades, interfaces)
    │   ├── Irepository/              # Interfaces de repositorios
    │   └── model/                    # Modelos de dominio
    │
    ├── infrastructure/               # Capa de infraestructura (persistencia, EF)
    │   ├── contextdb/                # DbContext y configuraciones de EF Core
    │   └── repository/               # Implementaciones de repositorios
    │
    └── shared/                       # Utilidades compartidas
        └── storageService/           # Servicio de almacenamiento de archivos
```

### Stack:
- **Framework**: .NET 10
- **ORM**: Entity Framework Core con Npgsql
- **BD**: PostgreSQL
- **Autenticación**: Google OAuth + JWT Bearer
- **Documentación API**: OpenAPI / Scalar

## Frontend Angular (`frontend-angular/`)

Aplicación web Angular con renderizado híbrido (SSR + CSR):

```
frontend-angular/
├── public/                       # Archivos estáticos
├── src/
│   ├── index.html                # Entry point HTML
│   ├── main.ts                   # Entry point browser
│   ├── main.server.ts            # Entry point server (SSR)
│   ├── server.ts                 # Servidor SSR (Angular Universal)
│   ├── styles.css                # Estilos globales
│   ├── app/
│   │   ├── app.ts                # Componente raíz
│   │   ├── app.html              # Template raíz
│   │   ├── app.css               # Estilos raíz
│   │   ├── app.config.ts         # Configuración de la app
│   │   ├── app.routes.ts         # Definición de rutas
│   │   ├── app.config.server.ts
│   │   ├── app.routes.server.ts
│   │   ├── app.spec.ts
│   │   ├── game-canvas/          # Componente canvas/juego
│   │   ├── pages/                # Componentes de páginas
│   │   ├── services/             # Servicios (HTTP, estado)
│   │   └── services-conf/        # Configuración de servicios
│   └── environments/             # Variables de entorno
│       ├── environment.ts
│       └── environment.development.ts
│
├── angular.json                  # Configuración de Angular CLI
├── package.json                  # Dependencias
├── tsconfig.json                 # Configuración TypeScript
└── tsconfig.app.json             # Config TS para la app
```

### Stack:
- **Framework**: Angular (con SSR)
- **Build**: Angular CLI
- **Package Manager**: pnpm

## Frontend React (`frontend-react/`)

Experimento personal con React (fuera del stack principal):

```
frontend-react/
├── public/
├── src/
├── index.html
├── vite.config.ts                # Configuración de Vite
├── eslint.config.js
├── tsconfig.json
├── tsconfig.app.json
├── tsconfig.node.json
└── .env
```

### Stack:
- **Framework**: React + TypeScript
- **Build**: Vite
- **Package Manager**: pnpm

## Servicios Docker

| Servicio    | Puerto Host | Puerto Container | Container            | Imagen/Build              |
|-------------|-------------|------------------|----------------------|---------------------------|
| PostgreSQL  | 5432        | 5432             | andrezog-postgres    | postgres:16-alpine        |
| Backend API | 5201        | 8080             | andrezog-backend     | ./backend-net/Dockerfile  |
| Frontend    | 4200        | 4000             | andrezog-frontend    | ./frontend-angular/Dockerfile |

Red interna: `andrezog-network` (bridge)

### Comandos comunes

```bash
# Iniciar todos los servicios
docker-compose up --build

# Iniciar solo PostgreSQL (para desarrollo local sin Docker)
docker-compose up postgres -d

# Ver logs de un servicio específico
docker-compose logs -f backend

# Detener y eliminar volúmenes
docker-compose down -v
```

## Ambientes

| Ambiente    | Estado     | Notas                          |
|-------------|------------|--------------------------------|
| Development | Local      | docker-compose up + dotnet run |
| Testing     | Pendiente  | Fase 3 del roadmap             |
| Production  | Pendiente  | Fase 3, 7, 8, 9 del roadmap   |

## Convenciones

- **Ramas**: Git Flow (desde Fase 4)
- **Commits**: Convencionales (feat, fix, chore, docs, refactor)
- **Código**: C# (PascalCase), TypeScript (camelCase)
- **BD**: Migrations con EF Core