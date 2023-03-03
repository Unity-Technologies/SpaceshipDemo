using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine.Events;

namespace GameplayIngredients.Editor
{
    public class InvalidTransformCheck : Check
    {
        public override string name => "GameObject/Invalid Transform";

        public override bool defaultEnabled => true;

        public override string[] ResolutionActions => new string[] { "Do Nothing", "Reset", "Delete Object" };

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
                    if (EditorUtility.DisplayCancelableProgressBar("Finding Transforms...", $"{go.name}", progress))
                    {
                        break;
                    }

                    Transform t = go.transform;
                    if(t.localScale.sqrMagnitude == 0)
                    {
                        yield return new CheckResult(this, CheckResult.Result.Warning, $"Transform of Game Object \"{go.name}\" has zero Scale.", go);
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
            throw new System.NotImplementedException();
        }
    }
}

