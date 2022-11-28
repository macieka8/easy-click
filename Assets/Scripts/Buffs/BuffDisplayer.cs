using UnityEngine;
using UnityEngine.UI;

namespace EasyClick
{
    public class BuffDisplayer : MonoBehaviour
    {
        [SerializeField] Slider _progress;
        [SerializeField] Image _icon;
        TimedBuff _buffToDisplay;

        public void Setup(TimedBuff buff)
        {
            _buffToDisplay = buff;
            _icon.sprite = _buffToDisplay.BuffData.Icon;
        }

        void Update()
        {
            _progress.value = _buffToDisplay.TimeLeft / _buffToDisplay.TimedBuffData.Duration;
            if (_buffToDisplay == null || _progress.value <= 0f)
                Destroy(gameObject);
        }
    }
}
