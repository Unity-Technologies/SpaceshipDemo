using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameplayIngredients.Editor
{
    public class NullMaterialCheck : Check
    {
        public override string name => "Renderer/Null Material";

        public override bool defaultEnabled => true;

        public override string[] ResolutionActions => new string[] { "Assign Default Material" };

        public override int defaultResolutionActionIndex => 0;

        public override IEnumerable<CheckResult> GetResults(SceneObjects sceneObjects)
        {
            foreach(var obj in sceneObjects.allObjects)
            {
                if(obj.TryGetComponent(out Renderer r))
                {
                    foreach(var mat in r.sharedMaterials)
                    {
                        if(mat == null)
                        {
                            yield return new CheckResult(this, CheckResult.Result.Failed, $"Missing Material for Object {obj.name}", obj);
                            break;
                        }
                    }
                }
            }
        }

        static Material s_DefaultMaterial;

        public override void Resolve(CheckResult result)
        {
            if(s_DefaultMaterial == null)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                s_DefaultMaterial = cube.GetComponent<Renderer>().sharedMaterial;
                Object.DestroyImmediate(cube);
            }

            switch (result.resolutionActionIndex)
            {
                default:
                    var renderer = (result.mainObject as GameObject).GetComponent<Renderer>();
                    
                    for(int i = 0; i<renderer.sharedMaterials.Length; i++)
                    {
                        if(renderer.sharedMaterials[i] == null)
                        {
                            renderer.materials[i] = s_DefaultMaterial;
                        }
                    }
                    break;
            }
        }
    }
}
