using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ActionCode.GameDataSystem.Editor
{
    /// <summary>
    /// Resets the project GameData (if available) before the Build starts.
    /// </summary>
    public sealed class GameDataBuildReseter : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            var hasGameData = GameDataFinder.TryGetGameData(out var gameData);
            if (!hasGameData) return;

            gameData.ResetData();
            Debug.Log($"ScriptableObject '{gameData.name}' was reseted.");
        }
    }
}