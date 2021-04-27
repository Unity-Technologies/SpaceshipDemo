using GameplayIngredients;
using GameplayIngredients.Rigs;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class VolumeLookAtControlRig : Rig
{
    public override bool canChangeUpdateMode => true;

    public override UpdateMode defaultUpdateMode => UpdateMode.LateUpdate;

    public override int defaultPriority => 0;

    [NonNullCheck, Tooltip("Target Transform used for the look at computation")]
    public Transform lookAtTarget;
    [Tooltip("Curve used to set the weight (X=0 when target is in our back, X=1 when target is right in front of us)")]
    public AnimationCurve lookAtWeightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    VirtualCameraManager m_VCM;
    Volume m_Volume;

    protected override void OnEnable()
    {
        base.OnEnable();

        Manager.TryGet(out m_VCM);
        TryGetComponent(out m_Volume);
    }


    public override void UpdateRig(float deltaTime)
    {
        if (lookAtTarget == null)
            return;

        Vector3 look = (lookAtTarget.position - m_VCM.transform.position).normalized;
        Vector3 view = m_VCM.transform.forward;
        float w = Vector3.Dot(look, view) * 0.5f + 0.5f;
        m_Volume.weight = lookAtWeightCurve.Evaluate(w);

    }
}
