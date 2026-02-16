using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public class RuntimeSync_USDA
{
    private static FileSystemWatcher projectWatcher;

    static RuntimeSync_USDA()
    {
        // Initialize watcher for the Assets folder
        string assetsPath = Application.dataPath;
        projectWatcher = new FileSystemWatcher(assetsPath);
        
        projectWatcher.IncludeSubdirectories = true;
        projectWatcher.Filter = "*.usda";
        projectWatcher.NotifyFilter = NotifyFilters.LastWrite;

        // Hook into the change event
        projectWatcher.Changed += OnUsdaFileChanged;
        projectWatcher.EnableRaisingEvents = true;
        
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        Debug.Log("<b>Cygon (UCF)</b> <color=white>Waiting for changes in sources files</color>");
    }
    
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        RefreshAll();
    }

    private static void OnUsdaFileChanged(object sender, FileSystemEventArgs e)
    {
        // FileSystemWatcher runs on a background thread. 
        // We must move back to the Main Thread for Unity API calls.
        EditorApplication.delayCall += () => 
        {
            RefreshSceneInstances(e.FullPath);
        };
    }
    
    [MenuItem("Tools/Cygon (UCF)/Force Refresh %&r", false, 10)]
    public static void ManualRefreshAll()
    {
        Debug.Log("<b>Cygon (UCF)</b> <color=orange>Manual Refresh Triggered...</color>");
        RefreshAll();
    }
    private static void RefreshAll()
    {
        string[] guids = AssetDatabase.FindAssets("t:DefaultAsset");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.ToLower().EndsWith(".usda")) RefreshSceneInstances(Path.GetFullPath(path));
        }
    }

    private static void RefreshSceneInstances(string fullPath)
    {
        // Fix: Convert System Path to Unity Project Path correctly
        string assetPath = fullPath.Replace("\\", "/"); // Normalize slashes
        string dataPath = Application.dataPath.Replace("\\", "/");

        if (assetPath.StartsWith(dataPath))
        {
            // Subtract the data path and add "Assets" back to get the local path
            assetPath = "Assets" + assetPath.Substring(dataPath.Length);
        }
        else
        {
            // Fallback: If it's outside the data path for some reason, try to find it via GUID
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath); 
        }

        // Now check if it exists in Unity's eyes
        var assetEntry = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (assetEntry == null)
        {
            Debug.Log($"<b>Cygon (UCF)</b> <color=red>File detected but Unity can't find it at: {assetPath}</color>");
            return;
        }

        Debug.Log($"<b>Cygon (UCF)</b> <color=orange>Refreshing scene...:</color> {assetPath}");

        // 1. Force the import
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        // 2. Load the prefab
        GameObject updatedAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (updatedAsset == null) return;

        // 3. Update scene objects
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        string fileName = Path.GetFileNameWithoutExtension(assetPath);

        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i] == null)
                break;
            
            // Check if the object name matches the file or the common Unity (Clone) suffix
            if (allObjects[i] != null && allObjects[i].name == fileName || allObjects[i].name == fileName + "(Clone)")
            {
                UpdateInstance(allObjects[i], updatedAsset);
            }
        }
    }

    private static void UpdateInstance(GameObject instance, GameObject sourceAsset)
    {
        // Record for Undo system
        Undo.RegisterCompleteObjectUndo(instance, "Cygon (UCF) RuntimeSync Update");

        // Clear children
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in instance.transform) children.Add(child.gameObject);
        foreach (GameObject child in children) Object.DestroyImmediate(child);

        // Re-spawn from the fresh import
        foreach (Transform child in sourceAsset.transform)
        {
            GameObject newChild = Object.Instantiate(child.gameObject, instance.transform);
            newChild.name = child.name;
        }
        
        // 1. If it's a prefab instance, revert it to clear "Play Mode" junk/overrides
        if (PrefabUtility.IsPartOfAnyPrefab(instance)) 
        { 
            // Revert ensures the instance is a "valid" clone of the disk asset again 
            PrefabUtility.RevertPrefabInstance(instance, InteractionMode.AutomatedAction);
        }

        Debug.Log($"<b>Cygon (UCF)</b> <color=green>Refreshed instance {instance.name}</color>");
    }
}