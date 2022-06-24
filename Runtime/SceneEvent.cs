using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class SceneEvent : MonoBehaviour
{
    public UnityAction onEnd;
    public abstract void Progress();

}
