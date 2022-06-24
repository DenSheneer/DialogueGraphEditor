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

        if (!AssetDatabase.IsValidFolder("Assets/Resources/Actors"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Actors");
        }
        AssetDatabase.CreateAsset(newActor, "Assets/Resources/Actors/Unnamed actor.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newActor;
    }
}


