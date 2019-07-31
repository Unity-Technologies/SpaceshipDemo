using GameplayIngredients;
using UnityEngine;
using Cinemachine;

public class DebugPOVManager : Manager
{
    CinemachineVirtualCamera m_VirtualCamera;

    public void SetCamera(Transform transform)
    {
        if (transform != null)
        {
            if (m_VirtualCamera == null)
            {
                CreateCamera();
            }

            m_VirtualCamera.gameObject.transform.position = transform.position;
            m_VirtualCamera.gameObject.transform.rotation = transform.rotation;
            m_VirtualCamera.gameObject.transform.localScale = Vector3.one;
        }
        else
            DestroyCamera();
    }

    void CreateCamera()
    {
        var go = new GameObject("DebugPOV");
        m_VirtualCamera = go.AddComponent<CinemachineVirtualCamera>();
        m_VirtualCamera.Priority = int.MaxValue;
        m_VirtualCamera.transform.parent = transform;
    }

    void DestroyCamera()
    {
        if(m_VirtualCamera != null)
        {
            Destroy(m_VirtualCamera.gameObject);
            m_VirtualCamera = null;
        }
    }
}
