using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance {get; private set;}
    
    /*[field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }*/
    
    [field: Header("Player")] 
    [field: SerializeField] public EventReference jump {get; private set;}
    [field: SerializeField] public EventReference jumpLanding {get; private set;}
    [field: SerializeField] public EventReference playerFootsteps {get; private set;}

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
