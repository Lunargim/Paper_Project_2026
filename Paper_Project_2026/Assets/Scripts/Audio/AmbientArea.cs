using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AmbientArea : MonoBehaviour
{
    [SerializeField] private EventReference ambientArea;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventDescription eventDescription = RuntimeManager.GetEventDescription(ambientArea);
            AudioManager.instance.SetAmbienceParameter(AudioManager.instance.GetParametersNames(eventDescription), 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventDescription eventDescription = RuntimeManager.GetEventDescription(ambientArea);
            AudioManager.instance.SetAmbienceParameter(AudioManager.instance.GetParametersNames(eventDescription), 0);
        }
    }
    
}
