using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InColliderPPS : UdonSharpBehaviour
    {
        [SerializeField]
        PostProcessVolume[] PostProcessVolumes;
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
                SetWeights(true);
                StaySound.gameObject.SetActive(true);
                InSound.Play();
                AudioLowPassFilter.enabled = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other == Head)
            {
                SetWeights(false);
                StaySound.gameObject.SetActive(false);
                OutSound.Play();
                AudioLowPassFilter.enabled = false;
            }
        }

        void SetWeights(bool active)
        {
            foreach (var vol in PostProcessVolumes)
            {
                vol.weight = active ? 1 : 0;
            }
        }
    }
}
