using UnityEngine;

namespace GameplayIngredients
{
    [ExecuteAlways]
    public class Folder : MonoBehaviour
    {
        public Color Color = Color.yellow;

#if UNITY_EDITOR
        private void Awake()
        {
            Reset();
        }
        private void Reset()
        {
            gameObject.isStatic = true;

            if(Application.isPlaying)
            {
                Destroy(this);
            }
        }
#endif
    }
}

