using UnityEngine;


namespace GameplayIngredients.Rigs
{
    [AddComponentMenu(ComponentMenu.rigsPath + "Floating Rig")]
    public class FloatingRig : Rig
    {
        public override int defaultPriority => 0;
        public override UpdateMode defaultUpdateMode =>  UpdateMode.Update;

        public Vector3 Frequency = new Vector3(4, 5, 6);
        public Vector3 Amplitude = new Vector3(0.0f, 0.2f, 0.0f);

        Vector3 m_InitialPosition;

        private void Awake()
        {
            m_InitialPosition = transform.position;
        }

        public override void UpdateRig(float deltaTime)
        {
            float t = (updateMode == UpdateMode.FixedUpdate)? Time.fixedTime : Time.time;
            transform.position = m_InitialPosition + new Vector3(Mathf.Sin(t * Frequency.x) * Amplitude.x, Mathf.Sin(t * Frequency.y) * Amplitude.y, Mathf.Sin(t * Frequency.z) * Amplitude.z);
        }
    }
}

