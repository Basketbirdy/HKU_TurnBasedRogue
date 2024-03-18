using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [Header("StateMachine")]
    [SerializeField] private GameFSM fsm;

    [Header("References")]
    [SerializeField] public GameObject playerObj;
    [SerializeField] public PlayerMovement playerMovement;

    [Header("UI")]
    [SerializeField] public UIObject[] uiObjects;

    [Header("Progression")]
    [SerializeField] public int cheeseCollected = 0;
    [SerializeField] public int cheeseNeeded = 4;
    public Color hasEnoughColor;

    [Header("Debug")]
    [Header("Data")]
    [SerializeField] public bool isPaused = false;
    [Header("Options")]
    [SerializeField] bool openMainMenu = true;
    [SerializeField] bool startInTutorial = true;

    #region Singleton

    public static GameManager instance;

    public static GameManager Instance
    {
        get { if (instance == null) { Debug.Log("TurnManager = null"); } return instance; }
    }

    private void Awake()
    {
        instance = this;
    }

    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        fsm = new GameFSM(this);

        if (openMainMenu) { fsm.ChangeState(typeof(MainMenuState)); }
        else { fsm.ChangeState(typeof(PlayingState)); }

        if (startInTutorial)
        {
            playerObj.transform.position = new Vector2(253.5f, .5f);
            playerObj.GetComponent<PlayerMovement>().targetPos = new Vector2(253.5f, .5f);
            playerObj.GetComponent<PlayerMovement>().currentTile = new Vector3Int(253, 0, 0);
            Camera.main.transform.position = new Vector3(253, 0, -10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // call statemachine update
        fsm.Update();

        if(Input.GetKeyDown(KeyCode.Escape)) { PauseGame(); }
    }

    public void StartGame()
    {
        fsm.ChangeState(typeof(PlayingState));
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        fsm.ChangeState(typeof(MainMenuState));
    }

    public void PauseGame()
    {
        if(!isPaused) { fsm.ChangeState(typeof(PauseState)); }
        else { StartGame(); }
    }

    public void GameOver()
    {
        fsm.ChangeState(typeof(GameOverState));
    }

    public void WinGame()
    {
        fsm.ChangeState(typeof(WinState));
    }

    public void CollectCheese()
    {
        cheeseCollected++;

        // update ui counter
        for(int i = 0; i < uiObjects.Length; i++)
        {
            TextMeshProUGUI counterText;

            if (uiObjects[i].objName == "HUD") 
            { 
                counterText = uiObjects[i].obj.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                counterText.text = "x " + cheeseCollected.ToString();

                if(cheeseCollected >= cheeseNeeded)
                {
                    counterText.color = hasEnoughColor;
                }
            }
        }

        TurnManager.instance.AdvanceTurn(1);
    }
}
