using UnityEngine;
using AxelF;

[RequireComponent(typeof(Heartbeat))]
public class InitAxelF : MonoBehaviour {

    protected void Awake() {
        Heartbeat.playerTransform = transform;
        Heartbeat.listenerTransform = transform;
    }
}

