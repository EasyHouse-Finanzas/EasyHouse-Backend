#  EasyHouse – Backend API (.NET)

API REST desarrollada en **C# / .NET** destinada a procesar los cálculos financieros necesarios para simular créditos hipotecarios bajo el método francés. El backend implementa todas las fórmulas, tasas, periodos de gracia y parámetros del sistema financiero peruano, garantizando precisión y transparencia, de acuerdo con los lineamientos de la SBS.

Esta API es consumida por el frontend de EasyHouse, diseñado para empresas inmobiliarias.

---

## Introducción

El sistema financiero peruano exige cálculos estandarizados y transparentes para créditos hipotecarios, especialmente en programas como **MiVivienda** y el **Bono Techo Propio**. Este backend implementa todas las reglas y algoritmos necesarios para simular créditos de manera profesional y confiable.

---

##  Objetivo del Backend

Ofrecer servicios web que permitan:

- Calcular créditos hipotecarios bajo el método francés.
- Determinar intereses, amortización y saldo por periodo.
- Aplicar periodos de gracia total o parcial.
- Convertir tasas nominales/efectivas.
- Calcular **VAN** y **TIR**.
- Integrar el **Bono Techo Propio** cuando corresponda.

---

## Tecnologías Utilizadas

- **.NET 7+ / .NET 8**
- **C# 12**
- **Entity Framework Core**
- **SQL Server / LocalDB**
- **Swagger / Swashbuckle**
- **AutoMapper**
- **Dependency Injection**
- **CORS habilitado para Angular**

---

##  Funcionalidades de la API

###  Simulación de créditos
- Método francés (cuotas constantes)
- Interés, amortización y saldo
- Escenarios con y sin periodo de gracia

###  Manejo financiero completo
- Cálculo de TEA y TNA
- Conversión automática de tasas
- Cálculo del Bono Techo Propio

###  Indicadores y análisis
- **Valor Actual Neto (VAN)**
- **Tasa Interna de Retorno (TIR)**

###  Documentación
Swagger habilitado para pruebas rápidas.

---

##  Endpoints de la API

La API sigue el estándar REST y utiliza el prefijo `/api/v1`. A continuación se detallan los recursos disponibles:

###  Autenticación (IAM)
Gestión de identidad y acceso seguro.

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `POST` | `/api/v1/iam/auth/signup` | Registro de nuevos usuarios (Agentes). |
| `POST` | `/api/v1/iam/auth/signin` | Inicio de sesión y obtención de Token JWT. |

###  Core Financiero (Simulaciones y Reportes)
El corazón de la aplicación donde se procesa la lógica financiera.

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `POST` | `/api/v1/simulations` | **Crear nueva simulación**: Calcula el cronograma (Método Francés), VAN, TIR y guarda el resultado. |
| `GET` | `/api/v1/simulations` | Listar todas las simulaciones realizadas. |
| `GET` | `/api/v1/simulations/{id}` | Obtener el detalle de una simulación específica. |
| `POST` | `/api/v1/reports/{simulationId}` | Generar reporte financiero detallado basado en una simulación existente. |

###  Gestión de Clientes
Administración de la información de los potenciales compradores.

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/v1/clients` | Listar clientes registrados. |
| `POST` | `/api/v1/clients` | Registrar un nuevo cliente. |
| `GET` | `/api/v1/clients/{id}` | Obtener datos de un cliente. |
| `PUT` | `/api/v1/clients/{id}` | Actualizar datos del cliente. |
| `DELETE` | `/api/v1/clients/{id}` | Eliminar cliente. |

###  Gestión de Inmuebles (Houses)
Catálogo de propiedades disponibles para financiamiento.

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/v1/houses` | Listar inmuebles disponibles. |
| `POST` | `/api/v1/houses` | Registrar un nuevo inmueble. |
| `GET` | `/api/v1/houses/{id}` | Ver detalles de un inmueble. |
| `PUT` | `/api/v1/houses/{id}` | Actualizar información del inmueble. |
| `DELETE` | `/api/v1/houses/{id}` | Dar de baja un inmueble. |

###  Configuración (Configs)
Parámetros globales (Tasas referenciales, límites, monedas).

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/v1/configs` | Ver configuraciones actuales. |
| `POST` | `/api/v1/configs` | Crear nueva configuración financiera. |
| `PUT` | `/api/v1/configs/{id}` | Modificar parámetros de configuración. |

---

##  Ejecución del Proyecto

Sigue estos pasos para levantar el servidor localmente:

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/EasyHouse-Finanzas/EasyHouse-Backend.git](https://github.com/EasyHouse-Finanzas/EasyHouse-Backend.git)
    ```

2.  **Configurar Base de Datos:**
    Asegúrate de tener SQL Server corriendo y actualiza la cadena de conexión en `appsettings.json`.

3.  **Restaurar dependencias y base de datos:**
    ```bash
    dotnet restore
    dotnet ef database update
    ```

4.  **Ejecutar la API:**
    ```bash
    dotnet run
    ```
    La API estará disponible (por defecto) en `https://localhost:7239` o el puerto configurado.

5.  **Ver Documentación:**
    Navega a la interfaz de Swagger para probar los endpoints:
    > `https://localhost:7239/swagger/index.html`

---
