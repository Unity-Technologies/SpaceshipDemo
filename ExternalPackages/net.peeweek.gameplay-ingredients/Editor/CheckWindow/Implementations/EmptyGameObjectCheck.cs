using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace GameplayIngredients.Editor
{
    public class EmptyGameObjectCheck : Check
    {
        public override string name => "GameObject/Empty";

        public override bool defaultEnabled => true;

        public override string[] ResolutionActions => new string[] { "Set Static", "Delete Object" };

        public override int defaultResolutionActionIndex => 0;

        public override IEnumerable<CheckResult> GetResults(SceneObjects so)
        {
            try
            {
                int count = so.allObjects.Length;
                int i = 0;

                foreach (var go in so.allObjects)
                {
                    float progress = ++i / count;
                    if (EditorUtility.DisplayCancelableProgressBar("Finding Empty Game Objects...", $"{go.name}", progress))
                    {
                        break;
                    }

                    var allComps = go.GetComponents<Component>();
                    if (allComps.Length == 1)
                    {
                        if (!go.isStatic)
                        {
                            if(!so.referencedGameObjects.Contains(go))
                            {
                                var result = new CheckResult(this, CheckResult.Result.Warning, $"Empty Game Object {go.name} is not static", go);
                                result.resolutionActionIndex = 0;
                                yield return result;
                            }
                        }
                        else
                        {
                            if (go.transform.childCount == 0)
                            {
                                if (!so.referencedGameObjects.Contains(go) && !so.referencedComponents.Contains(go.transform))
                                {
                                    var result =  new CheckResult(this, CheckResult.Result.Notice, "Empty Static Game Object is not referenced, and has no children", go);
                                    result.resolutionActionIndex = 1;
                                    yield return result;
                                }
                            }
                        }
                    }
                }

            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public override void Resolve(CheckResult result)
        {
            switch (result.resolutionActionIndex)
            {
                case 0:
                default:
                    (result.mainObject as GameObject).isStatic = true;
                    break;
                case 1:
                    Object.DestroyImmediate(result.mainObject as GameObject);
                    break;
            }
        }
    }
}

