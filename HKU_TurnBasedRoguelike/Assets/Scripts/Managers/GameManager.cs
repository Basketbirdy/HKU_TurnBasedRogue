using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("StateMachine")]
    [SerializeField] private FSM<GameManager> fsm;

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
        fsm = new FSM<GameManager>(this);

        if (openMainMenu) { fsm.ChangeState(typeof(MainMenuState<GameManager>)); }
        else { fsm.ChangeState(typeof(PlayingState<GameManager>)); }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) { PauseGame(); }
    }

    public void StartGame()
    {
        fsm.ChangeState(typeof(PlayingState<GameManager>));
    }

    public void PauseGame()
    {
        if(!isPaused) { fsm.ChangeState(typeof(PauseState<GameManager>)); }
        else { StartGame(); }
        
    }
}
