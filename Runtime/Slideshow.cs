using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Slideshow : ScriptableObject
{
    public List<Texture2D> ImageSet;

    [UnityEditor.MenuItem("Visual Novel/Create new Slideshow object...")]

    public static void CreateSlideshowObject()
    {
        var newSlideshow = CreateInstance<Slideshow>();

        if (!AssetDatabase.IsValidFolder("Assets/Resources/Slideshows"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Slideshows");
        }
        AssetDatabase.CreateAsset(newSlideshow, "Assets/Resources/Slideshows/Unnamed slideshow.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newSlideshow;
    }
}
