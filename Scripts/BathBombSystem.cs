using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace net.narazaka.vrchat.yutoroom_essentials
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BathBombSystem : UdonSharpBehaviour
    {
        [SerializeField]
        Collider[] BathBombs;
        [SerializeField]
        Material[] Materials;
        [SerializeField]
        Renderer Renderer;
        [SerializeField]
        Transform[] CorrespondingObjects;

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
