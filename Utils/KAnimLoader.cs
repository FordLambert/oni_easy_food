using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace EasyFood.Utils
{
    public static class KAnimLoader
    {
        private static string modPath;

        public static void SetModPath(string path)
        {
            modPath = path;
            Debug.Log($"EasyFood: Mod path set to {modPath}");
        }

        public static void LoadAllKAnims()
        {
            if (string.IsNullOrEmpty(modPath))
            {
                Debug.LogError("EasyFood: Mod path not set!");
                return;
            }

            string animAssetsPath = Path.Combine(modPath, "anim", "assets");

            if (!Directory.Exists(animAssetsPath))
            {
                Debug.LogWarning($"EasyFood: Anim assets directory not found at {animAssetsPath}");
                return;
            }

            foreach (string kanimDir in Directory.GetDirectories(animAssetsPath))
            {
                string kanimName = Path.GetFileName(kanimDir);
                LoadKAnim(kanimDir, kanimName);
            }
        }

        private static void LoadKAnim(string directory, string kanimName)
        {
            try
            {
                // Remove _kanim suffix for the base name
                string baseName = kanimName.EndsWith("_kanim")
                    ? kanimName.Substring(0, kanimName.Length - 6)
                    : kanimName;

                string pngPath = Path.Combine(directory, $"{baseName}_0.png");
                string animPath = Path.Combine(directory, $"{baseName}_anim.bytes");
                string buildPath = Path.Combine(directory, $"{baseName}_build.bytes");

                if (!File.Exists(pngPath) || !File.Exists(animPath) || !File.Exists(buildPath))
                {
                    Debug.LogWarning($"EasyFood: Missing files for kanim {kanimName}");
                    Debug.LogWarning($"  PNG exists: {File.Exists(pngPath)}");
                    Debug.LogWarning($"  Anim exists: {File.Exists(animPath)}");
                    Debug.LogWarning($"  Build exists: {File.Exists(buildPath)}");
                    return;
                }

                // Load texture
                byte[] pngData = File.ReadAllBytes(pngPath);
                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                ImageConversion.LoadImage(texture, pngData);
                texture.name = baseName + "_0";

                // Load anim and build data as TextAsset
                byte[] animData = File.ReadAllBytes(animPath);
                byte[] buildData = File.ReadAllBytes(buildPath);

                TextAsset animAsset = CreateTextAsset(animData, baseName + "_anim");
                TextAsset buildAsset = CreateTextAsset(buildData, baseName + "_build");

                // Create texture list
                var textureList = new List<Texture2D> { texture };

                // Register the kanim
                ModUtil.AddKAnim(kanimName, animAsset, buildAsset, textureList);

                Debug.Log($"EasyFood: Loaded kanim {kanimName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"EasyFood: Failed to load kanim {kanimName}: {e.Message}");
                Debug.LogException(e);
            }
        }

        private static TextAsset CreateTextAsset(byte[] data, string name)
        {
            // Create a TextAsset and set its bytes using reflection
            TextAsset textAsset = new TextAsset();

            // Set the name
            textAsset.name = name;

            // Use reflection to set the internal bytes field
            FieldInfo bytesField = typeof(TextAsset).GetField("m_Script", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bytesField != null)
            {
                bytesField.SetValue(textAsset, System.Text.Encoding.UTF8.GetString(data));
            }
            else
            {
                // Try alternative approach - TextAsset stores bytes differently in some Unity versions
                // For binary data, we need to use a workaround
                var prop = typeof(TextAsset).GetProperty("bytes", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(textAsset, data);
                }
            }

            return textAsset;
        }
    }
}
