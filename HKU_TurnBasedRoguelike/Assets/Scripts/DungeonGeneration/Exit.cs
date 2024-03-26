using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Exit : MonoBehaviour, IInteractable
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject uiObj;
    [SerializeField] GameObject bouncer;
    [SerializeField] TextMeshProUGUI cheeseText;

    [Header("Values")]
    [SerializeField] int cheeseNeeded;
    [SerializeField] bool isOpen = false;

    #region EventSetup

    private void OnEnable()
    {
        PlayingState.onStartPlaying += UpdateUI;
    }

    private void OnDisable()
    {
        PlayingState.onStartPlaying -= UpdateUI;
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // show amount of cheese needed
        if (collision.CompareTag("Player"))
        {
            uiObj.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // hide amount of chees needed
        if (collision.CompareTag("Player"))
        {
            uiObj.SetActive(false);
        }
    }

    void UpdateUI()
    {
        cheeseText.text = GameManager.instance.cheeseNeeded.ToString() + "x";
        Debug.Log("ExitUI; cheeseText is: " + GameManager.instance.cheeseNeeded);
    }

    public void Interact()
    {
        if(isOpen)
        {
            GameManager.instance.WinGame();
            return;
        }

        // check if the player has collected enough cheese
        if(GameManager.instance.cheeseCollected < GameManager.instance.cheeseNeeded) 
        {
            // if no, return with error and animation   
            animator.SetTrigger("Deny");
            return; 
        }

        // play succes animation
        // move the bouncer aside
        animator.SetTrigger("Accept");
        uiObj.SetActive(false);

        isOpen = true;
    }
}
