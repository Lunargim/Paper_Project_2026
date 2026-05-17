using UnityEngine;
using UnityEngine.InputSystem;

public class WeatherManager : MonoBehaviour
{
    
    [SerializeField] private Light _directionalLight;
    [SerializeField] private Color _rainDayLightColor;
    
    [SerializeField] private Material _skyboxSun;
    [SerializeField] private Material _skyboxRain;
    
    [SerializeField] private GameObject _rainBox;

    private Color _dayLightColor;
    
    private GamePhase _currentPhase;
    public void Awake()
    {
        _currentPhase = GamePhase.Sun;
        _dayLightColor = _directionalLight.color;
        _rainBox.SetActive(false);
    }

    public void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (_currentPhase == GamePhase.Sun)
            {
                PhaseManager.instance.ChangeGamePhase(GamePhase.Rain);
                _currentPhase = GamePhase.Rain;
            }
            else
            {
                PhaseManager.instance.ChangeGamePhase(GamePhase.Sun);
                _currentPhase = GamePhase.Sun;
            }
        }

    }
    
    private void ChangeWeather(GamePhase gamePhase)
    {
        if (gamePhase ==  GamePhase.Sun)
        {
            RenderSettings.skybox = _skyboxSun;
            _directionalLight.color = _dayLightColor;
            _rainBox.SetActive(false);
        }
        else
        {
            RenderSettings.skybox = _skyboxRain;
            _directionalLight.color = _rainDayLightColor;
            _rainBox.SetActive(true);
        }
    }

    public void OnEnable()
    {
        PhaseManager.OnGamePhaseChange += ChangeWeather;
    }

    public void OnDisable()
    {
        PhaseManager.OnGamePhaseChange -= ChangeWeather;
    }
    
    
    //SAREBBE DA FARE A PHASE RAIN PHASE E SUN PHASE UN PO COME NEL GIOCO DI TEAMOWRK.
    
}
