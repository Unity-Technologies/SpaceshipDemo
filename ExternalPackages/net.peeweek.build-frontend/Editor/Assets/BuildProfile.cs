using UnityEngine;
using UnityEditor;

namespace BuildFrontend
{
    public class BuildProfile : BuildFrontendAssetBase
    {
        [Header("Build Profile")]
        public bool DevPlayer;
        public BuildTarget Target;
    }

}

