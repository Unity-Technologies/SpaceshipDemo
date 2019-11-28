using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    public class Timer : MonoBehaviour
    {
        public bool StartOnEnable = false;

        public uint Hours = 0;
        [Range(0,59)]
        public uint Minutes = 1;
        [Range(0,59)]
        public uint Seconds = 30;
        [Range(0,999)]
        public uint Milliseconds = 0;

        public uint CurrentHours         { get { return (uint)Mathf.Floor(m_TTL / 3600); } }
        public uint CurrentMinutes       { get { return (uint)Mathf.Floor(m_TTL / 60) % 60; } }
        public uint CurrentSeconds       { get { return (uint)Mathf.Floor(m_TTL) % 60; } }
        public uint CurrentMilliseconds  { get { return (uint)((m_TTL % 1.0f) * 1000); } }


        [ReorderableList]
        public Callable[] OnTimerFinished;
        [ReorderableList]
        public Callable[] OnTimerInterrupt;
        [ReorderableList]
        public Callable[] OnTimerStart;

        float m_TTL = 0.0f;

        public bool isRunning => m_TTL > 0.0f;

        public void OnEnable()
        {
            if (StartOnEnable)
                Restart();
            else
                m_TTL = 0.0f;
        }


        public void Restart(GameObject instigator = null)
        {
            m_TTL = Hours * 3600 + Minutes * 60 + Seconds + Milliseconds * 0.001f;
            Callable.Call(OnTimerStart, instigator);
        }

        public void Update()
        {
            if(m_TTL > 0.0f)
            {
                m_TTL -= Time.deltaTime;
                if (m_TTL <= 0.0f)
                {
                    m_TTL = 0.0f;
                    Callable.Call(OnTimerFinished);
                }
            }
        }

        public void Interrupt(GameObject instigator = null)
        {
            if(m_TTL > 0.0f)
            {
                m_TTL = 0.0f;
                Callable.Call(OnTimerInterrupt, instigator);
            }
        }
    }
}
