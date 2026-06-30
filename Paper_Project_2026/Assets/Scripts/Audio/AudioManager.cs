using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private FMOD.GUID currentAmbienceId;
    private EventInstance musicEventInstance;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
        
    }

    private void Start()
    {
        /*masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");*/
        
        InitializeAmbience(FMODEvents.instance.a_Village_Ambient);
        InitializeMusic(FMODEvents.instance.music);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public void ChangeAmbience(EventReference newAmbienceEvent)
    {
        FMOD.GUID newId = newAmbienceEvent.Guid;

        if (ambienceEventInstance.isValid() && newId == currentAmbienceId)
            return;

        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            StartCoroutine(ReleaseWhenStopped(ambienceEventInstance));
        }

        ambienceEventInstance = CreateEventInstance(newAmbienceEvent);
        currentAmbienceId = newId;
        ambienceEventInstance.start();
    }

    private IEnumerator ReleaseWhenStopped(EventInstance instance)
    {
        PLAYBACK_STATE state;
        do
        {
            yield return null;
            instance.getPlaybackState(out state);
        } while (state != PLAYBACK_STATE.STOPPED);

        instance.release();
    }
    
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("area", (float) area);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    
    public string GetParametersNames(EventDescription eventDescription)
    {
        string name = string.Empty;
        if (eventDescription.isValid())
        {
            eventDescription.getParameterDescriptionCount(out int parameterCount);
            
            for (int i = 0; i < parameterCount; i++)
            {
                eventDescription.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION paramDescription);
                
                name = paramDescription.name;
                float min = paramDescription.minimum;
                float max = paramDescription.maximum;
            }
        }

        return name;
    }
}