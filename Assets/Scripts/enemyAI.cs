using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage, IOpen
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist; // sphere distance of roaming
    [SerializeField] int roamTimer; // how long to wait before move again
    [SerializeField] int animSpeedTransition;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    public Image EnemyHPBar;

    bool isShooting;
    bool playerInRange;
    bool isRoaming;

    
    int HPOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    Color colorOrig;
    RaycastHit hit;
    float angleToPlayer;
    float stoppingDistOrig; // to remember our original stopping distance. 
    Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        startingPos = transform.position; // to remember the starting position 
        stoppingDistOrig = agent.stoppingDistance; // to remember our original stopping distance. 
        HPOrig = HP;
        updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
    
        //float agentSpeed = agent.velocity.normalized.magnitude;
        //float animSpeed = anim.GetFloat("Speed");

        //anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTransition)); // original ID is Blend, we change the ID to Speed

        if (playerInRange && !canSeePlayer())
        {
            // check the timer && check distance if it is closer to the distance by 0.01f
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                coroutine = StartCoroutine(roam());
            }
                
        }
        else if (!playerInRange) // the enemy is not in player range
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                coroutine = StartCoroutine(roam());
            }
        }
    }

    IEnumerator roam()
    {
        isRoaming = true;
        
        yield return new WaitForSeconds(roamTimer); // wait for second before continuing. 
        
        agent.stoppingDistance = 0; // only for roaming to make sure the AI reach its destination
        
        Vector3 randomPos = Random.insideUnitSphere * roamDist; // how big is our roaming distance        
        randomPos += startingPos;

        NavMeshHit hit; // get info using similar like raycast
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1); // remember where the hit is at. 
        agent.SetDestination(hit.position);

        isRoaming = false;
    }

    void updateUI()
    {
        EnemyHPBar.fillAmount = (float)HP / HPOrig;
    }

    bool canSeePlayer()
    {
        // this is the head position of the enemy
        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // playerDirection = GameManager.instance.player.transform.position - transform.position; // this is the feet position of the enemy, not the head position. 

        // Draw the Raycast inside the debug mode
        #if UNITY_EDITOR
                Debug.DrawRay(headPos.position, playerDir);
        #endif

       
        // To know the location of the player by using raycasting, do we hit the player
        if (Physics.Raycast(headPos.position, playerDir, out hit)) // Inside the sphere range.
        {
            // if the Raycast hit the player then do the statement inside the if statement.
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                //agent.SetDestination(playerPosition.position);
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                agent.stoppingDistance = stoppingDistOrig; // get the distance closer to the player but not very close to the player face. 
                return true; // I can see player
            }

        }
        agent.stoppingDistance = 0; // I cannot see the player
        return false; // do not see the player, raycast did not hit the player
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);          
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other) // enter sphere collision
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other) // exit sphere collision
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0; // agent cannot see player set stopping distance at zero
        }
    }

    public void takeDamage(int amount)
    {   
        HP -= amount;
        updateUI();

        agent.SetDestination(gamemanager.instance.player.transform.position); // go to the last position of the player
        
        StopCoroutine(coroutine);
        isRoaming = false; // need function for this to keep track 
        
        StartCoroutine(flashRed());

        if(HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            
            // I am dead
            Destroy(gameObject);            
        }

    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }


    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

}
