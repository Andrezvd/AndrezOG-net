# Arquitectura Global del Ecosistema AndrezOG

## Estructura de repositorios

El proyecto completo está dividido en 3 repositorios independientes:

### 1. andrezog-net

Implementación usando:

* ASP.NET Core
* Angular

Contiene:

* Backend .NET
* Frontend Angular
* Docker y despliegue específicos del stack Microsoft

---

### 2. andrezog-java

Implementación usando:

* Spring Boot
* Next.js

Contiene:

* Backend Java
* Frontend React/Next.js
* Docker y despliegue específicos del stack Java/Node

---

### 3. andrezog-core

Repositorio compartido entre ambos stacks.

NO es un backend ni frontend independiente.

Es la fuente de verdad compartida para:

* themes visuales
* contratos JSON
* assets
* prompts de IA
* reglas visuales
* configuraciones comunes
* documentación arquitectónica

---

# Regla crítica

Los stacks `andrezog-net` y `andrezog-java` deben comportarse funcionalmente iguales aunque estén implementados con tecnologías distintas.

La experiencia del usuario debe sentirse equivalente entre stacks.

---

# Uso obligatorio del core

Ningún stack debe redefinir localmente:

* themes
* contratos JSON
* prompts IA
* nombres de variables CSS globales

Toda configuración compartida debe originarse desde `andrezog-core`.

---

# Themes compartidos

Los themes viven en:

/themes

Ejemplo:

* cyberpunk.json
* minimal.json
* corporate.json

Ambos frontends deben interpretar exactamente la misma estructura JSON.

---

# Contrato de estilos IA

El endpoint:

POST /api/chat-style

debe devolver siempre:

{
"styles": {
"--bg-color": "#000000",
"--text-color": "#ffffff"
}
}

Las claves CSS válidas son definidas únicamente en `andrezog-core/contracts`.

No inventar nuevas variables CSS sin agregarlas primero al core.

---

# Objetivo arquitectónico

Este proyecto NO busca crear dos aplicaciones distintas.

Busca demostrar:

* arquitectura multi-stack
* consistencia cross-platform
* adaptabilidad tecnológica
* separación entre lógica compartida e implementación específica

---

# Restricciones para agentes IA

Cuando trabajes en un stack:

* NO asumir que los themes son locales.
* NO duplicar configuraciones compartidas.
* NO modificar contratos JSON unilateralmente.
* Consultar primero `andrezog-core` antes de crear nuevas estructuras globales.
* Mantener compatibilidad visual y funcional entre stacks.

---

# Reglas de comportamiento para agentes IA

Este proyecto tiene un **propósito principal de aprendizaje**.

Todo agente IA que interactúe con este repositorio **debe leer y cumplir** las reglas definidas en `AGENTS.md` (ubicado en la raíz de este workspace).

Las reglas incluyen:

* Enfoque enseñanza → práctica (no resolver por ti)
* PROHIBIDO implementar módulos completos sin autorización explícita
* Formato didáctico obligatorio (explicar antes de codificar)
* Tú tienes el control del ritmo y las decisiones

---

# Filosofía del proyecto

El usuario debe percibir:

"Es el mismo producto, reinterpretado en diferentes stacks."

NO:

"Son dos proyectos separados."
