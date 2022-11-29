using UnityEngine;

namespace EasyClick
{
    public class RacerEntity : MonoBehaviour
    {
        [SerializeField] RacerEntityCollection _allRacers;
        [SerializeField] GameObject _thisPrefab;
        string _name;
        bool _isPlayer;

        IBody _body;
        BuffableEntity _buffableEntity;

        public string Name => _name;
        public bool IsPlayer => _isPlayer;
        public IBody Body => _body;
        public GameObject Prefab => _thisPrefab;
        public BuffableEntity BuffableEntity => _buffableEntity;

        void Awake()
        {
            _body = GetComponent<IBody>();
            LevelLoader.OnBeforeLevelUnload += HandleOnBeforeLevelUnload;
            _isPlayer = TryGetComponent<PlayerInput>(out var _);
            _buffableEntity = GetComponent<BuffableEntity>();
            _allRacers.Add(this);
        }

        void OnDestroy()
        {
            LevelLoader.OnBeforeLevelUnload -= HandleOnBeforeLevelUnload;
            _allRacers.Remove(this);
        }

        void HandleOnBeforeLevelUnload()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(this);
        }

        public void ChangeName(string name)
        {
            _name = name;
        }
    }
}
