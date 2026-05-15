using FMOD.Studio;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    private EventInstance _playerFootsteps;
    private PlayerMovementController _player;
    
    private bool _wasWalking = false;
    
    void Awake()
    {
        _player = GetComponent<PlayerMovementController>();
        
        _player.Jump += PlayJumpSound;
        _player.Land += PlayLandSound;
        _player.Walk += PlayWalkSound;
        /*_player.OnDash         += PlayDash;
        _player.OnStartFalling += StopFootstep;*/
        
    }
    
    void Start()
    {
        _playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_playerFootsteps, this.gameObject);
    }
    
    void Update()
    {
        _playerFootsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.gameObject));
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
        bool condition = (playerX != 0 || playerY != 0) && isGrounded;
        
        if (condition != _wasWalking)
        {
            _wasWalking = condition;
            PlayingLoopingSounds(_playerFootsteps, condition);   
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
    
}
