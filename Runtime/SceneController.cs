using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    SceneEvent sceneEvent;

    [SerializeField]
    Object nextScene = null;

    public void Start()
    {
        if (sceneEvent != null)
        {
            sceneEvent.onEnd += onEnd;
        }

        var clickBox = FindObjectOfType<Clickbox>();
        if (clickBox != null)
        {
            clickBox.AddListener(TryProgress);
        }
        else
            Debug.Log("No clickbox was found. You need a clickbox somewhere in the scene to progress the current event");
    }
    public void TryProgress()
    {
        if (sceneEvent != null)
        {
            {
                sceneEvent.Progress();
            }
        }
        else
            Debug.Log("Trying to progress but there is no scene event to progress");
    }
    private void onEnd()
    {
        if (nextScene != null)
        {
            if (nextScene != null)
            {
                SceneManager.LoadScene(nextScene.name);
            }
        }
    }
}
