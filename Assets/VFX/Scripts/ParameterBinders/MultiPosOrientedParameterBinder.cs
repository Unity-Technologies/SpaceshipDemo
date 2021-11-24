//MultiPosOrientedParameterBinder
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace UnityEngine.VFX.Utility
{
    [AddComponentMenu("VFX/Utilities/Parameters/VFX Multiple Position (Oriented) Binder")]
    [VFXBinder("Point Cache/Multiple Position (Oriented) Binder")]
    public class MultiPosOrientedParameterBinder : VFXBinderBase
    {
        [VFXPropertyBinding("UnityEngine.Texture2D"), FormerlySerializedAs("PositionMapParameter")]
        public ExposedProperty PositionMapProperty = "PositionMap";
        [VFXPropertyBinding("UnityEngine.Texture2D"), FormerlySerializedAs("DirectionMapParameter")]
        public ExposedProperty DirectionMapProperty = "DirectionMap";
        [VFXPropertyBinding("System.Int32"), FormerlySerializedAs("PositionCountParameter")]
        public ExposedProperty PositionCountProperty = "PositionCount";

        public Transform[] Targets;
        public bool EveryFrame = false;

        public enum DirectionAxis { X, Y, Z }
        public DirectionAxis Axis = DirectionAxis.Y;

        private Texture2D positionMap;
        private Texture2D directionMap;
        private int count = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateTextures();
        }

        public override bool IsValid(VisualEffect component)
        {
            return Targets != null &&
                component.HasTexture(PositionMapProperty) &&
                component.HasTexture(DirectionMapProperty) &&
                component.HasInt(PositionCountProperty);
        }

        public override void UpdateBinding(VisualEffect component)
        {
            if (EveryFrame || Application.isEditor)
                UpdateTextures();

            component.SetTexture(PositionMapProperty, positionMap);
            component.SetTexture(DirectionMapProperty, directionMap);
            component.SetInt(PositionCountProperty, count);
        }

        void UpdateTextures()
        {
            if (Targets == null || Targets.Length == 0)
                return;

            var candidates = new List<Transform>();

            foreach (var obj in Targets)
            {
                if (obj != null)
                    candidates.Add(obj.transform);
            }

            count = candidates.Count;

            if (positionMap == null) 
            {
                positionMap = new Texture2D(count, 1, TextureFormat.RGBAFloat, false);
                directionMap = new Texture2D(count, 1, TextureFormat.RGBAHalf, false);
            }
            else if (positionMap.width != count)
            {
                positionMap.Reinitialize(count, 1);
                directionMap.Reinitialize(count, 1);
            }

            List<Color> positions = new List<Color>();
            List<Color> directions = new List<Color>();

            foreach (var transform in candidates)
            {
                var pos = transform.position;
                Vector3 dir;
                switch(Axis)
                {
                    default:
                    case DirectionAxis.X:
                        dir = transform.right;
                        break;
                    case DirectionAxis.Y:
                        dir = transform.up;
                        break;
                    case DirectionAxis.Z:
                        dir = transform.forward;
                        break;
                }

                positions.Add(new Color(pos.x, pos.y, pos.z));
                directions.Add(new Color(dir.x, dir.y, dir.z));
            }

            positionMap.name = gameObject.name + "_PositionMap";
            positionMap.filterMode = FilterMode.Point;
            positionMap.wrapMode = TextureWrapMode.Repeat;
            positionMap.SetPixels(positions.ToArray(), 0);
            positionMap.Apply();

            directionMap.name = gameObject.name + "_DirectionMap";
            directionMap.filterMode = FilterMode.Point;
            directionMap.wrapMode = TextureWrapMode.Repeat;
            directionMap.SetPixels(directions.ToArray(), 0);
            directionMap.Apply();
        }

        public override string ToString()
        {
            return string.Format("Multiple Position (Oriented) Binder ({0} objects)", count);
        }
    }
}

