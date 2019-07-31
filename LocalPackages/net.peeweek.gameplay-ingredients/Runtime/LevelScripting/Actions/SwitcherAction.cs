using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class SwitcherAction : MonoBehaviour
    {
        public GameObject[] Objects;
        public GameObject Default;

        private void Start()
        {
            if (Default != null)
                SwitchTo(Default);
        }

        public void SwitchTo(GameObject obj)
        {
            foreach (GameObject o in Objects)
            {
                if (o == null) continue;
                if (o == obj)
                {
                    if (!o.activeSelf)
                        o.SetActive(true);

                }
                else
                {
                    if (o.activeSelf)
                        o.SetActive(false);
                }
            }
        }
    }

}
