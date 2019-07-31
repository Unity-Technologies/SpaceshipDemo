using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    public class FollowPathRig : MonoBehaviour
    {
        public enum PlayMode
        {
            Playing,
            Stopped,
            Reverse
        }

        public enum LoopMode
        {
            Hold,
            Loop,
            PingPong
        }

        public LoopMode loopMode = LoopMode.PingPong;
        public bool StopOnSteps = false;
        public float Speed = 2.0f;

        [ReorderableList, SerializeField, NonNullCheck]
        protected GameObject[] Path;

        public PlayMode initialPlayMode = PlayMode.Playing;
        PlayMode m_PlayMode;
        float m_Progress;

        public void SetProgress(float progress)
        {
            m_Progress = Mathf.Clamp(progress, 0, Path.Length - 1);
        }

        public void Play(float progress = -1.0f)
        {
            if (progress >= 0.0f)
                SetProgress(progress);

            m_PlayMode = PlayMode.Playing;
        }

        public void Reverse(float progress = -1.0f)
        {
            if (progress >= 0.0f)
                SetProgress(progress);

            m_PlayMode = PlayMode.Reverse;

        }

        public void Stop(float progress = -1.0f)
        {
            if (progress >= 0.0f)
                SetProgress(progress);

            m_PlayMode = PlayMode.Stopped;

        }

        private void Start()
        {
            m_PlayMode = initialPlayMode;
            m_Progress = 0.0f;
        }

        private void LateUpdate()
        {
            if(m_PlayMode != PlayMode.Stopped)
            {
                if(Path.Where(o => o == null).Count() > 0)
                {
                    Debug.LogWarning("Path contains null objects. Cannot Compute.", this);
                }

                // Process loopMode and boundary reach
                switch(loopMode)
                {
                    case LoopMode.Hold:
                        if((m_PlayMode == PlayMode.Playing && m_Progress == Path.Length - 1) || (m_PlayMode == PlayMode.Reverse && m_Progress == 0.0f))
                        {
                            m_PlayMode = PlayMode.Stopped;
                            return;
                        }
                        break;
                    case LoopMode.Loop:
                        if (m_PlayMode == PlayMode.Playing && m_Progress == Path.Length -1)
                        {
                            m_Progress = 0.0f;
                        }
                        else if (m_PlayMode == PlayMode.Reverse && m_Progress == 0.0f)
                        {
                            m_Progress = Path.Length -1;
                        }
                        break;
                    case LoopMode.PingPong:
                        if (m_PlayMode == PlayMode.Playing && m_Progress == Path.Length -1)
                        {
                            m_PlayMode = PlayMode.Reverse;
                        }
                        else if (m_PlayMode == PlayMode.Reverse && m_Progress == 0.0f)
                        {
                            m_PlayMode = PlayMode.Playing;
                        }
                        break;
                }

                // Process move on path

                float sign = 1.0f;

                if (m_PlayMode == PlayMode.Reverse)
                    sign = -1.0f;

                int idx = Mathf.Clamp( sign > 0? (int)Mathf.Floor(m_Progress) : (int)Mathf.Ceil(m_Progress), 0 , Path.Length-1);
                int nextidx = idx + (int)sign;

                Vector3 inPos = Path[idx].transform.position;
                Vector3 outPos = Path[nextidx].transform.position;

                Vector3 dir = ( outPos - inPos ).normalized;
                Vector3 pos = Vector3.Lerp( sign > 0? inPos : outPos, sign > 0? outPos : inPos, m_Progress % 1.0f);
                Vector3 move = dir * Speed * Time.deltaTime;
                float moveT = move.magnitude / (outPos - inPos).magnitude * sign;

                m_Progress = Mathf.Clamp(m_Progress + moveT, (sign > 0)? idx : nextidx, (sign > 0) ? nextidx : idx);

                if(m_Progress == nextidx)
                    transform.position = outPos;
                else
                    transform.position = Vector3.Lerp(inPos, outPos, (sign > 0) ? m_Progress % 1.0f : 1.0f - (m_Progress % 1.0f));

                if((m_Progress %1.0f == 0.0f) && StopOnSteps)
                {
                    m_PlayMode = PlayMode.Stopped;
                }

            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
            DrawGizmosPath();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            DrawGizmosPath();
        }

        void DrawGizmosPath()
        {
            if(Path != null && Path.Length > 1 && Path.Where(o => o == null).Count() == 0)
            {
                for(int i = 0; i < Path.Length -1; i++)
                {
                    Gizmos.DrawLine(Path[i].transform.position, Path[i + 1].transform.position);
                }
            }
        }

    }
}

