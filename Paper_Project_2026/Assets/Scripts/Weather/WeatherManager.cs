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
    public void Awake()
    {
        _dayLightColor = _directionalLight.color;
        _rainBox.SetActive(false);
    }

    public void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (_directionalLight.color == _rainDayLightColor)
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

    }
    
    //SAREBBE DA FARE A PHASE RAIN PHASE E SUN PHASE UN PO COME NEL GIOCO DI TEAMOWRK.
    
}
