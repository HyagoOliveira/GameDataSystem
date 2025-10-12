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
        private static void FindGameData()
        {
            var hasGameData = TryGetGameData(out var gameData);
            if (!hasGameData) return;

            Selection.activeObject = gameData;
            EditorGUIUtility.PingObject(gameData);
        }

        public static bool TryGetGameData(out AbstractGameData data)
        {
            data = null;
            var type = nameof(AbstractGameData);
            var query = $"t:{type}";
            var guids = AssetDatabase.FindAssets(query);

            if (guids.Length == 0)
            {
                Debug.LogWarning($"No assets of type '{type}' were found.");
                return false;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            data = AssetDatabase.LoadAssetAtPath<AbstractGameData>(path);
            return true;
        }
    }
}