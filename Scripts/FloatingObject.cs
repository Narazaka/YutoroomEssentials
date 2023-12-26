using System;
using UdonSharp;
using UnityEngine;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    namespace UdonFloatingObject
    {
        [RequireComponent(typeof(Collider))]
        [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
        public class FloatingObject : UdonSharpBehaviour
        {
            [SerializeField]
            Collider[] Colliders;
            [SerializeField, Tooltip("required for Top")]
            Collider[] AirColliders;
            [SerializeField]
            Transform[] ColliderTops;
            [SerializeField]
            Rigidbody Target;
            [SerializeField]
            Transform TopPosition;
            [SerializeField]
            Transform BottomPosition;
            [SerializeField]
            Transform CenterOfMass;
            [SerializeField]
            Transform StableFloatPosition;
            [SerializeField]
            bool CanInvert;
            [SerializeField, Tooltip("required for Top / CanInvert")]
            Transform CenterOfMassInvert;
            [SerializeField, Tooltip("required for Top / CanInvert")]
            Transform StableFloatPositionInvert;
            [SerializeField, Tooltip("required for Top / CanInvert")]
            Transform UpperDirection;
            [SerializeField, Tooltip("required for Top / CanInvert")]
            float BottomUpAngle;

            [SerializeField]
            float FloatingForceOffsetFactor;
            [SerializeField]
            float FloatingForceFactor;
            [SerializeField]
            float AngularDampeningFactor;
            [SerializeField]
            float InFluidDrag;

            bool IsInFluid;
            // only true if Top
            bool IsTopInFluid;
            // only true if Top
            bool IsTopCompletelyInFluid;
            // only true if Top
            bool NoAir;
            float FluidTopY;

            // for debug
            /* public */
            bool inverted;
            /* public */
            float bottomY;
            /* public */
            float topY;
            /* public */
            float stableY;
            /* public */
            float lowerY;
            /* public */
            float upperY;
            /* public */
            float differenceFromLower;
            /* public */
            float differenceFromStable;
            /* public */
            float lowerLen;
            /* public */
            float rate;


            void OnEnable()
            {
                var collider = GetComponent<Collider>();
            }

            void OnTriggerEnter(Collider other)
            {
                var i = Array.IndexOf(Colliders, other);
                if (i != -1)
                {
                    IsInFluid = true;
                    FluidTopY = ColliderTops[i].position.y;
                }
            }

            void OnTriggerExit(Collider other)
            {
                var i = Array.IndexOf(Colliders, other);
                if (i != -1)
                {
                    IsInFluid = false;
                }
            }

            public void OnTopTriggerEnter(Collider other)
            {
                var i = Array.IndexOf(Colliders, other);
                if (i != -1)
                {
                    IsTopInFluid = true;
                }
                i = Array.IndexOf(AirColliders, other);
                if (i != -1)
                {
                    IsTopCompletelyInFluid = false;
                    NoAir = false;
                }
            }

            public void OnTopTriggerExit(Collider other)
            {
                var i = Array.IndexOf(Colliders, other);
                if (i != -1)
                {
                    IsTopInFluid = false;
                }
                i = Array.IndexOf(AirColliders, other);
                if (i != -1)
                {
                    IsTopCompletelyInFluid = true;
                }
            }

            void FixedUpdate()
            {
                if (IsInFluid && Target.drag == 0)
                {
                    Target.drag = InFluidDrag;
                }
                else if (!IsInFluid && Target.drag != 0)
                {
                    Target.drag = 0;
                }

                if (IsInFluid)
                {
                    inverted = (CanInvert || (IsTopCompletelyInFluid && !NoAir)) && IsInverted(); // CanInvertでなくかつしずんで空気が無い場合は計算する必要がない

                    bottomY = (inverted ? TopPosition : BottomPosition).position.y;
                    topY = (inverted ? BottomPosition : TopPosition).position.y;
                    var topYIsUpper = topY > bottomY;
                    lowerY = topYIsUpper ? bottomY : topY;

                    differenceFromLower = FluidTopY - lowerY; // invert対応にかかわらず下の方の値を常に使う
                    if (differenceFromLower > 0)
                    {
                        stableY = (inverted ? StableFloatPositionInvert : StableFloatPosition).position.y;
                        upperY = topYIsUpper ? topY : bottomY;

                        differenceFromStable = FluidTopY - stableY;
                        lowerLen = Mathf.Abs(stableY - bottomY);
                        rate = differenceFromStable < 0
                            ? FloatingForceOffsetFactor + ((1f - FloatingForceOffsetFactor) * (differenceFromLower / lowerLen))
                            : 1f + ((((FluidTopY > upperY ? Mathf.Abs(topY - bottomY) : differenceFromLower) / lowerLen) - 1f) * FloatingForceFactor);
                        if (!IsTopInFluid)
                        {
                            Target.AddForceAtPosition(-Physics.gravity * rate * Target.mass, inverted ? CenterOfMassInvert.position : CenterOfMass.position);
                        }
                        else if (IsTopCompletelyInFluid && !NoAir)
                        {
                            if (inverted)
                            {
                                Target.AddForceAtPosition(-Physics.gravity * rate * Target.mass, CenterOfMassInvert.position);
                            }
                            else
                            {
                                NoAir = true;
                            }
                        }
                        Target.AddTorque(-Target.angularVelocity * AngularDampeningFactor);
                    }
                }
            }

            bool IsInverted()
            {
                Quaternion.FromToRotation(Vector3.up, UpperDirection.up).ToAngleAxis(out var angle, out var axis);
                return angle >= BottomUpAngle;
            }
        }
    }
}
