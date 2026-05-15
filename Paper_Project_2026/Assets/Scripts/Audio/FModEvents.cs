using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance {get; private set;}
    
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }
    
    [field: Header("Player")] 
    
    [field: Header("Player/Footspeps")]
    
    [field: SerializeField] public EventReference p_Footsteps_Wood {get; private set;}
    [field: SerializeField] public EventReference p_Footsteps_Rock {get; private set;}
    [field: SerializeField] public EventReference p_Footsteps_Grass {get; private set;}
    [field: SerializeField] public EventReference p_Footsteps_Dirt {get; private set;}
    
    [field: Header("Player/Jump")]
    [field: SerializeField] public EventReference jump {get; private set;}
    [field: SerializeField] public EventReference jumpLanding {get; private set;}

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
