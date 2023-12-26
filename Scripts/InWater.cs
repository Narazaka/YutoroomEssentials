using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InWater : UdonSharpBehaviour
    {
        [SerializeField]
        Transform[] Objects;
        [SerializeField]
        AudioSource StaySound;
        [SerializeField]
        AudioSource InSound;
        [SerializeField]
        AudioSource OutSound;
        [SerializeField]
        AudioLowPassFilter AudioLowPassFilter;
        [SerializeField]
        Collider Head;

        void Update()
        {
            var headData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            Head.transform.position = headData.position;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other == Head)
            {
                SetActives(true);
                StaySound.gameObject.SetActive(true);
                InSound.Play();
                AudioLowPassFilter.enabled = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other == Head)
            {
                SetActives(false);
                StaySound.gameObject.SetActive(false);
                OutSound.Play();
                AudioLowPassFilter.enabled = false;
            }
        }

        void SetActives(bool active)
        {
            foreach (var vol in Objects)
            {
                vol.gameObject.SetActive(active);
            }
        }
    }
}
