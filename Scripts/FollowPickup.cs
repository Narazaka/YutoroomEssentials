using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [RequireComponent(typeof(ParentConstraint)), RequireComponent(typeof(VRCPickup))]
    public class FollowPickup : UdonSharpBehaviour
    {
        [SerializeField]
        Collider Parent;
        [UdonSynced]
        bool Following;
        [UdonSynced]
        Vector3 PositionOffset;
        [UdonSynced]
        Vector3 RotationOffset;

        ParentConstraint Constraint;
        VRCPickup VRCPickup;

        bool InTrigger;

        void OnEnable()
        {
            Constraint = GetComponent<ParentConstraint>();
            VRCPickup = GetComponent<VRCPickup>();
            if (Parent.ClosestPoint(transform.position) == transform.position)
            {
                PrepareConstraint();
                Following = true;
                ActivateConstraint();
                RequestSerialization();
            }
        }

        public override void OnPickup()
        {
            if (Following && Networking.IsOwner(gameObject))
            {
                Following = false;
                Constraint.constraintActive = false;
                RequestSerialization();
            }
        }

        public override void OnDrop()
        {
            if (InTrigger && Networking.IsOwner(gameObject))
            {
                PrepareConstraint();
                Following = true;
                ActivateConstraint();
                RequestSerialization();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other == Parent)
            {
                InTrigger = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other == Parent)
            {
                InTrigger = false;
            }
        }

        public override void OnDeserialization()
        {
            if (Following)
            {
                ActivateConstraint();
            }
            else
            {
                Constraint.constraintActive = false;
            }
        }

        void PrepareConstraint()
        {
            var positionDelta = transform.position - Parent.transform.position;
            PositionOffset = Quaternion.Inverse(Parent.transform.rotation) * positionDelta;
            /*
            var inverse = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
            PositionOffset = inverse.MultiplyPoint3x4(transform.position);
            */
            RotationOffset = (Quaternion.Inverse(Parent.transform.rotation) * transform.rotation).eulerAngles;
        }

        void ActivateConstraint()
        {
            Constraint.SetTranslationOffset(0, PositionOffset);
            Constraint.SetRotationOffset(0, RotationOffset);
            Constraint.constraintActive = true;
        }
    }
}
