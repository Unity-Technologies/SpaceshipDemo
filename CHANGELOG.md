# CHANGELOG

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

