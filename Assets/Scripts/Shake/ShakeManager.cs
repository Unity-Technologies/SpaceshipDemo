using Cinemachine;
using GameplayIngredients;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ManagerDefaultPrefab("ShakeManager")]
public class ShakeManager : Manager
{
    [Header("CameraShake")]
    public CinemachineImpulseSource CinemachineImpulseSource;

    [Header("Audio")]
    public bool EnableAudioShake = true;
    public AudioSource shakeAudioSource;

    [Header("Settings")]
    public bool LogShakes = false;
    public bool ShakeEnabled = false;
    public float Attenuation = 0.5f;
    public float MinDelay = 0.5f;
    public float MaxDelay = 1.7f;
    public Settings MinSettings = new Settings
    {
        Intensity = 0.1f,
        Position = Vector3.one * -32.0f
    };

    public Settings MaxSettings = new Settings
    {
        Intensity = 1.0f,
        Position = Vector3.one * 32.0f
    };

    List<OnShakeEvent> m_Recievers;
    CinemachineImpulseSource m_ImpulseSource;

    [System.Serializable]
    public struct Settings
    {
        public float Intensity;
        public Vector3 Position;
    }

    private void OnEnable()
    {
        m_Recievers = new List<OnShakeEvent>();
    }

    void DebugLog(string message)
    {
        if(LogShakes)
            Console.Console.Log("ShakeManager", message, LogType.Log);
    }

    public void Shake(Settings shake, bool force = false)
    {
        if (force || Time.time > (lastShakeTime + MinDelay))
        {
            DebugLog($"Shake : {shake.Position} : {shake.Intensity} {(force ? "(Forced)" : "")}");
            lastShakeTime = Time.time;

            Vector3 ShakeVector = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized * shake.Intensity;

            if (CinemachineImpulseSource != null)
            {
                DebugLog($"CinemachineImpulseSource.GenerateImpulseAt({shake.Position},{ShakeVector})");
                CinemachineImpulseSource.GenerateImpulseAt(shake.Position, ShakeVector);
            }

            foreach(var reciever in m_Recievers)
            {
                reciever.Shake(shake, Attenuation);
            }

            if(EnableAudioShake && shakeAudioSource != null)
            {
                shakeAudioSource.transform.position = shake.Position;
                shakeAudioSource.Play();
            }
        }
    }

    Coroutine shakeCoroutine;
    float lastShakeTime;

    private void Update()
    {
        if(ShakeEnabled)
        {
            if (shakeCoroutine == null)
                shakeCoroutine = StartCoroutine(PeriodicShake());
        }
        else
        {
            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
    }

    IEnumerator PeriodicShake()
    {
        yield return new WaitForSeconds(Random.Range(MinDelay, MaxDelay));

        // Compute Shake
        Settings newShake = new Settings
        {
            Intensity = Random.Range(MinSettings.Intensity, MaxSettings.Intensity),
            Position = new Vector3(Random.Range(MinSettings.Position.x, MaxSettings.Position.x), Random.Range(MinSettings.Position.y, MaxSettings.Position.y), Random.Range(MinSettings.Position.z, MaxSettings.Position.z))
        };

        Shake(newShake);
        shakeCoroutine = null;
    }

    public void RegisterShake(OnShakeEvent reciever)
    {
        if(!m_Recievers.Contains(reciever))
            m_Recievers.Add(reciever);
    }

    public void UnRegisterShake(OnShakeEvent reciever)
    {
        if (m_Recievers.Contains(reciever))
            m_Recievers.Remove(reciever);
    }

}
