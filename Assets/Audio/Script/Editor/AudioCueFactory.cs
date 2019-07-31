using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;

public class AudioCueFactory
{
    [MenuItem("Assets/Create/VFX Demo/AudioCue", priority = 301)]
    private static void MenuCreateAudioCue()
    {
        var icon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAudioCue>(), "AudioCue.asset", icon, null);
    }

    internal static AudioCue CreateAudioCueFactoryAtPath(string path)
    {
        AudioCue asset = ScriptableObject.CreateInstance<AudioCue>();
        asset.name = Path.GetFileName(path);
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

}

internal class DoCreateAudioCue : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        AudioCue asset = AudioCueFactory.CreateAudioCueFactoryAtPath(pathName);
        ProjectWindowUtil.ShowCreatedAsset(asset);
    }
}

