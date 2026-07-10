<header>

![image](https://github.com/user-attachments/assets/95f4cd3a-c3be-4896-9a63-026bd422bce4)
![image](https://github.com/user-attachments/assets/49dd560e-5dc5-45c8-8b8b-68f27da00cd8)

# **UnityFolders**

_**Carpetas personalizadas para Unity**_


# 🎮 Proyecto Unity - Estructura de Carpetas

Esta es la estructura oficial del proyecto, diseñada para maximizar claridad, escalabilidad y eficiencia.

---

## 📦 Instalación (VzFolders vía Package Manager)

Esta tool (`VzFolders`) se distribuye como paquete de Unity Package Manager con el id `com.jaimecamachodev.unityfolders`, publicado en npm.

Para instalarla en **otro proyecto de Unity**, añade un scoped registry a tu `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "JaimeCamachoDevs",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "com.jaimecamachodev"
      ]
    }
  ],
  "dependencies": {
    "com.jaimecamachodev.unityfolders": "2.1.0"
  }
}
```

También puedes hacerlo desde el Editor:
1. `Edit > Project Settings > Package Manager`.
2. En **Scoped Registries**, pulsa `+` y añade:
   - **Name:** `JaimeCamachoDevs`
   - **URL:** `https://registry.npmjs.org`
   - **Scope(s):** `com.jaimecamachodev`
3. Abre `Window > Package Manager`, cambia el desplegable a **My Registries** y busca **VzFolders** para instalarla.

> A partir de la versión `2.0.0` el paquete distribuye la herramienta **VzFolders** (reescritura completa que sustituye al sistema anterior de iconos de carpetas).

---

## 📂 Estructura Principal

```
Assets/
│
├── 1-Programming/           → Scripts globales, prefabs de lógica y materiales sólidos
│   ├── Observers/
│   ├── Prefabs/
│   ├── Scripts/
│   └── SolidMats/
├── 2-Art/                   → Todo el arte (3D, VFX, SFX, UI, etc.)
├── 3-Scenes/                → Escenas jugables (cargables)
├── 4-Presets/               → Presets de herramientas y assets
└── 5-Settings/              → Settings del proyecto (URP, Inputs, Tags, etc.)
```

---

## 📂 Detalle del Arte (`2-Art`)

```
2-Art/
│
├── 1-3D/                    → Modelos 3D organizados por tipo
│   ├── Animals/
│   ├── Characters/
│   └── Environments/
│
├── 2-VFX/                   → Efectos visuales globales (fuego, explosiones, etc.)
├── 3-SFX/                   → Sonidos globales
├── 4-Directors/             → Timeline, Playables, cinemáticas
├── 5-Skyboxes/              → Cielos y entornos espaciales
├── 6-Videos/                → Videos y cutscenes
├── 7-PostProcessing/        → Post-Process assets y perfiles
└── 8-UI/                    → Interfaces gráficas, fonts, prefabs de UI
```

Esta estructura se puede generar automáticamente con **VzFolders**: `Tools > JaimeCamachoDev > VzFolders > Project structure > Create base project structure` (o clic derecho en `Assets` > `VzFolders > Create base project structure`).

---

## 🧹 Herramientas de organización de assets (VzFolders)

Además de la navegación con paletas de color, VzFolders incluye un pipeline para que las carpetas de arte no se conviertan en un caos cuando varios artistas dejan texturas, mallas y prefabs mezclados. Todo vive bajo `Tools > JaimeCamachoDev > VzFolders > ...` y también como clic derecho en `Assets > VzFolders > ...`:

- **Project structure > Create base project structure** — crea el árbol de carpetas de arriba.
- **Project structure > Create asset type folders...** — pide un nombre y crea, dentro de la carpeta seleccionada, las subcarpetas por tipo: `Animation`, `Audio`, `Material`, `Mesh`, `Prefab`, `Script`, `Shader`, `VFX`.
- **Organize > Organize this folder** — mueve, en el sitio, cada asset suelto de la carpeta seleccionada a su subcarpeta de tipo (creándolas si hace falta) y borra las subcarpetas que queden vacías.
- **Organize > Organize into new subfolder...** — igual que lo anterior, pero primero envuelve todo el contenido en una subcarpeta nueva con el nombre que indiques.
- **Organize > Rename assets with folder prefix** — renombra todos los assets de una carpeta con el prefijo del nombre de la carpeta (p. ej. `Rock01_Diffuse.png`).
- **Ingest pipeline > Create materials (MAS / Lit)** — genera materiales URP a partir de texturas con sufijos `_ColorAlpha`, `_Normal`, `_MetalSmooth`/`_MaskMap`, `_AO`/`_Emission`, y los asigna automáticamente a los modelos importados de la misma carpeta.
- **Ingest pipeline > Create prefabs from Mesh folder** — crea un prefab por cada malla dentro de la subcarpeta `Mesh`.
- **Ingest pipeline > Full pipeline...** — asistente que encadena, de forma configurable, la creación de materiales, el volcado a una subcarpeta nueva, el renombrado y la creación de prefabs en un solo paso.

---

## 🎯 Ventajas:
- Escalable y robusta para equipos grandes o proyectos a largo plazo.
- Navegación rápida y predecible.
- Fácil integración con sistemas de control de versiones (Git).
- Compatible con automatización y builds profesionales.

---

> **NOTA:** Si introduces nuevas escenas o assets globales, asegúrate de seguir este esquema para mantener la consistencia.


