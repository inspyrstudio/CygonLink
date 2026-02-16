# Changelog

All notable changes to this project will be documented in this file.

## [0.1.0] - 2024-05-20

### Added

- Initial release of the Cygon Unity Importer.
- Support for USDA file importing.

### Changed

- The minimum supported editor version is now **Unity 6000.1.4f1**. Compatibility with versions before this is not guaranteed.

### Fixed

- Textures and materials are now created in the correct sub-folders upon import.

## [0.1.1-preview] - 2024-05-20

### Added ###

- Better logs with color and informations display.
- Tool for manual refresh in _[Tools/Cygon (UCF)/Force Refresh]_, available with `CTRL + ALT + R`

### Changed ###

- Renamed scripts and assemblies definitions

### Fixed ###

- `RefreshAll` method was not refreshing correctly with the shortcut

## [0.1.2-preview] - 2024-05-20

### Added ###

- EditorRuntime_USDA, a static class with information for custom logs.

### Fixed ###

- `Cygon Link` name was not the same everywhere.
