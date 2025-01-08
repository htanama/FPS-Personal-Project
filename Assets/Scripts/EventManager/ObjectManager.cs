using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // Publisher
    public static event Action OnHidingObject;
    public static event Action OnShowingObject;

    [SerializeField] private GameObject sphere1;
    [SerializeField] private GameObject sphere2;
    [SerializeField] private GameObject sphere3;

    public void HideSphere()
    {
        // Fire or Invoke the event
        OnHidingObject?.Invoke();
    }
    public void ShowSphere()
    {
        OnShowingObject?.Invoke();
    }

}
