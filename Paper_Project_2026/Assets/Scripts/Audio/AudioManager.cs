using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {get; private set;}

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayOneShot(EventReference eventReference, Vector3 position)
    {
        RuntimeManager.PlayOneShot(eventReference, position);
    }
}
