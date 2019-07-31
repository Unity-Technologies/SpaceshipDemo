using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRig : MonoBehaviour
{
    public Vector3 Frequency = new Vector3(4,5,6);
    public Vector3 Amplitude = new Vector3(0.0f, 0.2f, 0.0f);

    Vector3 m_InitialPosition;

    private void Awake()
    {
        m_InitialPosition = transform.position;
    }

    private void Update()
    {
        float t = Time.time;
        transform.position = m_InitialPosition + new Vector3(Mathf.Sin(t * Frequency.x) * Amplitude.x, Mathf.Sin(t * Frequency.y) * Amplitude.y, Mathf.Sin(t * Frequency.z) * Amplitude.z);
    }
}
