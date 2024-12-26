using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject door;
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return; // ignore trigger colliding with other triggers. 
        }

        IOpen open = other.GetComponent<IOpen>();
        if (open != null)
        {
            door.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return; // ignore trigger colliding with other triggers. 
        }

        IOpen open = other.GetComponent<IOpen>();
        if (open != null)
        {
            door.SetActive(true);
        }
    }
}
