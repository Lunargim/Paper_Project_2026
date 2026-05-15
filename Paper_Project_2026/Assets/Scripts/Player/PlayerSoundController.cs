using FMOD.Studio;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    private EventInstance _playerFootsteps;
    private PlayerMovementController _player;
    
    private bool _wasWalking = false;
    
    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private float _groundCheckHeight;
    
    private SurfaceType _currentSurfaceType = SurfaceType.Dirt;
    
    void Awake()
    {
        _player = GetComponent<PlayerMovementController>();
        
        _player.Jump += PlayJumpSound;
        _player.Land += PlayLandSound;
        _player.Walk += PlayWalkSound;
        /*_player.OnDash         += PlayDash;
        _player.OnStartFalling += StopFootstep;*/
        
    }
    
    void Update()
    {
        //_playerFootsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));

    }

    void OnDestroy()
    {
        _player.Jump -= PlayJumpSound;
        _player.Land -= PlayLandSound;
        _player.Walk -= PlayWalkSound;
        /*_player.OnDash         -= PlayDash;
        _player.OnStartFalling -= StopFootstep;*/

        _playerFootsteps.release();
    }
    
    private void PlayWalkSound(float playerX, float playerY, bool isGrounded)
    {
        bool isWalking = (playerX != 0 || playerY != 0) && isGrounded;
        
        SurfaceType newSurface = isWalking ? GetSurfaceType() : _currentSurfaceType;

        bool surfaceChanged = isWalking && (newSurface != _currentSurfaceType);
        bool walkStateChanged = isWalking != _wasWalking;

        if (walkStateChanged || surfaceChanged)
        {
            _playerFootsteps.stop(STOP_MODE.IMMEDIATE);
            _playerFootsteps.release();

            if (isWalking)
            {
                _currentSurfaceType = newSurface;
                switch (GetSurfaceType())
                {
                    case SurfaceType.Grass:
                        _playerFootsteps =
                            AudioManager.instance.CreateEventInstance(FMODEvents.instance.p_Footsteps_Grass);
                        break;
                    case SurfaceType.Rock:
                        _playerFootsteps =
                            AudioManager.instance.CreateEventInstance(FMODEvents.instance.p_Footsteps_Rock);
                        break;
                    case SurfaceType.Wood:
                        _playerFootsteps =
                            AudioManager.instance.CreateEventInstance(FMODEvents.instance.p_Footsteps_Wood);
                        break;
                    case SurfaceType.Dirt:
                        _playerFootsteps =
                            AudioManager.instance.CreateEventInstance(FMODEvents.instance.p_Footsteps_Dirt);
                        break;
                }
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(_playerFootsteps, this.gameObject);
            }
            
            _wasWalking = isWalking;
            PlayingLoopingSounds(_playerFootsteps, isWalking);
        }

    }

    public void PlayJumpSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.jump, this.transform.position);
    }

    public void PlayLandSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.jumpLanding, this.transform.position);
    }

    public void PlayingLoopingSounds(EventInstance eventInstance, bool checkingCondition)
    {
        if (checkingCondition)
        {
            eventInstance.getPlaybackState(out var playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                eventInstance.start();
            }
        }
        else 
        {
            eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    public SurfaceType GetSurfaceType()
    {
       
        var colliders = Physics.OverlapSphere(transform.position, _groundCheckRadius, 
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
    
        foreach (var col in colliders)
        {
            var surface = col.GetComponentInParent<Surface>();
            if (surface != null)
                return surface.type;
        }

        var ray = new Ray(transform.position, Vector3.down);
        if (Physics.SphereCast(ray, _groundCheckRadius, out RaycastHit info, _groundCheckHeight,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            var surface = info.transform.GetComponentInParent<Surface>();
            if (surface != null)
                return surface.type;
        }

        return SurfaceType.Wood;
    }
    
    private void OnDrawGizmos()
    {
        var ray = new Ray(transform.position, Vector3.down);
        bool hit = Physics.SphereCast(ray, _groundCheckRadius, out RaycastHit info, _groundCheckHeight);
    
        Gizmos.color = hit ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundCheckHeight);
        Gizmos.DrawWireSphere(transform.position + Vector3.down * _groundCheckHeight, _groundCheckRadius);
    }
    
}
