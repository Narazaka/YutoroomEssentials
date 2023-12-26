using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DrinkSoundPickup : UdonSharpBehaviour
    {
        [SerializeField]
        DrinkSound DrinkSound;
        [UdonSynced, FieldChangeCallback(nameof(Drinking))]
        bool _Drinking;
        public bool Drinking
        {
            get => _Drinking;
            set
            {
                if (_Drinking == value || !Pickuping) return;
                DrinkSound.AudioSource.enabled = _Drinking = value;
                if (Networking.IsOwner(gameObject))
                {
                    RequestSerialization();
                }
            }
        }
        public bool Pickuping;

        public override void OnPickup()
        {
            Pickuping = DrinkSound.enabled = true;
        }

        public override void OnDrop()
        {
            Pickuping = DrinkSound.enabled = Drinking = false;
        }
    }
}
