using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BathBombSystem : UdonSharpBehaviour
    {
        [Header("投げ入れたい入浴剤コライダー")]
        [SerializeField]
        Collider[] BathBombs;
        [Header("お湯表面のマテリアル")]
        [SerializeField]
        Material[] Materials;
        [Header("お湯表面")]
        [SerializeField]
        Renderer Renderer;
        [Header("各入浴剤有効時にアクティブにするオブジェクト（水面下PostProcessing等）")]
        [SerializeField]
        Transform[] CorrespondingObjects;

        [Header("入浴剤有効index（変化時のみ反映）")]
        [UdonSynced, FieldChangeCallback(nameof(MaterialIndex))]
        public sbyte _MaterialIndex = -1;
        sbyte MaterialIndex
        {
            get => _MaterialIndex;
            set
            {
                _MaterialIndex = value;
                if (_MaterialIndex != -1)
                {
                    Renderer.material = Materials[(int)_MaterialIndex];
                    var len = CorrespondingObjects.Length;
                    for (var i = 0; i < len; ++i)
                    {
                        CorrespondingObjects[i].gameObject.SetActive(i == (int)_MaterialIndex);
                    }
                }
            }
        }

        Vector3[] InitialPositions;
        Quaternion[] InitialRotations;
        void Start()
        {
            var len = BathBombs.Length;
            InitialPositions = new Vector3[len];
            InitialRotations = new Quaternion[len];
            for (var i = 0; i < len; i++)
            {
                InitialPositions[i] = BathBombs[i].transform.position;
                InitialRotations[i] = BathBombs[i].transform.rotation;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            var index = Array.IndexOf(BathBombs, other);
            if (index != -1)
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                MaterialIndex = (sbyte)index;
                RequestSerialization();
                var bathBomb = BathBombs[index];
                if (Networking.IsOwner(bathBomb.gameObject))
                {
                    if (InitialPositions != null)
                    {
                        bathBomb.transform.position = InitialPositions[index];
                    }
                    if (InitialRotations != null)
                    {
                        bathBomb.transform.rotation = InitialRotations[index];
                    }
                }
            }
        }
    }
}
