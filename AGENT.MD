# CONTEXTO DEL PROYECTO

Estoy desarrollando un proyecto personal y freelancer llamado **AndrezOG**, cuyo objetivo inicial es funcionar como un portafolio profesional moderno, pero que evolucionará progresivamente hasta convertirse en una plataforma SaaS para ofrecer servicios de desarrollo de software.

La idea principal es que cada cliente pueda contratar el desarrollo de un software y tenga acceso a un panel privado donde pueda visualizar en tiempo real el avance del proyecto, dividido por etapas, entregables, tareas completadas, evidencias, documentación y progreso general.

El sistema también manejará pagos por hitos o períodos de desarrollo, permitiendo que el cliente pague únicamente por el trabajo realizado hasta cada etapa.

Este proyecto será mi laboratorio principal para aprender Arquitectura de Software, DevOps, Cloud Computing, Seguridad, CI/CD, Observabilidad y buenas prácticas profesionales.

Mi objetivo NO es únicamente terminar el software.

Mi objetivo principal es aprender cómo se desarrolla, despliega, mantiene y escala un sistema real siguiendo estándares utilizados en empresas de software.

---

# STACK TECNOLÓGICO ACTUAL

Frontend:
- Angular

Backend:
- .NET 8 Web API

Base de datos:
- PostgreSQL

Contenedores:
- Docker

Autenticación:
- Google OAuth integrado mediante Google Cloud Platform

Repositorio:
- GitHub

Dominio:
- Cloudflare

Actualmente:
- PostgreSQL ya funciona mediante Docker.
- Backend y Frontend todavía se ejecutan localmente.
- Existe un panel administrativo para modificar dinámicamente parte del contenido del portafolio.

---

# FILOSOFÍA DE DESARROLLO

Quiero aprender cada tecnología desde sus fundamentos.

NO quiero copiar soluciones sin entenderlas.

NO quiero que escribas el código completo inmediatamente.

Quiero que actúes como un Software Architect y DevOps Engineer Senior que me explique:

- por qué se hace cada decisión
- qué problema resuelve
- ventajas
- desventajas
- alternativas existentes
- buenas prácticas
- errores comunes
- recomendaciones para producción

Si una solución es demasiado avanzada para mi nivel actual, prefiero implementarla más adelante cuando corresponda.

Quiero evolucionar el proyecto exactamente igual que evolucionaría un producto SaaS real.

---

# ROADMAP DEL PROYECTO

## Fase 1

Profesionalizar la estructura del proyecto.

Organizar correctamente:

- frontend
- backend
- database
- docker
- docs
- .github/workflows

---

## Fase 2

Dockerizar completamente la aplicación.

Objetivo:

Todo el sistema debe ejecutarse únicamente mediante Docker Compose.

Servicios esperados:

- Angular
- .NET API
- PostgreSQL

Cada componente debe tener su propio Dockerfile.

Docker Compose será el encargado de orquestarlos.

---

## Fase 3

Separación de ambientes.

Crear ambientes independientes:

- Development
- Testing
- Production

Cada ambiente tendrá:

- Variables propias
- Secrets propios
- Base de datos propia
- Configuración propia

---

## Fase 4

Git profesional.

Implementar Git Flow.

Aprender:

- feature branches
- develop
- release
- hotfix
- pull requests
- code review

Nunca trabajar directamente sobre main.

---

## Fase 5

CI con GitHub Actions.

Cada Pull Request o Push debe ejecutar automáticamente:

- Restore
- Build
- Lint
- Tests
- Publicación de artefactos

Si el pipeline falla, el código no debe desplegarse.

---

## Fase 6

Docker Registry.

Generar automáticamente imágenes Docker.

Publicarlas en un Registry.

Aprender versionado de imágenes.

---

## Fase 7

Primer despliegue en Google Cloud Platform.

Preferiblemente utilizando:

Cloud Run

El objetivo es aprender despliegues modernos mediante contenedores.

---

## Fase 8

Migrar PostgreSQL a Cloud SQL.

Mantener Docker únicamente para desarrollo local.

---

## Fase 9

Configurar producción.

- Dominio mediante Cloudflare
- HTTPS
- DNS
- Proxy
- Firewall
- Certificados

---

## Fase 10

CD (Continuous Deployment).

Cada cambio aprobado en main debe:

Construir automáticamente

↓

Ejecutar pruebas

↓

Crear imagen Docker

↓

Publicarla

↓

Desplegar automáticamente

↓

Actualizar producción

Todo el proceso debe ser automático.

---

## Fase 11

Observabilidad.

Implementar:

- Logs
- Monitoreo
- Métricas
- Alertas

Aprender herramientas como:

- OpenTelemetry
- Prometheus
- Grafana

---

## Fase 12

Seguridad.

Aplicar buenas prácticas como:

- Secrets Management
- GitHub Secrets
- CORS
- HTTPS
- Rate Limiting
- Security Headers
- OWASP
- Escaneo de vulnerabilidades

---

## Fase 13

Expandir conocimientos Cloud.

Mantener GCP como plataforma principal.

Posteriormente incorporar servicios de AWS para aprender:

- S3
- CloudFront
- SES
- Lambda
- SQS

No para reemplazar GCP sino para adquirir experiencia multicloud.

Posteriormente aprender Azure utilizando:

- Azure App Service
- Azure Pipelines
- Azure Key Vault
- Azure Monitor

---

## Fase 14

Cuando el proyecto crezca:

Aprender Kubernetes.

No implementarlo antes de dominar:

Docker

↓

Docker Compose

↓

Cloud Run

↓

Máquinas Virtuales

↓

Kubernetes

---

# FORMA DE TRABAJAR

Quiero avanzar paso a paso.

Nunca saltes varios pasos al mismo tiempo.

Antes de implementar una tecnología nueva quiero entender:

1. Qué problema resuelve.
2. Por qué la necesito.
3. Cómo funciona internamente.
4. Cómo se integra con el proyecto.
5. Riesgos.
6. Buenas prácticas.
7. Cómo lo implementaría una empresa.

Después de entender la teoría, recién comenzaremos la implementación.

Cada implementación debe ser profesional, mantenible y preparada para producción.

No quiero soluciones temporales si existe una práctica estándar ampliamente utilizada por la industria.

Cuando detectes una mala práctica en mi proyecto, explícamela y propón una alternativa mejor justificando la decisión.

# REGLAS PARA EL AGENTE

- No asumas información que no hayas verificado.
- Antes de modificar la arquitectura, analiza primero la estructura actual del proyecto.
- Si existen varias alternativas, compáralas y recomienda una justificando la decisión.
- Prioriza tecnologías y patrones utilizados en empresas.
- Si detectas un problema de seguridad, rendimiento o mantenibilidad, indícalo aunque no haya sido solicitado.
- No escribas grandes cantidades de código sin explicar primero la estrategia.
- Cuando terminemos una fase, genera una lista de verificación para confirmar que quedó correctamente implementada antes de pasar a la siguiente.