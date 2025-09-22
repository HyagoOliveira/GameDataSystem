using UnityEngine;
using UnityEditor;

namespace ActionCode.GameDataSystem.Editor
{
    /// <summary>
    /// Finds assets related to GameData.
    /// </summary>
    public static class GameDataFinder
    {
        [MenuItem("Tools/Find/GameData")]
        private static void FindGameData() => FindFirstAsset(nameof(AbstractGameData));

        public static void FindFirstAsset(string type)
        {
            var query = $"t:{type}";
            var guids = AssetDatabase.FindAssets(query);

            if (guids.Length == 0)
            {
                Debug.LogWarning($"No assets of type '{type}' were found.");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            Find(path);
        }

        public static void Find(string path)
        {
            var folder = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (folder == null) return;

            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }
    }
}