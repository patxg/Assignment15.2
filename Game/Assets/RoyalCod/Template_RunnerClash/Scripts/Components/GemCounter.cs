using Data;
using TMPro;
using UnityEngine;

namespace Components
{
//A simple component used for displaying the gem count label.
    public class GemCounter : MonoBehaviour
    {
        private TextMeshProUGUI _gemAmountLabel;
        private int _currentGemAmount;

        private int CurrentGemAmount
        {
            get => _currentGemAmount;
            set => _currentGemAmount = value < 0 ? 0 : value;
        }

        public void OnEnable()
        {
            CurrentGemAmount = PlayerPrefs.GetInt("GemsAmount");
            _gemAmountLabel = GetComponentInChildren<TextMeshProUGUI>();
            _gemAmountLabel.text = CurrentGemAmount.ToString();

            GameEvents.OnLevelRewardCollected += ChangeGemAmount;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelRewardCollected -= ChangeGemAmount;
        }

        private void ChangeGemAmount(int value)
        {
            CurrentGemAmount += value;
            _gemAmountLabel.text = CurrentGemAmount.ToString();
            PlayerPrefs.SetInt("GemsAmount", CurrentGemAmount);
        }
    }
}