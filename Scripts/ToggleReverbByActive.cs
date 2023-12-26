using UdonSharp;
using UnityEngine;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleReverbByActive : UdonSharpBehaviour
    {
        [SerializeField]
        AudioReverbFilter AudioReverbFilter;

        void OnEnable()
        {
            AudioReverbFilter.enabled = true;
        }

        void OnDisable()
        {
            AudioReverbFilter.enabled = false;
        }
    }
}
