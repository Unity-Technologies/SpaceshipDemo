using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayIngredients
{
    public class Factory : MonoBehaviour
    {
        public enum BlueprintSelectionMode
        {
            Random,
            Sequential,
            Shuffle,
            GameSave
        }

        public enum SpawnTargetSelection
        {
            OneSequential,
            OneRandom,
            All
        }

        public enum SpawnLocation
        {
            Default,
            SameSceneAsTarget,
            ChildOfTarget,
            DontDestroyOnLoad
        }

        [Header("Blueprint")]
        [ReorderableList, NonNullCheck]
        public GameObject[] FactoryBlueprints;
        public BlueprintSelectionMode blueprintSelecionMode = BlueprintSelectionMode.Random;
        [ShowIf("usesGameSave")]
        public GameSaveManager.Location gameSaveLocation = GameSaveManager.Location.User;
        [ShowIf("usesGameSave")]
        public string gameSaveVariableName = "FactoryBPIndex";
        [ShowIf("usesGameSave")]
        public int defaultGameSaveIndex = 0;

        [Header("Spawn Target")]
        [NonNullCheck]
        public GameObject SpawnTarget;
        public SpawnLocation spawnLocation = SpawnLocation.SameSceneAsTarget;
        [Tooltip("Sacrifices oldest instance if necessary")]
        public bool SacrificeOldest = false;

        [Header("Reap and Respawn")]
        public bool RespawnTarget = true;
        public float RespawnDelay = 3.0f;
        public bool ReapInstancesOnDestroy = true;

        [Min(1), SerializeField]
        private int MaxInstances = 1;

        [ReorderableList]
        public Callable[] OnSpawn;
        [ReorderableList]
        public Callable[] OnRespawn;

        List<GameObject> m_Instances;

        private void OnEnable()
        {
            if(m_Instances != null)
        	    m_Instances.RemoveAll(item => item == null);
        }

        private void OnDestroy()
        {
            if(ReapInstancesOnDestroy && m_Instances != null)
            {
                foreach(var instance in m_Instances)
                {
                    if (instance != null)
                        Destroy(instance);
                }
            }
        }

        bool usesGameSave()
        {
            return blueprintSelecionMode == BlueprintSelectionMode.GameSave;
        }

        public void SetTarget(GameObject target)
        {
            if(target != null)
            {
                SpawnTarget = target;
            }
        }

        public GameObject GetInstance(int index)
        {
            if (m_Instances != null && m_Instances.Count > index)
                return m_Instances[index];
            else
                return null;
        }

        public void Spawn()
        {
            if(SpawnTarget == null || FactoryBlueprints == null  || FactoryBlueprints.Length == 0)
            {
                Debug.LogWarning(string.Format("Factory '{0}' : Cannot spawn as there are no spawn target or factory blueprints", gameObject.name));
                return;
            }

            if (m_Instances == null)
                m_Instances = new List<GameObject>();

            if(m_Instances.Count == MaxInstances && SacrificeOldest)
            {
                var oldest = m_Instances[0];
                m_Instances.RemoveAt(0);
                Destroy(oldest);
            }

            if (m_Instances.Count < MaxInstances)
            {
                GameObject newInstance = Spawn(SelectBlueprint(), SpawnTarget);

                switch(spawnLocation)
                {
                    case SpawnLocation.Default:
                        break;
                    case SpawnLocation.SameSceneAsTarget:
                        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene( newInstance, SpawnTarget.scene);
                        break;
                    case SpawnLocation.ChildOfTarget:
                        newInstance.transform.parent = SpawnTarget.transform;
                        break;
                    case SpawnLocation.DontDestroyOnLoad:
                        DontDestroyOnLoad(newInstance);
                        break;
                }

                m_Instances.Add(newInstance);
                
                Callable.Call(OnSpawn, newInstance);
            }

        }

        private void LateUpdate()
        {
            if(m_Instances != null)
            {
                List<int> todelete = new List<int>();
                for(int i = 0; i < m_Instances.Count; i++)
                {
                    if(m_Instances[i] == null)
                    {
                        todelete.Add(i);
                    }
                }

                foreach (var index in todelete)
                {
                    m_Instances.RemoveAt(index);

                    if(RespawnTarget)
                        AddRespawnCoroutine();
                }
            }
        }

        private List<Coroutine> m_RespawnCoroutines; 

        private void AddRespawnCoroutine()
        {
            if (m_RespawnCoroutines == null)
                m_RespawnCoroutines = new List<Coroutine>();
            else
            {
                m_RespawnCoroutines.RemoveAll(o => o == null);
            }

            m_RespawnCoroutines.Add(StartCoroutine(Respawn(RespawnDelay)));
        }

        private IEnumerator Respawn(float time)
        {
            yield return new WaitForSeconds(time);
            Callable.Call(OnRespawn, this.gameObject);
            Spawn();
        }

        private GameObject Spawn(GameObject blueprint, GameObject target)
        {
            var Go = Instantiate(blueprint, target.transform.position, target.transform.rotation);
            Go.name = (blueprint.name);
            return Go;
        }

        int currentBlueprintIndex = -1;

        private GameObject SelectBlueprint()
        {
            if(FactoryBlueprints == null || FactoryBlueprints.Length == 0)
            {
                Debug.LogError($"Factory '{gameObject.name}' could not spawn anything as there are no blueprints set up");
                return null;
            }

            switch(blueprintSelecionMode)
            {
                default:
                case BlueprintSelectionMode.Random:
                    currentBlueprintIndex = Random.Range(0, FactoryBlueprints.Length);
                    break;
                case BlueprintSelectionMode.Sequential:
                    currentBlueprintIndex = (currentBlueprintIndex++) % FactoryBlueprints.Length;
                    break;
                case BlueprintSelectionMode.Shuffle:
                    currentBlueprintIndex = Shuffle(currentBlueprintIndex);
                    break;
                case BlueprintSelectionMode.GameSave:
                    currentBlueprintIndex = GetFromGameSave();
                    break;
            }
            return FactoryBlueprints[currentBlueprintIndex];
        }

        List<int> shuffleIndices;

        private int Shuffle(int i)
        {
            if(shuffleIndices == null || shuffleIndices.Count != FactoryBlueprints.Length)
            {
                shuffleIndices = Enumerable.Range(0, FactoryBlueprints.Length).OrderBy(x => Random.value).ToList();
            }
            return shuffleIndices[(shuffleIndices.IndexOf(i) + 1) % shuffleIndices.Count];
        }

        private int GetFromGameSave()
        {
            var gsm = Manager.Get<GameSaveManager>();
            int index = -1;
            if(gsm.HasInt(gameSaveVariableName, gameSaveLocation))
            {
                index = gsm.GetInt(gameSaveVariableName, gameSaveLocation);
            }
            else
            {
                index = defaultGameSaveIndex;
            }

            return Mathf.Clamp(index, 0, FactoryBlueprints.Length - 1);
        }
    }
}
