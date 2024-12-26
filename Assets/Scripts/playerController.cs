using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage, IOpen
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask; 

    [Header("----- Stats -----")]
    [SerializeField][Range(1, 100)] int HP;
    [SerializeField] [Range(1, 5)] int speed;
    [SerializeField] [Range(1, 5)] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] [Range(5, 20)] int jumpSpeed;
    [SerializeField] [Range(25, 35)] int gravity;
    [SerializeField] private float crouchHeight = 1.0f;  // Height of the character when crouching
    [SerializeField] private float normalHeight = 2.0f; // Normal height of the character

    [Header("----- Gun Stats -----")]
    [SerializeField] public List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int ammo;
    [SerializeField] int reloadRate;
    //[SerializeField] GameObject bullet;
    public Sprite newSprite;
    SpriteChanger spriteChanger;
    

    [Header("----- Player Sounds -----")]
    [SerializeField] AudioSource srcAudio;
    [SerializeField] AudioClip[] audioJump; // muliple jumping sound: ah, uh, eah,uah,..
    [SerializeField][Range(0, 1)] float audioJumpVol;
    [SerializeField] AudioClip[] audioDamage; 
    [SerializeField][Range(0, 1)] float audioDamageVol;
    [SerializeField] AudioClip[] audioStep;
    [SerializeField][Range(0, 1)] float audioStepVol;    

    Vector3 moveDir;

    int jumpCount;
    int HPOrig;
    int gunListPos;

    Vector3 playerVel;
    bool isSprinting;
    bool isPlayingStep;
    bool isShooting;
    bool isReloading;
    private bool isCrouching = false;

    

    //public AudioSource laserSound;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();

        spriteChanger = GetComponent<SpriteChanger>();
        if (spriteChanger == null)
        {
            Debug.Log("displayWeapon is null");
            spriteChanger.Initialize(newSprite);
        }        
         

        if (gunList.Count > 0)
            gamemanager.instance.ammoNumText.text = gunList[gunListPos].ammoCur.ToString();
    }

    // Update is called once per frame
    void Update()
    {        
               
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!gamemanager.instance.IsPaused)
        {
            movement();
            selectGun();
        }

        sprint();
        crouch();

        if (gunList.Count > 0)
        {            
            gamemanager.instance.ammoNumText.text = gunList[gunListPos].ammoCur.ToString();
            gamemanager.instance.ammoMaxNumText.text = gunList[gunListPos].ammoMax.ToString();           
        }

        
    }

    void crouch()
    {
        // Check for crouch key input
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchHeight;  // Reduce the height of the CharacterController                    
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false; 
            controller.height = normalHeight;  // Restore height            
        }
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.3 && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime;

        moveDir = (transform.right * Input.GetAxis("Horizontal")) +
                  (transform.forward * Input.GetAxis("Vertical"));
        
        //Debug.Log("MoveDir: " + moveDir); // Check if moveDir values are changing
        
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVel = Vector3.zero;
        }


        if(Input.GetButton("Fire1") && gunList.Count > 0 && !isShooting && ammo > 0 )
        {
            //Debug.Log("Fire1 button pressed!");

            //SpriteChanger.instance.SwitchWeapon();

            //IEnumerator must be called with StartCoroutine
            StartCoroutine(shoot());

            
                       
        }

        if (Input.GetButton("Reload") && gunList.Count > 0)
        {
            StartCoroutine(reload());
        }
    }

   

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            srcAudio.PlayOneShot(audioJump[Random.Range(0, audioJump.Length)], audioJumpVol);
        }
              
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    //timer
    IEnumerator shoot()
    {
        isShooting = true;
        srcAudio.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);

        // shoot code here
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);

            // does it has IDamage or not? 
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

            if (gunList[gunListPos].hitEffect != null)
            {
                Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

                gunList[gunListPos].ammoCur--;
                if( gunList[gunListPos].ammoCur < 0) gunList[gunListPos].ammoCur = 0;
                ammo = gunList[gunListPos].ammoCur;                
            }
            
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        
    }

    IEnumerator reload()
    {
        isReloading = true;
        //srcAudio.PlayOneShot(gunList[gunListPos].reloadSound[Random.Range(0, gunList[gunListPos].reloadSound.Length)], gunList[gunListPos].reloadSoundVol);

        srcAudio.clip = gunList[gunListPos].reloadSound; // Assign the clip
        srcAudio.volume = gunList[gunListPos].reloadSoundVol; // Set the volume
        srcAudio.Play(); // Play the clip

        gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        ammo = gunList[gunListPos].ammoCur;      

        yield return new WaitForSeconds(reloadRate);

        isReloading = false;
    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHpBar.fillAmount = (float)HP / HPOrig;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashScreenDamage());
        srcAudio.PlayOneShot(audioDamage[Random.Range(0, audioDamage.Length)], audioDamageVol);

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    IEnumerator flashScreenDamage()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);
    }

    // knock back player when getting shot
//    void OnTriggerEnter(Collider other)
//    {
//        // Check if the trigger is the sphere
//        if (other.CompareTag("Damage"))
//        {
//#if UNITY_EDITOR
//            Debug.Log("Player hit by ball");
//#endif

//            // Get the direction vector from the ball (sphere) to the player
//            Vector3 pushDirection = (transform.position - other.transform.position).normalized;
//            // Define the push distance
//            float pushDistance = 11.0f;
//            // Use CharacterController to move the player
//            controller.Move(pushDirection * pushDistance);
//        }
//    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;
        ammo = gun.ammoCur;

        gunModel.GetComponent<MeshFilter>().sharedMesh  = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;

        changeWeaponUI();
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changegun();
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--; 
            changegun();
        }
    }

    void changegun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;
        ammo = gunList[gunListPos].ammoCur;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        Debug.Log($"Sprite Exist for {gunList[gunListPos].name}");
        changeWeaponUI();


    }

    void changeWeaponUI()
    {
        if (gunList[gunListPos].name == "M1918Bar")
        {
            SpriteChanger.instance.changeM19(gunList[gunListPos].weaponSprite);
        }
        else if (gunList[gunListPos].name == "M1Garand")
        {
            SpriteChanger.instance.changeM1Garand(gunList[gunListPos].weaponSprite);
        }
        else if (gunList[gunListPos].name == "ThompsonM1A1")
        {
            SpriteChanger.instance.changeThomson(gunList[gunListPos].weaponSprite);
        }
    }
    
    IEnumerator playStep()
    {
        isPlayingStep = true;
        srcAudio.PlayOneShot(audioStep[Random.Range(0, audioStep.Length)], audioStepVol);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingStep = false;

    }
}
