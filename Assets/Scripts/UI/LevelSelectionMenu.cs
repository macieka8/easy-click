using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EasyClick
{
    public class LevelSelectionMenu : MonoBehaviour
    {
        [SerializeField] GameObject _MenuEntryPrefab;
        [SerializeField] string[] _LevelNames;

        void Awake()
        {
            foreach (var levelName in _LevelNames)
            {
                var entry = Instantiate(_MenuEntryPrefab, transform);
                entry.GetComponent<Button>().onClick.AddListener(() => { LevelLoader.StartLevel(levelName); });

                entry.transform.Find("Text").gameObject
                    .GetComponent<TextMeshProUGUI>()
                    .text = levelName.Substring(levelName.IndexOf(" ") + 1);
            }
        }
    }
}