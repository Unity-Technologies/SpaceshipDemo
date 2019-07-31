using GameplayIngredients;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCameraManagerRig : MonoBehaviour
{
    [Header("Orientation")]
    public bool UseUpAxis = true;
    public Vector3 UpAxis = Vector3.up;

    [Header("Motion")]
    public float MotionBlend = 2.0f;

    Vector3 m_InterpolatedPosition;

    public void Start()
    {
        m_InterpolatedPosition = Manager.Get<VirtualCameraManager>().transform.position;
    }

    public void Update()
    {
        var newPosition = Manager.Get<VirtualCameraManager>().transform.position;
        m_InterpolatedPosition = Vector3.Lerp(m_InterpolatedPosition, newPosition, MotionBlend * Time.deltaTime);

        if (UseUpAxis)
            transform.LookAt(m_InterpolatedPosition, UpAxis);
        else
            transform.LookAt(m_InterpolatedPosition);
    }

}
