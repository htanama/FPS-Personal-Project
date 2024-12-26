using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin, menuLose;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] public TMP_Text ammoNumText;
    [SerializeField] public TMP_Text ammoMaxNumText;

    public Image playerHpBar;
    public GameObject playerDamageScreen;

    public GameObject player;
    public playerController playerScript; // playerController to access playerScript


    private bool isPaused;
    

    float timeScaleOrig;

    int goalCount;

    // Start is called before the first frame update
    void Awake() // Awake() reserved for GameManager only
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel")) // this is escape button created by Unity
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuPause.SetActive(isPaused); // toggle menuPause active or inactive
                Debug.Log("Pause Menu");
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
                Debug.Log("Game Resume");
            }
        }
    }

    // Getter and Setter for private bool isPaused
    public bool IsPaused 
    { 
        get => isPaused; // Returns the current value of the 'isPaused' field
        set => isPaused = value; // Assigns the incoming value to the 'isPaused' field
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    //amount can be positive or negative
    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");

        if (goalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }

    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
