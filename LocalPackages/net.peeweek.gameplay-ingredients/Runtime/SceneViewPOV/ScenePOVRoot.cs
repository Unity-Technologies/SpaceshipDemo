using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameplayIngredients
{
    public class ScenePOVRoot : MonoBehaviour
    {
        public Scene Scene
        {
            get { return gameObject.scene; }
        }

        public Transform[] AllPOV
        {
            get
            {
                List<Transform> all = new List<Transform>();
                for(int i = 0; i< gameObject.transform.childCount; i++)
                    all.Add(gameObject.transform.GetChild(i));
                return all.ToArray();
            }
        }

        public void AddPOV(Transform t, string Name)
        {
            var newPov = new GameObject(Name);
            newPov.transform.position = t.position;
            newPov.transform.rotation = t.rotation;
            newPov.transform.parent = this.transform;
        }
    }
}


