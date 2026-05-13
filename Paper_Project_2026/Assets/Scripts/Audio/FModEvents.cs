using FMODUnity;
using UnityEngine;

public class FModEvents : MonoBehaviour
{
    public FModEvents instance {get; private set;}
    [field: SerializeField] public EventReference jump {get; private set;}

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
