using FMODUnity;
using UnityEngine;

public class Audio3DEmitter : StudioEventEmitter
{
    private StudioEventEmitter _eventEmitter;

    [Header("Occlusion")]
    [SerializeField] private bool _useOcclusion = false;
    [SerializeField] private LayerMask _occlusionLayers;
    [SerializeField] private float _occlusionSmoothTime = 0.2f;
    [SerializeField]private Transform _listener;
    
    private float _currentOcclusion;
    private float _occlusionVelocity;

    protected override void Start()
    {
        base.Start();
        if (EventReference.Guid.IsNull)
        {
            _eventEmitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.SFX_People_Talking_Inside, this.gameObject);
        }
        else
        {
            _eventEmitter = AudioManager.instance.InitializeEventEmitter(EventReference, this.gameObject);
        }
        
    }

    private void Update()
    {
        if (!_useOcclusion || _listener == null) return;

        float targetOcclusion = CalculateOcclusion();
        _currentOcclusion = Mathf.SmoothDamp(_currentOcclusion, targetOcclusion, ref _occlusionVelocity, _occlusionSmoothTime);

        if (EventInstance.isValid())
            EventInstance.setParameterByName("Occlusion", _currentOcclusion);
    }

    private float CalculateOcclusion()
    {
        Vector3 direction = _listener.position - transform.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, _occlusionLayers))
            return 1f;

        return 0f;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _eventEmitter.Stop();
    }
    
    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, _listener.position);
    }
}
