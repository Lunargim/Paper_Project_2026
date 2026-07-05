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

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _ambienceBus;
    private Bus _sfxBus;

    private List<EventInstance> _eventInstances;
    private List<StudioEventEmitter> _eventEmitters;

    private EventInstance _ambienceEventInstance;
    private FMOD.GUID _currentAmbienceId;
    private EventInstance _musicEventInstance;

    private Levels _currentLevel = 0;
    private GamePhase _currentPhase;
    
    [SerializeField] private EventReference[] _levelMusicEvents;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        _eventInstances = new List<EventInstance>();
        _eventEmitters = new List<StudioEventEmitter>();
        
        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _ambienceBus = RuntimeManager.GetBus("bus:/Ambient");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        
    }

    private void Start()
    {
        
        if (PhaseManager.instance != null)
            _currentPhase = PhaseManager.instance.CurrentGamePhase;
        
        
        InitializeAmbience();
        InitializeMusic(); //questo sarebbe da mettere che controlla il current level perche ad adesso
                           //fa solo il primo e ogni livello ha la propria bank con i suoi suoni
                           //e andrebbe aggiornato ad ogni cambio livello non nello start
    }

    private void Update()
    {
        _masterBus.setVolume(masterVolume);
        _musicBus.setVolume(musicVolume);
        _ambienceBus.setVolume(ambienceVolume);
        _sfxBus.setVolume(SFXVolume);
    }
    
    //--------------------------AMBIENCE----------------------------

    private void InitializeAmbience()
    {
        _ambienceEventInstance = CreateEventInstance(FMODEvents.instance.a_Village_Ambient);
        ApplyWeatherParameter(_ambienceEventInstance);
        _ambienceEventInstance.start();
    }
 
    public void ChangeAmbience(EventReference newAmbienceEvent)
    {
        FMOD.GUID newId = newAmbienceEvent.Guid;
 
        if (_ambienceEventInstance.isValid() && newId == _currentAmbienceId)
            return;
 
        if (_ambienceEventInstance.isValid())
        {
            _ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            StartCoroutine(ReleaseWhenStopped(_ambienceEventInstance));
        }
 
        _ambienceEventInstance = CreateEventInstance(newAmbienceEvent);
        _currentAmbienceId = newId;
        ApplyWeatherParameter(_ambienceEventInstance);
        _ambienceEventInstance.start();
    }
 
    private void ApplyWeatherParameter(EventInstance instance)
    {
        instance.getDescription(out EventDescription description);
 
        if (description.isValid() &&
            description.getParameterDescriptionByName("weather", out _) == FMOD.RESULT.OK)
        {
            instance.setParameterByName("weather", (float)_currentPhase);
        }
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
    
    //--------------------------MUSIC----------------------------
    
    private void InitializeMusic()
    {
        int levelIndex = (int)_currentLevel;

        if (levelIndex < 0 || levelIndex >= _levelMusicEvents.Length)
        {
            return;
        }

        _musicEventInstance = CreateEventInstance(_levelMusicEvents[levelIndex]);
        _musicEventInstance.start();
        
    }

    public void SetMusicArea(MusicAreaType area)
    {
        _musicEventInstance.setParameterByName("area", (float) area);
    }
    
    //--------------------------ONESHOT----------------------------

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    
    //--------------------------EFFECTS----------------------------

    public void SetGlobalParameter(string parameterName, float value)
    {
        Debug.Log($"Setting {parameterName} to {value}");
        RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
    }

    
    //--------------------------GENERAL----------------------------

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        _eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in _eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    
    public string GetParametersNames(EventDescription eventDescription) //la tengo se dovesse servire
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
    
    public void SetGamePhase(GamePhase phase)
    {
        _currentPhase = phase;
 
        /*if (_musicEventInstance.isValid())
            _musicEventInstance.setParameterByName("area", (float)phase);*/
 
        if (_ambienceEventInstance.isValid())
            ApplyWeatherParameter(_ambienceEventInstance);
    }
    
    private void OnEnable()
    {
        PhaseManager.OnGamePhaseChange += SetGamePhase;
    }
 
    private void OnDisable()
    {
        PhaseManager.OnGamePhaseChange -= SetGamePhase;
    }
}

public enum Levels //Ovviamente so che non dovrebbe stare qui e' solo per test
{
    level1 = 0,
    level2 = 1,
    level3 = 2,
}