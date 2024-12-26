using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {      
        if(other.CompareTag("Player"))
        {
            Debug.Log("pickup");
            gamemanager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
