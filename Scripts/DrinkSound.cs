using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DrinkSound : UdonSharpBehaviour
    {
        [SerializeField]
        DrinkSoundPickup DrinkSoundPickup;
        [SerializeField]
        public AudioSource AudioSource;

        void Update()
        {
            DrinkSoundPickup.Drinking = (transform.rotation * Vector3.up).y < 0.1f
                && (Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position - transform.position).sqrMagnitude < 0.1225f; // 0.35^2
        }
    }
}
