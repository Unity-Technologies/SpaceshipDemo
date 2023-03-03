using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;

namespace GameplayIngredients.Editor
{
    public class CheckResult
    {
        public enum Result
        {
            Notice,
            Warning,
            Failed
        }

        public Check check { get; private set; }
        public Result result { get; private set; }
        public Object mainObject { get { if (objects != null && objects.Length > 0) return objects[0]; else return null; } }
        Object[] objects;
        public GUIContent message;


        public int resolutionActionIndex;
        public string action { get { return check.ResolutionActions[resolutionActionIndex]; } }
        public CheckResult(Check check, Result result, string message, params Object[] objects)
        {
            this.check = check;
            this.result = result;
            this.message = new GUIContent(message, GetIcon(result));
            this.objects = objects;
        }

        public static Texture GetIcon(Result r)
        {
            switch (r)
            {
                default:
                case Result.Notice:
                    return Contents.notice;
                case Result.Warning:
                    return Contents.warning;
                case Result.Failed:
                    return Contents.failed;

            }
        }

        static class Contents
        {
            public static Texture notice;
            public static Texture warning;
            public static Texture failed;

            static Contents()
            {
                notice = EditorGUIUtility.IconContent("console.infoicon.sml").image;
                warning = EditorGUIUtility.IconContent("console.warnicon.sml").image;
                failed = EditorGUIUtility.IconContent("console.erroricon.sml").image;

            }
        }
        public void SetIgnored(bool ignored = true)
        {
            if (!(mainObject is GameObject))
                return;

            GameObject go = mainObject as GameObject;
            Scene scene = go.scene;
            var ignoredResults = Object.FindObjectsOfType<IgnoredCheckResults>();
            IgnoredCheckResults targetResults = null;

            foreach(var result in ignoredResults)
            {
                if(result.gameObject.scene == scene)
                {
                    targetResults = result;
                    break;
                }
            }
            
            if(targetResults == null)
            {
                var newGo = new GameObject("__IGNORED_CHECK_RESULTS__");
                newGo.hideFlags = HideFlags.HideInHierarchy;
                SceneManager.MoveGameObjectToScene(newGo,scene);
                targetResults = newGo.AddComponent<IgnoredCheckResults>();
            }

            if (targetResults.ignoredCheckResults == null)
                targetResults.ignoredCheckResults = new List<IgnoredCheckResults.IgnoredCheckResult>();

            if(ignored)
            {
                if(!targetResults.ignoredCheckResults.Any(o => o.gameObject == go && o.check == check.GetType().ToString()))
                {
                    IgnoredCheckResults.IgnoredCheckResult r = new IgnoredCheckResults.IgnoredCheckResult()
                    {
                        check = check.GetType().ToString(),
                        gameObject = go
                    };
                    targetResults.ignoredCheckResults.Add(r);
                }
            }
            else
            {
                if (targetResults.ignoredCheckResults.Any(o => o.gameObject == go && o.check == check.GetType().ToString()))
                {
                    targetResults.ignoredCheckResults.Remove(targetResults.ignoredCheckResults.FirstOrDefault(o => o.gameObject == go && o.check == check.GetType().ToString()));
                }
            }
        }
    }
}
