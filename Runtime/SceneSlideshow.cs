using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SceneSlideshow : SceneEvent
{
    [SerializeField]
    Slideshow slideshowObject;

    Image imageComponent;
    private int currentIndex = 0;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    public void Start()
    {
        if (slideshowObject != null)
        {
            if (slideshowObject.ImageSet.Count > 0)
            {
                displayImage(slideshowObject.ImageSet[0]);
            }
        }
    }

    public override void Progress()
    {
        currentIndex++;
        if (currentIndex < slideshowObject.ImageSet.Count)
        {
            var nextImage = slideshowObject.ImageSet[currentIndex];
            displayImage(nextImage);
        }
        else
        {
            onEnd();
        }
    }

    private void displayImage(Texture2D img)
    {
        Debug.Log(img.name);
        Sprite sprite = constructSprite(img);
        imageComponent.sprite = sprite;
    }

    private Sprite constructSprite(Texture2D text)
    {
        if (text != null)
        {
            return Sprite.Create(text, imageComponent.sprite.rect, imageComponent.sprite.pivot);
        }
        return null;
    }
}
