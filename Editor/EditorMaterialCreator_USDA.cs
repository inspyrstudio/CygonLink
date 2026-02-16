using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.Rendering;

public class EditorMaterialCreator_USDA : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        List<string> usdasToReimport = new List<string>();

        foreach (string path in importedAssets)
        {
            if (path.ToLower().EndsWith(".usda"))
            {
                if (ProcessUsdaMaterials(path)) 
                {
                    usdasToReimport.Add(path);
                }
            }
        }

        // Re-import the USDA files ONLY if we just created their materials for the first time.
        // This fixes the "Pink" issue by letting the Importer see the new .mat files.
        foreach (string usdaPath in usdasToReimport)
        {
            AssetDatabase.ImportAsset(usdaPath, ImportAssetOptions.ForceUpdate);
        }
    }

    private static bool ProcessUsdaMaterials(string usdaPath)
    {
        bool createdNew = false;
        string usdaFolder = Path.GetDirectoryName(usdaPath);
        string materialsFolder = Path.Combine(usdaFolder, "materials");
        if (!Directory.Exists(materialsFolder)) Directory.CreateDirectory(materialsFolder);

        string rawText = File.ReadAllText(usdaPath);

        // 1. Identify all Material blocks
        // Using a simpler split to find blocks starting with 'def Material'
        string[] materialBlocks = rawText.Split(new string[] { "def Material" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var block in materialBlocks)
        {
            Match nameMatch = Regex.Match(block, @"""([^""]+)""");
            if (!nameMatch.Success) continue;

            string matName = nameMatch.Groups[1].Value;
            string matPath = Path.Combine(materialsFolder, matName + ".mat").Replace('\\', '/');

            // Only create if it doesn't exist to allow for manual user edits later
            if (AssetDatabase.LoadAssetAtPath<Material>(matPath) == null)
            {
                // Detect Pipeline
                string shaderName = "Standard";
                string colorProp = "_Color";
                string texProp = "_MainTex";

                if (GraphicsSettings.currentRenderPipeline != null)
                {
                    string rpName = GraphicsSettings.currentRenderPipeline.GetType().ToString();
                    if (rpName.Contains("Universal")) {
                        shaderName = "Universal Render Pipeline/Lit";
                        colorProp = "_BaseColor";
                        texProp = "_BaseMap";
                    }
                    else if (rpName.Contains("HDRP")) {
                        shaderName = "HDRP/Lit";
                        colorProp = "_BaseColor";
                        texProp = "_BaseColorMap";
                    }
                }

                Material mat = new Material(Shader.Find(shaderName));

                // 2. Parse Color
                Match colorMatch = Regex.Match(block, @"color3f inputs:diffuseColor = \(([^)]+)\)");
                if (colorMatch.Success)
                {
                    string[] c = colorMatch.Groups[1].Value.Split(',');
                    Color col = new Color(
                        float.Parse(c[0].Trim(), CultureInfo.InvariantCulture),
                        float.Parse(c[1].Trim(), CultureInfo.InvariantCulture),
                        float.Parse(c[2].Trim(), CultureInfo.InvariantCulture)
                    );
                    mat.SetColor(colorProp, col);
                }

                // 3. Parse Texture Path
                Match texMatch = Regex.Match(block, @"asset inputs:file = @([^@]+)@");
                if (texMatch.Success)
                {
                    string localTexPath = texMatch.Groups[1].Value.Trim().Replace('/', Path.DirectorySeparatorChar);
                    string fullTexPath = Path.Combine(usdaFolder, localTexPath).Replace('\\', '/');
                    
                    // Convert system path to Project relative path for AssetDatabase
                    string projectRelativeTexPath = "Assets" + fullTexPath.Substring(Application.dataPath.Length);
                    
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(projectRelativeTexPath);
                    if (tex != null) mat.SetTexture(texProp, tex);
                }

                AssetDatabase.CreateAsset(mat, matPath);
                createdNew = true;
            }
        }

        if (createdNew)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return createdNew;
    }
}