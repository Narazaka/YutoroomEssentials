using UdonSharp;
using UnityEngine;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    namespace UdonFloatingObject
    {
        [RequireComponent(typeof(Collider))]
        [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
        public class FloatingObjectTop : UdonSharpBehaviour
        {
            [SerializeField]
            FloatingObject FloatingObject;

            void OnTriggerEnter(Collider other)
            {
                FloatingObject.OnTopTriggerEnter(other);
            }

            void OnTriggerExit(Collider other)
            {
                FloatingObject.OnTopTriggerExit(other);
            }
        }
    }
}
