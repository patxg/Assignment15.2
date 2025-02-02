using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
//A component for a simple UI message for it's initializing.
    public class UIMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private TextMeshProUGUI _buttonMessage;
        [SerializeField] private Button _button;

        public void Initialize(string messageText, string buttonText, Action tapAction)
        {
            _buttonMessage.text = buttonText;
            _message.text = messageText;
            _button.onClick.AddListener(() =>
            {
                _button.onClick.RemoveAllListeners();
                tapAction?.Invoke();
            });
        }
    }
}