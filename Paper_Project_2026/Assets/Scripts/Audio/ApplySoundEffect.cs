using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ApplySoundEffect : MonoBehaviour
{
    [SerializeField] private string _parameterName = "Reverb";
    [SerializeField] private float _enterValue = 1f;
    [SerializeField] private float _exitValue = 0f;

    public void Start()
    {
        AudioManager.instance.SetGlobalParameter(_parameterName, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        AudioManager.instance.SetGlobalParameter(_parameterName, _enterValue);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        AudioManager.instance.SetGlobalParameter(_parameterName, _exitValue);
    }
}