using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TriggerInOutSound : UdonSharpBehaviour
    {
        [SerializeField]
        AudioSource[] AudioSources;
        [SerializeField]
        AudioClip[] PlayerInSounds;
        [SerializeField]
        AudioClip[] PlayerOutSounds;
        [SerializeField]
        AudioClip[] ObjectInSounds;
        [SerializeField]
        AudioClip[] ObjectOutSounds;

        int UseAudioSourceIndex;

        const int PickupLayer = 13;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            AllocateAndPlay(player.GetPosition(), RandomAudioOf(PlayerInSounds));
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            AllocateAndPlay(player.GetPosition(), RandomAudioOf(PlayerOutSounds));
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PickupLayer) AllocateAndPlay(other.transform.position, RandomAudioOf(ObjectInSounds));
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == PickupLayer) AllocateAndPlay(other.transform.position, RandomAudioOf(ObjectOutSounds));
        }

        void AllocateAndPlay(Vector3 position, AudioClip clip)
        {
            UseAudioSourceIndex++;
            if (UseAudioSourceIndex >= AudioSources.Length) UseAudioSourceIndex = 0;
            var audioSource = AudioSources[UseAudioSourceIndex];
            audioSource.transform.position = position;
            audioSource.clip = clip;
            audioSource.Play();
        }

        AudioClip RandomAudioOf(AudioClip[] clips)
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}
