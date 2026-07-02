using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AmbientArea : MonoBehaviour
{
    [SerializeField] private EventReference _ambientArea;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            AudioManager.instance.ChangeAmbience(_ambientArea);
    }
}
