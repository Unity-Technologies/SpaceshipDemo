using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCue : ScriptableObject
{
    public AudioClip Clip;
    public int CueSourceIndex = 0;
    [ReorderableList]
    public SubtitleManager.Subtitle[] Subtitles;
}
