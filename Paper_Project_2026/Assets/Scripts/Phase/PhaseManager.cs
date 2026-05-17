using System;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager instance;
    
    private GamePhase _gamePhase;
    public GamePhase CurrentGamePhase => _gamePhase;
    public static Action<GamePhase> OnGamePhaseChange;
    public delegate void BeforeGamePhaseChange();
    public delegate void AfterGamePhaseChange();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }
        
    public void ChangeGamePhase(GamePhase newPhase, BeforeGamePhaseChange before = null, AfterGamePhaseChange after = null)
    {
        before?.Invoke();
        _gamePhase = newPhase;
        OnGamePhaseChange?.Invoke(_gamePhase);
        after?.Invoke();
    }
    
}

public enum GamePhase
{
    Rain,
    Sun,
}

