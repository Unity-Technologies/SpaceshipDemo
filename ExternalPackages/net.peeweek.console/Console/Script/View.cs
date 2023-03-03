using UnityEngine;

namespace ConsoleUtility
{
    /// <summary>
    /// Represents a view that can be added to the console.
    /// It initializes with an update rate and will be updated upon being visible.
    /// </summary>
    public abstract class View
    {
        float m_UpdateRate;
        float m_TTL;

        /// <summary>
        /// Creates a new View Object
        /// </summary>
        /// <param name="updateRate">The desired update frequency (default 10/s)</param>
        public View(float updateRate = 10f)
        {
            m_UpdateRate = updateRate;
            m_TTL = 0;
        }


        /// <summary>
        /// Updates the View and returns whether to redraw it.
        /// </summary>
        /// <returns>Whether the text should be updated in the console</returns>
        public virtual bool Update()
        {
            if(m_TTL < 0)
            {
                m_TTL = 1.0f / m_UpdateRate;
                return true;
            }
            else
            {
                m_TTL -= Time.unscaledDeltaTime;
                return false;
            }
        }

        /// <summary>
        /// Implement this method that Returns the String to be displayed by the View
        /// </summary>
        /// <returns>The String to be displayed</returns>
        public abstract string GetDebugViewString();

        /// <summary>
        /// Called Upon Creation of the Debug View (while being constructed)
        /// </summary>
        public virtual void OnCreate() { }

        /// <summary>
        /// Called Upon Destruction of the View
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// Called when the View becomes visible
        /// </summary>
        public virtual void OnEnable() 
        {
            m_TTL = 0.0f;
        }

        /// <summary>
        /// Called when the View becomes invisible
        /// </summary>
        public virtual void OnDisable() { }

    }
}
