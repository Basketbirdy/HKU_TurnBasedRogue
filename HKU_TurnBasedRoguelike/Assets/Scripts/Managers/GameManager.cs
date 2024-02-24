using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("StateMachine")]
    [SerializeField] private FSM<GameManager> fsm;

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
        fsm.ChangeState(typeof(MainMenuState<GameManager>));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G)) 
        {
            fsm.ChangeState(typeof(PlayingState<GameManager>));
        }
    }
}
