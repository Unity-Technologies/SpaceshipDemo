using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{

    public class FullScreenFadeAction : ActionBase
    {
        public FullScreenFadeManager.FadeMode Fading = FullScreenFadeManager.FadeMode.ToBlack;
        public float Duration = 2.0f;

        [ReorderableList]
        public Callable[] OnComplete;

        public override void Execute(GameObject instigator = null)
        {
            Manager.Get<FullScreenFadeManager>().Fade(Duration, Fading, OnComplete, instigator);
        }
    }

}
