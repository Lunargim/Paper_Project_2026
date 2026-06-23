using FMODUnity;
using UnityEngine;

public class Audio3DEmitter : StudioEventEmitter
{
   private StudioEventEmitter _eventEmitter;
   [SerializeField] private EventReference _sound;

   protected override void Start()
   {
      base.Start();
      if (_sound.Guid.IsNull)
      {
         _eventEmitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.SFX_People_Talking_Inside, this.gameObject);
      }
      else
      {
         _eventEmitter = AudioManager.instance.InitializeEventEmitter(_sound, this.gameObject);
      }
      
   }

   protected override void OnDestroy()
   {
      base.OnDestroy();
      _eventEmitter.Stop();
   }
}
