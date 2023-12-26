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
            [SerializeField, Header("水中領域")]
            Collider[] Colliders;
            [SerializeField, Header("水面直上の大気領域（FloatingObjectTopを指定する場合必要）"), Tooltip("required for FloatingObjectTop")]
            Collider[] AirColliders;
            [SerializeField, Header("水面高さ")]
            Transform[] ColliderTops;
            [SerializeField, Header("対象RigidBody")]
            Rigidbody Target;
            [SerializeField, Header("上面位置")]
            Transform TopPosition;
            [SerializeField, Header("下面位置")]
            Transform BottomPosition;
            [SerializeField, Header("重心（ここに力がかかる）")]
            Transform CenterOfMass;
            [SerializeField, Header("浮遊平衡位置")]
            Transform StableFloatPosition;
            [SerializeField, Header("逆向き浮遊（FloatingObjectTop構成では指定しない）")]
            bool CanInvert;
            [SerializeField, Header("逆向きでの重心（ここに力がかかる）"), Tooltip("required for FloatingObjectTop / CanInvert")]
            Transform CenterOfMassInvert;
            [SerializeField, Header("逆向きでの浮遊平衡位置"), Tooltip("required for FloatingObjectTop / CanInvert")]
            Transform StableFloatPositionInvert;
            [SerializeField, Header("上面方向（Yが上に向くように配置する 優勢面判定用）"), Tooltip("required for FloatingObjectTop / CanInvert")]
            Transform UpperDirection;
            [SerializeField, Header("逆向きが優勢になる回転角度(度)"), Tooltip("required for FloatingObjectTop / CanInvert")]
            float BottomUpAngle;

            [SerializeField, Header("沈み割合に関係しない浮力割合")]
            float FloatingForceOffsetFactor;
            [SerializeField, Header("浮力係数（重力加速度比）")]
            float FloatingForceFactor;
            [SerializeField, Header("水中回転減衰係数")]
            float AngularDampeningFactor;
            [SerializeField, Header("水中での摩擦係数(drag)")]
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
