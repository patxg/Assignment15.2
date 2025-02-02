using System.Collections;
using TMPro;
using UnityEngine;

namespace Components
{
//The MenCountLabel shows a current number of men in a specific crowd.
    public class MenCountLabel : MonoBehaviour
    {
        private SpriteRenderer _labelBg;
        private TextMeshPro _label;
        private Coroutine _refreshLabelCoroutine;
        private int _currentValue;

        public void Initialize(Color bgColor)
        {
            _labelBg = GetComponentInChildren<SpriteRenderer>();
            _label = GetComponentInChildren<TextMeshPro>();

            _labelBg.enabled = true;
            _label.enabled = true;

            _labelBg.color = bgColor;
        }

        public void SetLabelValue(int value)
        {
            if (_refreshLabelCoroutine != null)
                StopCoroutine(_refreshLabelCoroutine);

            _refreshLabelCoroutine = StartCoroutine(RefreshLabelValue(value));
        }

        private void SetValue(int value)
        {
            if (_currentValue > 0 && value <= 0)
            {
                _labelBg.enabled = false;
                _label.enabled = false;
            }

            _currentValue = value;
            _label.text = _currentValue.ToString();
        }

        //This coroutine is used to smooth the transition during a number change.
        private IEnumerator RefreshLabelValue(int value)
        {
            if (value < _currentValue)
            {
                SetValue(value);
                yield break;
            }

            var duration = 0.5f;
            var fixedDeltaTime = Time.fixedDeltaTime;
            var addTimes = duration / fixedDeltaTime;

            if (value * 0.1f <= duration)
            {
                SetValue(value);
            }
            else
            {
                for (var i = 1; i <= addTimes && _currentValue >= 0; i++)
                {
                    var v = (int) Mathf.Lerp(_currentValue, value, i / addTimes);
                    SetValue(v);
                    yield return new WaitForFixedUpdate();
                }
            }
            
            _currentValue = value;
            _refreshLabelCoroutine = null;
        }
    }
}