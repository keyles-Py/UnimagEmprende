# UnimagEmprende
# Sistema de Gestión de Eventos - Proyecto Integrador DevOps

Este repositorio constituye la base técnica y metodológica del Sistema Integrador desarrollado para el curso. El proyecto se fundamenta en un enfoque de desarrollo evolutivo e incremental, donde la calidad del software reside en la trazabilidad, el orden técnico y la disciplina en el control de versiones.

## 1. Conformación del Equipo

Se han establecido roles específicos para esta fase inicial con el objetivo de organizar el trabajo técnico, distribuir responsabilidades y asegurar que cada incremento del sistema sea supervisado y validado de manera independiente.

| Nombre Completo | Rol / Cargo | Especialidad | Correo electrónico |
| :--- | :--- | :--- | :--- |
| Daniel Bonnett | Lead Architect & DB Manager | Backend | dbonnet@unimagdalena.edu.co |
| Jhon Galofre | Security & Auth Specialist | Backend | jgalofre@unimagdalena.edu.co |
| Keyner Mendoza | DevOps & Integration Lead | Frontend | kdmendoza@unimagdalena.edu.co |
| Armando Hernandez | UI/UX & API Consumer | Frontend | [Insertar Correo] |

## 2. Definición del Stack Tecnológico

El conjunto de herramientas seleccionado busca garantizar la escalabilidad del sistema y su capacidad para integrar nuevos módulos de forma progresiva, cumpliendo con los requisitos de estabilidad y soporte a largo plazo.

| Categoría | Tecnología Elegida | Versión | Justificación Técnica |
| :--- | :--- | :--- | :--- |
| Lenguaje Backend | C# | 12 | Lenguaje robusto con tipado fuerte que previene errores en sistemas de crecimiento incremental. |
| Framework Backend | .NET CORE | 10 | Infraestructura de alto rendimiento para la creación de APIs escalables y servicios externos. |
| Base de Datos | Postgres | 16 | Motor relacional que asegura la integridad de los datos y la persistencia desde el núcleo inicial. |
| Lenguaje Frontend | Typescript | 5.4 | Implementa tipado estático en el cliente, facilitando el mantenimiento y la refactorización. |
| Framework Frontend | Next JS | 14 | Permite una arquitectura modular y organizada, optimizando el renderizado y la escalabilidad. |
| Autenticación | JWT | Estándar | Estándar para el manejo de sesiones de forma segura y desacoplada del servidor. |
| Control de Versiones | Git | 2.44+ | Herramienta base para la metodología GitFlow y la trazabilidad del código. |
| Infraestructura | Azure | Nube | Entorno para el despliegue de servicios gestionados y bases de datos. |
| Otros (CI/CD) | Github Actions | Nativo | Automatización de la integración y entrega continua para validar cada incremento técnico. |

## 3. Metodología de Desarrollo (GitFlow)

El proyecto se gestiona bajo el flujo de trabajo GitFlow, lo que permite un desarrollo colaborativo ordenado y protege la estabilidad del código principal a través de un sistema de ramas especializado.

* **main**: Contiene la versión estable y productiva del sistema.
* **develop**: Rama de integración donde se consolidan las funcionalidades terminadas antes de su paso a producción.
* **feature/**: Ramas independientes creadas para cada ítem del backlog (PBI), evitando el trabajo directo sobre las ramas de integración y facilitando la revisión de código.

## 4. Alcance de la Sesión Inicial: Usuarios y Acceso

De acuerdo con los requerimientos habilitados para la primera fase, el equipo se concentra en la construcción del núcleo de identidad y control de acceso:

* Modelado de la entidad de usuario y definición de roles iniciales.
* Configuración de la persistencia de datos mediante PostgreSQL.
* Implementación de procesos de registro y autenticación con manejo seguro de credenciales.
* Validación de reglas de negocio, incluyendo la unicidad del correo electrónico.

## 5. Gestión del Proyecto (Backlog)

El seguimiento de las tareas se realiza a través de **GitHub Projects**, donde cada requerimiento se descompone en Product Backlog Items (PBIs). Cada PBI cuenta con criterios de aceptación definidos para asegurar la calidad de la entrega y permitir la validación técnica por parte del equipo.
