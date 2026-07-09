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
    "com.jaimecamachodev.unityfolders": "1.8.2"
  }
}
```

También puedes hacerlo desde el Editor:
1. `Edit > Project Settings > Package Manager`.
2. En **Scoped Registries**, pulsa `+` y añade:
   - **Name:** `JaimeCamachoDevs`
   - **URL:** `https://registry.npmjs.org`
   - **Scope(s):** `com.jaimecamachodev`
3. Abre `Window > Package Manager`, cambia el desplegable a **My Registries** y busca **UnityFolders** para instalarla.

---

## 📂 Estructura Principal

```
Assets/
│
├── 1-Scripts/               → Scripts globales del proyecto (Managers, Systems, etc.)
├── 2-Art/                   → Todo el arte (3D, VFX, SFX, UI, etc.)
├── 3-Scenes/                → Escenas jugables (cargables)
├── 4-Presets/               → Presets de herramientas y assets
├── 5-Settings/              → Settings del proyecto (URP, Inputs, Tags, etc.)
├── Plugins/                 → Plugins externos o de la Asset Store
└── Packages/                → Packages de Unity Package Manager (Read-only)
```

---

## 📂 Detalle del Arte (`2-Art`)

```
2-Art/
│
├── 1-3D/                    → Modelos 3D organizados por tipo
│   ├── Characters/          → Personajes (Player, NPCs, etc.)
│   ├── Environment/         → Escenarios (por escena y globales)
│   │   ├── Scene_1_Forest/  → Assets específicos de la escena 1
│   │   └── Global/          → Assets comunes de entornos (Shared)
│
├── 2-VFX/                   → Efectos visuales globales (fuego, explosiones, etc.)
├── 3-SFX/                   → Sonidos globales
├── 4-Directors/             → Timeline, Playables, cinemáticas
├── 5-Skyboxes/              → Cielos y entornos espaciales
├── 6-Videos/                → Videos y cutscenes
├── 7-SolidMats/             → Materiales sólidos genéricos o tiles
├── 8-PostProcessing/        → Post-Process assets y perfiles
├── 9-UI/                    → Interfaces gráficas, fonts, prefabs de UI
└── 10-Lighting/             → Lightmaps, HDRIs, configuraciones de iluminación
```

---

## 📋 Reglas de Organización:
1. **Cada escena** tiene su propia carpeta en `Environment` para sus assets internos.
2. Assets que se usan en **múltiples escenas** van en las carpetas `Global` si corresponde.
3. Los nombres de las escenas siguen el formato:  
`Scene_01_NombrePropio` para orden automático y claridad.
4. Scripts globales siempre van en `1-Scripts` (nunca mezclados con arte).
5. Plugins y paquetes de terceros van en sus carpetas designadas (`Plugins` y `Packages`).

---

## 🎯 Ventajas:
- Escalable y robusta para equipos grandes o proyectos a largo plazo.
- Navegación rápida y predecible.
- Fácil integración con sistemas de control de versiones (Git).
- Compatible con automatización y builds profesionales.

---

> **NOTA:** Si introduces nuevas escenas o assets globales, asegúrate de seguir este esquema para mantener la consistencia.


