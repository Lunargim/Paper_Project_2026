using UnityEngine;

public class MusicArea : MonoBehaviour
{
    [SerializeField] private MusicAreaType _musicArea;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            AudioManager.instance.SetMusicArea(_musicArea);
    }
}