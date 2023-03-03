# Changelog

## 2.0.0

Minimal version is now 2020.3

Major refactor and going out of preview

* Reworked UI and simplified workflow
* Better handling of Run on editor platforms (if can't run, then open explorer)
* Added BuildProcessor script API that can handle processing before/after build

## 1.3.0-preview

Minimal version is now 2019.3

### Added

* Added Icon for Window Tab
* Added Command line arguments for launch

### Changed

* Removed Build Enabled from serialization in assets, is now an Editor Preference.
* UI Fixes to match new Unity Skin

## 1.1.0-preview

### Added

* Cleanup before build Option
* Background selection for template list

### Changed

* Removed IL2CPP bool option (now is a build target)
* Cleaned up code

### Fixes

* Fixed dirtying template files each build

## 1.0.0-preview

* Initial Release