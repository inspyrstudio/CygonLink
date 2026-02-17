

# Cygon Link

Seamless integration and live-sync for USDA files.

Cygon Link is a powerful bridge that provides a robust pipeline for bringing USDA (Universal Scene Description) assets into Unity. It features a custom Scripted Importer for automated conversion and a Live Reloader that syncs changes from Cygon to Unity in real-time.


## Key Features


*📦 Automated Prefab Generation* : Converts USDA hierarchies, meshes, and materials directly into native Unity GameObjects with collision.

*🔥 Live Hot-Reloading* : Detects file saves in Cygon and instantly updates all instances in your active Unity scene (even in Play Mode).

*🛠️ Intelligent Mesh Processing* : Includes a "Weld Vertices" pass and normal-correction logic to eliminate dark artifacts and shadow leaks.

*🎨 Material Management* : Automatically generates materials in a local /Materials folder with support for any Graphics Pipeline.


## Getting Started

*Installation via Git URL*
- Open the Unity Package Manager (Window > Package Manager).
- Click the + button and select Add package from git URL...
- Paste the following URL:  
  https://github.com/inspyrstudio/CygonLink.git

*How to use it ?*
1. Import your Assets  
   Drag your .usda file along with its associated textures and models folders into the Unity Project window.  
   Note: The importer will automatically create a /Materials sub-folder to store generated .mat files.

2. Add to Scene  
   Drag the imported USDA asset from the Project window into your Hierarchy or Scene View.

3. Live Editing Workflow  
   Keep your Unity Scene open (works in both Edit and Play Mode).  
   Open the source file in Cygon.  
   Modify geometry or transforms and Export or Quick Export with CTRL + S.

4. Switch back to Unity : * the auto sync will trigger an orange log in the console, and your objects will
   update instantly.

## Miscellaneous

*⚙️ Requirements*
- Unity Version: 6000.1.4f1 or higher.  `(Note, the package is not garanteed to work before 6000.1.4f1 but it's up to you if you want to test)`
- Cygon Version 0.2.0.7i minimum. `(Note, this time the version of Cygon needed is mandatory to be at least 0.2.0.7i, before this one, exportation was not meant for unity importation and will not work)`

*🤝 Contributing*  

Contributions are welcome! Please feel free to tell us about anything that doesnt work with the package on the Discord.




