using UdonSharp;
using UnityEngine;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InWaterCollider : UdonSharpBehaviour
    {
        [SerializeField, Header("InWaterオブジェクト")]
        InWater Controller;

        void OnTriggerEnter(Collider other)
        {
            Controller.EnterWater(other);
        }

        void OnTriggerExit(Collider other)
        {
            Controller.ExitWater(other);
        }
    }
}