using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GameplayIngredients;

[ManagerDefaultPrefab("SubtitleManager")]
public class SubtitleManager : Manager
{
    [Serializable]
    public struct Subtitle
    {
        public float Time;
        public string Text;
    }

    [Header("UI")]
    public GameObject SubtitleContainer;
    public TMPro.TMP_Text SubtitleText;


    [Header("Pause")]
    public string PauseMessage = "PAUSE";
    public string UnPauseMessage = "UNPAUSE";

    Subtitle[] m_Subtitles;
    List<Coroutine> m_Coroutines;

    private void OnEnable()
    {
        Messager.RegisterMessage(PauseMessage, OnPause);
        Messager.RegisterMessage(UnPauseMessage, OnUnPause);
    }

    private void OnDisable()
    {
        Messager.RemoveMessage(PauseMessage, OnPause);
        Messager.RemoveMessage(UnPauseMessage, OnUnPause);
    }

    bool bWasVisible;

    void OnPause(GameObject instigator = null)
    {
        bWasVisible = SubtitleContainer.activeSelf;

        if(bWasVisible)
            SubtitleContainer.SetActive(false);
    }

    void OnUnPause(GameObject instigator = null)
    {
        if(bWasVisible)
            SubtitleContainer.SetActive(true);
    }
    public void StopSubtitles()
    {
        if (m_Coroutines == null)
            m_Coroutines = new List<Coroutine>();
        else
        {
            foreach(var c in m_Coroutines) StopCoroutine(c);
            m_Coroutines.Clear();
        }

        SubtitleContainer.SetActive(false);
    }

    public void PlaySubtitles(Subtitle[] subs)
    {
        StopSubtitles();
        m_Subtitles = subs;
        for(int i = 0; i < m_Subtitles.Length; i++)
        {
            if (subs[i].Text != "")
                m_Coroutines.Add(StartCoroutine(SubtitleShowCouroutine(subs[i].Text, subs[i].Time)));
            else
                m_Coroutines.Add(StartCoroutine(SubtitleHideCouroutine(subs[i].Time)));
        }
    }

    private IEnumerator SubtitleHideCouroutine(float time)
    {
        yield return new WaitForSeconds(time);

        if (GameplayIngredientsSettings.currentSettings.verboseCalls)
            Debug.Log("[SubtitleManager] Disabled Subtitle");

        SubtitleContainer.SetActive(false);
        SubtitleText.text = "";
        LayoutRebuilder.MarkLayoutForRebuild(SubtitleContainer.GetComponent<RectTransform>());
    }

    private IEnumerator SubtitleShowCouroutine(string text, float time)
    {
        yield return new WaitForSeconds(time);

        if(GameplayIngredientsSettings.currentSettings.verboseCalls)
            Debug.Log(string.Format("[SubtitleManager] Set Subtitle : '{0}' at time {1}",text,time));

        SubtitleContainer.SetActive(true);
        SubtitleText.text = text;
        LayoutRebuilder.MarkLayoutForRebuild(SubtitleContainer.GetComponent<RectTransform>());
    }
}
