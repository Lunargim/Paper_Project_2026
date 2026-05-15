using UnityEngine;

public class Surface : MonoBehaviour
{
    public SurfaceType type;
}

public enum SurfaceType
{
    Dirt,
    Wood,
    Grass,
    Beach,
    Rock
}
