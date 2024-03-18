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

    // Start is called before the first frame update
    void Start()
    {
        cheeseNeeded = GameManager.instance.cheeseNeeded;
        cheeseText.text = cheeseNeeded.ToString() + "x";
    }

    // Update is called once per frame
    void Update()
    {

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

    public void Interact()
    {
        if(isOpen)
        {
            GameManager.instance.WinGame();
            return;
        }

        // check if the player has collected enough cheese
        if(GameManager.instance.cheeseCollected < cheeseNeeded) 
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


    //IEnumerator Lerp(float lerpDuration, float startValue, float endValue)
    //{
    //    float timeElapsed = 0;

    //    while (timeElapsed < lerpDuration)
    //    {
    //        valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
    //        timeElapsed += Time.deltaTime;

    //        yield return null;
    //    }

    //    valueToLerp = endValue;
    //}
}
