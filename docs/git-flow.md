# Git Flow — AndrezOG

## Ramas

| Rama | Propósito | Base | Mergea a | ¿Vive en remoto? |
|---|---|---|---|---|
| `master` | Producción. Solo código liberado. | — | — | Sí |
| `develop` | Integración de funcionalidades completadas. | `master` | — | Sí |
| `feature/*` | Nueva funcionalidad o mejora. | `develop` | `develop` | Sí |
| `release/*` | Preparación de una nueva versión. | `develop` | `master` + `develop` | Sí |
| `hotfix/*` | Corrección urgente sobre producción. | `master` | `master` + `develop` | Sí |

## Flujo de trabajo diario

### Crear una funcionalidad nueva

```bash
# 1. Partir desde develop siempre
git checkout develop
git pull origin develop

# 2. Crear rama feature
git checkout -b feature/mi-funcionalidad

# 3. Trabajar normalmente (varios commits)
git add .
git commit -m "feat(scope): descripción del cambio"

# 4. Publicar rama para respaldo o revisión
git push -u origin feature/mi-funcionalidad

# 5. Abrir Pull Request en GitHub:
#    base: develop  ←  compare: feature/mi-funcionalidad
```

### Pull Request

1. El CI se ejecuta automáticamente en la PR.
2. Se requiere al menos 1 approval.
3. **Si el CI falla, no se puede mergear.**
4. El merge debe ser **Squash and merge** para mantener historial limpio.

### Preparar una release

```bash
# 1. Partir de develop
git checkout develop
git pull origin develop

# 2. Crear release
git checkout -b release/v1.2.0

# 3. Ajustes finales (versión, changelog, bugs menores)
git add .
git commit -m "chore(release): v1.2.0"

# 4. Publicar
git push -u origin release/v1.2.0

# 5. Abrir Pull Request hacia master y otra hacia develop
```

### Hotfix (urgencia en producción)

```bash
# 1. Partir de master
git checkout master
git pull origin master

# 2. Crear hotfix
git checkout -b hotfix/v1.2.1

# 3. Corregir y commit
git add .
git commit -m "fix(scope): descripción de la corrección"

# 4. Publicar
git push -u origin hotfix/v1.2.1

# 5. Abrir Pull Request hacia master y otra hacia develop
```

## Convención de commits

Usamos **Conventional Commits**:

```
<tipo>(<scope>): <descripción>

Tipos:
  feat:     Nueva funcionalidad
  fix:      Corrección de bug
  chore:    Tareas de mantenimiento (build, CI, dependencias)
  docs:     Documentación
  refactor: Refactorización sin cambio funcional
  test:     Tests
  style:    Cambios de formato (espacios, commas, etc.)
  perf:     Mejora de rendimiento
```

Ejemplos:
```
feat(auth): add Google OAuth callback endpoint
fix(profile): resolve image upload permission denied
chore(ci): update Docker build cache strategy
docs(readme): add deployment instructions
```

## CI

El pipeline se ejecuta automáticamente en:

| Evento | Ramas |
|---|---|
| Push | `master`, `develop`, `feature/*`, `release/*`, `hotfix/*` |
| Pull Request | Hacia `master`, `develop`, `release/*` |

- **Publicación de imágenes Docker** solo en push a `master`.
- Si el CI falla, el PR no puede mergearse (protegido por reglas de branch).

## Reglas de branch protection (GitHub)

Aplicar en Settings > Branches:

### master
- [x] Require pull request before merging
- [x] Require approvals (1)
- [x] Dismiss stale pull request approvals when new commits are pushed
- [x] Require status checks to pass before merging (CI)
- [x] Require branches to be up to date
- [x] Do not allow bypassing the above settings
- [x] Restrict pushes: solo admins (o nadie)

### develop
- [x] Require pull request before merging
- [x] Require approvals (1)
- [x] Require status checks to pass before merging (CI)
- [x] Restrict pushes