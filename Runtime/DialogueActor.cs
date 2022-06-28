using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DialogueActor : ScriptableObject
{
    public Color HighlightColor;
    [MenuItem("Visual Novel/Create new actor...")]
    public static void CreateNewActor()
    {
        var newActor = CreateInstance<DialogueActor>();

        DGE_Utilities.EnsureExistingActorsFolder();
        AssetDatabase.CreateAsset(newActor, "Assets/Resources/Actors/Unnamed actor.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newActor;
    }
}


