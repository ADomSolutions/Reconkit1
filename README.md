
# ReconKit DEMO (PC) 🧠🔐 - DOMENICO ALEJANDRO

[![C#](https://img.shields.io/badge/C%23-language-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Windows Forms](https://img.shields.io/badge/Windows%20Forms-Desktop-0078D6?logo=windows&logoColor=white)](https://learn.microsoft.com/dotnet/desktop/winforms/)
[![SQLite](https://img.shields.io/badge/SQLite-DB-003B57?logo=sqlite&logoColor=white)](https://www.sqlite.org/index.html)
[![NuGet](https://img.shields.io/badge/NuGet-System.Data.SQLite-004880?logo=nuget&logoColor=white)](https://www.nuget.org/packages/System.Data.SQLite.Core/)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/framework/)
[![Windows](https://img.shields.io/badge/Windows-Only-0078D6?logo=windows&logoColor=white)](https://www.microsoft.com/windows/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)

---

## 📖 Descripción

**ReconKit (PC)** es una app de escritorio en **C# (WinForms)** para la **administración de credenciales** y **gestión de usuarios** de un kit de reconocimiento para PC.  
La versión actual incluye:

- Módulo **Administradores** (CRUD con permisos por nivel).
- Módulo **Usuarios** (editor con generación automática de **ID de familia**).
- Persistencia en **SQLite** (archivo único, portable).
- Control de permisos:
  - **Nivel 3**: agregar/eliminar administradores.
  - **Nivel ≥2**: editar.
  - **Nivel <2**: solo lectura.

> Las pantallas principales se encuentran en `ReconKitApp/AdminsForm.cs` y `ReconKitApp/UserEditorForm.cs`.

---

## 🖼️ Capturas (demo)

![3](https://github.com/user-attachments/assets/ada7b56b-6e90-49ef-81e0-a4a56523f2da)
![2](https://github.com/user-attachments/assets/d243ec22-1aff-4b0a-9e1d-4b61c7b0f4f6)
![1](https://github.com/user-attachments/assets/267a313d-8f70-4d34-8453-a9ec9ba834d9)


- **Panel principal**  
  `![Panel principal](docs/img/admin_main.png)`
- **CRUD Administradores**  
  `![CRUD Admins](docs/img/crud_admins.png)`
- **Editor de Usuarios**  
  `![Editor Usuarios](docs/img/user_editor.png)`

---

## ⚙️ Tecnologías

- **C# + .NET Framework 4.8**
- **Windows Forms**
- **SQLite** (`System.Data.SQLite.Core`)
- **Visual Studio 2022`

---

## 🚀 Cómo ejecutar

1. Cloná el repo:
   ```bash
   git clone https://github.com/tuusuario/reconkit-pc.git
   cd reconkit-pc
2. Abrí la solución en Visual Studio 2022.

3. Instalá los paquetes NuGet requeridos:
Install-Package System.Data.SQLite.Core
Install-Package System.Data.SQLite -Version 1.0.118  # (opcional: meta paquete)

4.Compilá y ejecutá. La app creará la base de datos si no existe (ver sección Base de datos).

🗄️ Base de datos

La app usa un archivo SQLite (por ejemplo data/reconkit.db).
Si preferís crear la base manualmente o para CI/CD, usá este esquema:
-- admins: credenciales y niveles
CREATE TABLE IF NOT EXISTS admins (
  id        INTEGER PRIMARY KEY AUTOINCREMENT,
  username  TEXT NOT NULL UNIQUE,
  level     INTEGER NOT NULL  -- 1=lectura, 2=edición, 3=admin total
);

-- users: datos maestros de usuarios del sistema
CREATE TABLE IF NOT EXISTS users (
  id          INTEGER PRIMARY KEY AUTOINCREMENT,
  dni         TEXT,
  apellido    TEXT,
  nombre      TEXT,
  id_familia  TEXT,
  fecha_nac   TEXT,   -- ISO yyyy-MM-dd
  direccion   TEXT,
  provincia   TEXT,
  localidad   TEXT,
  integrantes TEXT,
  notas       TEXT
);

-- admin por defecto (opcional)
INSERT OR IGNORE INTO admins (id, username, level) VALUES (1, 'admin', 3);
El método DataAccess.EnsureDatabase() se encarga de crear el archivo y las tablas si no existen, y puede sembrar un admin por defecto. Ajustá la ruta/semilla allí.

🔌 Cadena de conexión

En DataAccess.ConnectionString usá algo como:
// Ejemplo: ./data/reconkit.db (ruta relativa a la app)
public static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "reconkit.db");
public static string ConnectionString => $"Data Source={DbPath};Version=3;Foreign Keys=True;";
Recordá crear la carpeta data/ en el post-build si no existe.

🔑 Permisos (resumen)
Acción	Nivel requerido
Agregar administrador	3
Eliminar administrador	3
Editar administrador	≥ 2
Refrescar/Ver administradores	≥ 1
Editar usuario (alta/modif)	≥ 1 (según tu lógica)

La UI aplica esto en AdminsForm.ApplyPermissions():

btnAdd.Enabled = (currentLevel == 3)

btnEdit.Enabled = (currentLevel >= 2)

btnDelete.Enabled = (currentLevel == 3)

🧩 Funcionalidades clave
AdminsForm

Carga de grilla: SELECT id, username, level FROM admins

Alta/edición/baja con verificación de permisos

Confirmación de eliminación

Refresco de datos

UserEditorForm

Carga de registro por id si se edita

Guardado INSERT/UPDATE parametrizado

Generación de ID de familia: APELLIDO(5) + "+" + DNI + "+" + correlativo

Escanea existentes (LIKE base%) y calcula el siguiente número disponible.

🧪 Pruebas rápidas (datos de ejemplo)
INSERT INTO admins (username, level) VALUES ('operador', 2), ('visor', 1);

INSERT INTO users (dni, apellido, nombre, id_familia, fecha_nac, provincia, localidad, notas)
VALUES
('222222', 'PEREZ', 'JUAN', 'PEREZ+222222+1', '1990-05-01', 'BUENOS AIRES', 'TORTUGUITAS', 'VIVE SOLO'),
('333333', 'GOMEZ', 'ANA',  'GOMEZ+333333+1', '1988-03-15', 'BUENOS AIRES', 'SAN MARTIN', 'NINGUNA');

/src
  ReconKitApp.sln
  /ReconKitApp
    Program.cs
    DataAccess.cs
    AdminsForm.cs
    AdminEditorForm.cs
    UserEditorForm.cs
    /Properties
/data
  reconkit.db           # (generado en runtime si no existe)
/docs
  /img                  # capturas para el README
LICENSE
README.md

🔒 Notas de seguridad

No guardes contraseñas en texto plano (si agregás auth, usá hash con salt).

Restringí la escritura de la DB a carpetas de usuario (evitá Program Files).

Usá parámetros en SQL (ya lo hacés) para prevenir inyecciones.


🛠️ Roadmap

Autenticación de administradores con password (hash).

Registro de auditoría (quién editó qué y cuándo).

Búsqueda y filtros en CRUD.

Exportación CSV/Excel de usuarios.

Migración opcional a .NET 8 (Windows) con WinForms moderno.


📜 Licencia

Este proyecto se distribuye bajo licencia MIT.

🤝 Contribuciones

¡Se aceptan PRs! Abrí un issue para discutir nuevas features o bugs.
Formateo: EditorConfig/dotnet-format, estilos de commit tipo Conventional Commits si te copa.

