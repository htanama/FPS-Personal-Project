using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary};

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destoryTime;
    
    // Start is called before the first frame update
    void Start()
    {
        if (type == damageType.moving)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destoryTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);

        if (other.isTrigger)
        {
            return; // ignore trigger colliding with other triggers. 
        }
      
        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
        }

        if (type == damageType.moving)
        {
            Destroy(gameObject);
        }
        
    }

}
