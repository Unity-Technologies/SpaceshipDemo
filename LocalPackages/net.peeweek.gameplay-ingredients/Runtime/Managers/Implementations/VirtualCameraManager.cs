using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace GameplayIngredients
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CinemachineBrain))]
    [ManagerDefaultPrefab("VirtualCameraManager")]
    public class VirtualCameraManager : Manager
    {
        public Camera Camera { get; private set; }
        public CinemachineBrain Brain { get; private set; }

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            Brain = GetComponent<CinemachineBrain>();
        }

    }
}
