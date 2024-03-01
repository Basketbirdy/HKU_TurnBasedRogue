using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("StateMachine")]
    [SerializeField] private GameFSM fsm;

    [Header("References")]
    [SerializeField] public GameObject playerObj;
    [SerializeField] public PlayerMovement playerMovement;

    [Header("UI")]
    [SerializeField] public UIObject[] uiObjects;

    [Header("Debug")]
    [Header("Data")]
    [SerializeField] public bool isPaused = false;
    [Header("Options")]
    [SerializeField] bool openMainMenu = true;

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

    public void PauseGame()
    {
        if(!isPaused) { fsm.ChangeState(typeof(PauseState)); }
        else { StartGame(); }
    }
}
