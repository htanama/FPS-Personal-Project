using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    private void Awake()
    {
        // Subscriber - Subscribing to event 
        ObjectManager.OnHidingObject += Sphere_OnHidingObject;
        ObjectManager.OnShowingObject += Sphere_OnShowingObject;
    }

    private void Sphere_OnShowingObject()
    {
        Show();
    }

    private void Sphere_OnHidingObject()
    {
        Hide();
    }

    private void OnDestroy()
    {
        ObjectManager.OnHidingObject -= Sphere_OnHidingObject;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
