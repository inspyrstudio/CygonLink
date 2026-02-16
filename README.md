
## SCP USDA Importer & Reloader for Unity ##
A custom Unity toolset designed for seamless integration and live-synchronization of USDA (Universal Scene Description) files exported from Cygon.

[](https://github.com/inspyrstudio/CygonUnityImporter/blob/main/README.md#scp-usda-importer--reloader-for-unity-a-custom-unity-toolset-designed-for-seamless-integration-and-live-synchronization-of-usda-universal-scene-description-files-exported-from-cygon)

**1. Overview**  This package provides a robust pipeline for bringing  `USDA`  assets into Unity. It includes a custom  `Scripted Importer`  to handle the conversion of  `USD`  data into Unity Prefabs and a Live Reloader to ensure your scene updates instantly when the source file changes.

_Core Features Automated_  `Prefab`  Generation: Converts  `USDA`  hierarchies,  `meshes`, and  `materials`  directly into Unity-native  `GameObjects`.

_Live Hot-Reloading_: Detects file saves in  _**Cygon**_  and automatically updates all instances in your active Unity scene.

_Intelligent Mesh Processing_: Includes a custom "`Weld Vertices`" pass and normal-correction logic to prevent dark artifacts and shadow leaks on corners.

_Material Library_: Automatically generates and manages physical materials in a local /materials folder, supporting  `Standard`,  `URP`, and  `HDRP`  shaders.

**2. Usage Guide**  
*The first time* you want to import you cygon file, simply drag a  `.usda`  file and its associated textures and models folders into your Unity Project window.

*The importer* : will create a sub-folder named materials to store the generated .mat files.

*Drag the imported*  `USDA`  asset from the Project window into your Scene.

*Live Editing* Keep your Unity Scene open even in play mode.

*Open the source* file in  _**Cygon**_.

*Modify* the geometry or transforms and Save.

*Switch back to Unity*; the ScpLiveReloader will trigger an orange log message in the console, and your scene objects will update instantly.
