# CHANGELOG

## 2021 - 11 - 24 - First 2021.2 Version

**Version Tag** : `2021.2.0`

* Updated Unity to 2021.2.1f1
* Updated HDRP/VFX to 12.1.1
* **Fixes and Changes:**
  * Added Support for Upsampling methods as options
    * NVidia DLSS (Windows Only)
    * AMD FSR
    * Temporal AA Upsampling
    * Contrast-Adaptive Sharpen
    * Catmull-Rom
  * Added Lensflares through the walkthrough
* **Known Issues:**
  * Particle Depth Collisions not working properly when rendering at < 100%
  * Jittering of the FPS Reticula Crosshair

## 2021 - 11 - 17 - LTS 2020.3 Version

**Version Tag** : `2020.3.0`

* Updated Unity to 2020.3.19f1
* Updated HDRP/VFX to 10.6.0

* **Fixes and Changes:**
  * Added Quality Settings : Low & Ultra
  * Updated Content to optimize/enhance based on quality settings
  * Updated Terminal Room to show outer space through a window
  * Updated Benchmark Mode to Display FPS Counter, and a Benchmark Report at the end
  * Benchmark also saves a HTML reports in Documents/SpaceshipDemo folder
  * VFX / Lighting polishing.

## 2021 - 02 - 17 - LTS 2020.2 Version

**Version Tag** : `2020.2.0`

* Updated Unity to 2020.2.4f1
* Updated HDRP/VFX to 10.3.1

* **Fixes and Changes:**
  * Rebuilt Lighting
  * Rebuilt VFX Graphs
  * Fixed Many Errors and Warnings
  * Fixed Animated Light Cookes (CustomRenderTexture)
  * Block Player input while in pause/toggle console

## 2021 - 02 - 08 - 2020.1 Update

**Version Tag** : `2020.1.1`

* Updated Unity to 2020.1.17f1
* Updated HDRP/VFX to 8.3.1

* **Fixes and Changes:**
  * Rebuilt Lighting
  * Rebuilt VFX Graphs
  * Fixed Lights with broken Cookie textures

## 2020 - 08 - 06 - 2020.1 Version

**Version Tag** : `2020.1.0`

* Updated Unity to 2020.1.0f1
* **Fixes and Changes:**
  * Rebuilt Lighting
  * Rebuilt VFX Graphs

## 2020 - 08 - 06 - LTS 2019.4 Version

**Version Tag** : `2019.4.0`

* Updated Unity to 2019.4.3f1
* Updated Timeline Package to 1.4.1
* Added Benchmark Mode (Flyby Camera)
* **Fixes and Changes:**
  * Rebuilt VFX Graphs
  * Small Fixes for macOS

## 2020 - 06 - 08 - Package Upgrade

**Version Tag** : `2019.3.3`

* Updated Unity to 2019.3.15f1
* Updated HD Render Pipeline / VFX Graph to package `7.4.1`
* Updated Timeline Package to `1.3.0`
* Removed references to many unnecessary built-in modules
* Removed reference to Recorder Package
* **Fixes and Changes:**
  * Rebuilt VFX Graphs
  * Fixes Area Light meshes that were broken (always visible) while using `7.3.1`
  * Updated Gameplay Ingredients

## 2020 - 02 - 03 - Content Update

**Version Tag** : `2019.3.2`

* Updated Unity to 2019.3.2f1
* Updated HD Render Pipeline / VFX Graph to package `7.2.1`
* **Fixes and Changes:**
  * Major overhaul on Visual Effect Graphs, commented and removed workarounds
  * Work on physic assets (barrels, hanging cables)
  * Added more destruction during the attack / wrecked scene
  * Main Menu Postprocess and Glitch ornaments
  * Added Feature Sample Scenes
  * Enabled Editor Quick play mode (disabled Domain Reload on Play in Editor)
  * Memory improvements (Audio, fixed planar reflection huge VRAM allocation)
  * Better handling of the pause (pauses also sound and subtitles)
  * Re-enabled Attack music
  * Various tweaks and fixes

## 2020 - 02 - 12 - Update to HDRP/VFX 7.2.0

**Version Tag** : `2019.3.1`

* Updated HD Render Pipeline / VFX Graph to package `7.2.0`
* **Fixes and Changes:**
  * Removed UnityBall Duplicate FBX from project (is now available in HDRP package)
  * Removed Obsolete Settings Manager (prevented start in native resolution)
  * Updated Console Package

## 2020 - 01 - 30 - Updated project to Unity 2019.3

**Version Tag** : `2019.3.0`

* Updated all project to **Unity 2019.3.0f6**
* Updated HD Render Pipeline / VFX Graph to package `7.1.8`

**Known Issues:**

* HDRP renders pitch-black on Linux Vulkan
* Post-Processing Issues on macOS metal

## 2019 - 08 - 07 - Initial Release

**Version Tag :** `2019.2.0`

* Initial Project Release
* Requires Editor : **[Unity 2019.2.0f1](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe)**
* Uses HD Render Pipeline `6.9.1-preview`

**Known Issues:**

* macOS build has trouble rendering SSAO
* Visual Effect Issues with macOS/metal
* HDRP renders pitch-black on Linux Vulkan

