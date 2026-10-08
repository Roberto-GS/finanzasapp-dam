FINANZASAPP

Ver documentación técnica: [Documentación PDF ](<docs/Documentación Finanzas App.pdf>)

FinanzasApp es una aplicación de gestión de finanzas personales que nace con la idea de ofrecer a cualquier persona una herramienta sencilla pero completa para tener un control real de su dinero. La mayoría de personas no lleva un seguimiento ordenado de en qué gasta o cuánto ingresa cada mes, lo que dificulta tomar buenas decisiones económicas o cumplir objetivos de ahorro. FinanzasApp pretende resolver eso de forma accesible, sin necesidad de conocimientos de contabilidad ni de economía 
La aplicación permite registrar cada ingreso y gasto, organizarlos por categorías personalizadas, visualizar la distribución del gasto mediante gráficas, establecer objetivos financieros con seguimiento automático en tiempo real, y colaborar con otras personas a través de grupos compartidos que muestran un resumen financiero conjunto y un ranking de gastos entre los miembros
Técnicamente, la aplicación está formada por dos partes bien diferenciadas. Por un lado, una API REST desarrollada con ASP.NET Core 9 que actúa como backend, gestionando toda la lógica de negocio, la autenticación mediante tokens JWT y el acceso a la base de datos. Por otro, una interfaz de usuario multiplataforma desarrollada con .NET MAUI 9 que se ejecuta tanto en Android como en Windows desde una única base de código 
La decisión de separar el backend en una API independiente no es solo técnica, sino también práctica: permite que la aplicación funcione desde cualquier dispositivo con conexión a internet, ya que los datos están siempre sincronizados en la nube. La API se despliega de forma independiente en Railway mediante Docker, y la base de datos MySQL está alojada en Aiven, un servicio gestionado en la nube que garantiza disponibilidad, copias de seguridad automáticas y conexiones cifradas mediante SSL 
La arquitectura del proyecto está organizada en cuatro capas bien separadas (Core, Infrastructure, API y UI), siguiendo principios de diseño que hacen el sistema mantenible, escalable y fácil de extender en el futuro sin necesidad de reescribir partes ya funcionales.

<img width="540" height="400" alt="{F20C8B69-294C-4F06-BDD9-DAB273732708}" src="https://github.com/user-attachments/assets/69880e24-3061-4ca8-88e0-ad8afbc3d333" />
<img width="470" height="400" alt="{FC1A120E-F9FE-4B1A-998B-1A6315044817}" src="https://github.com/user-attachments/assets/d8dc326c-cbbf-4c54-908a-100ddaa6e652" />


ARQUITECTURA
El proyecto sigue el patrón Clean Architecture, organizado en 4 capas con dependencias en una sola dirección:
- Core: modelos de dominio, DTOs, interfaces de repositorio y enums. No depende de nada.
- Infrastructure: implementación de los repositorios con Entity Framework Core, mapeos con AutoMapper.
- API: controladores REST, autenticación JWT, middleware de gestión de errores.
- UI: cliente .NET MAUI con patrón MVVM

TECNOLOGÍAS
- Herramientas de Desarrollo
  · Visual Studio 2022: El IDE
- Infraestructura
  · Aiven: EL Servicio de MySQL en la nube para la base de datos
  · Railway: Es la Plataforma de Despliegue del backend mediante el contenedor Docker
  · Docker: Es para la contenedorización de la API.
  · GitHub: Para el control de versiones y como repositorio del código fuente
- Backend
  · .Net 9: Es el Framework para el desarrollo
  · ASP.NET Core Web API: Para la construcción de los endpoints REST
  · Entity Framework Core 9: ORM para el acceso a la base de datos
  · Pomelo.EntityFrameworkCore.MySQL 9.0.0: Proveedor de EF Core para MySQL
  · MySQL 8: Es el motor de la base de datos relacional
  · JWT (JSON Web Tokens): Para la autentificación y la autorización
  · BCrypt: Para el hash de las contraseñas
  · AutoMapper: Para el mapeo entre las entidades y DTOs
  · Swagger: Para las pruebas de la API
- Frontend
  · .NET MAUI 9: Es el Framework multiplataforma para Android y Windows
  · CommunityToolkit.Mvvm:  Para la implementación de los MVVM
  · CommunityToolkit.Maui: Los controles y las utilidades adicionales para MAUI
  · Microcharts.Maui: Para las gráficas
  · SkiaSharp: Es el motor de renderizado 2D usado internamente por Microcharts

QUE APRENDÍ
- A estructurar un proyecto en capas separadas por responsabilidad, de forma que un cambio en la base de datos o el ORM no afecte a la lógica de negocio
- A implementar autenticación segura de extremo a extremo (JWT + hash de contraseñas con BCrypt)
- A desplegar una API en producción con Docker, resolviendo problemas reales como la gestión del puerto dinámico de Railway y la configuración segura de variables de entorno
