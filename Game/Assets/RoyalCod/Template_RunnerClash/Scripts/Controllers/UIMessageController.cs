using System;
using System.Collections;
using Components;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
//This component is used for handling game messages.
    [RequireComponent(typeof(CanvasGroup))]
    public class UIMessageController : MonoBehaviour
    {
        private GameData _gameData;
        private UIMessage _oneButtonMessage;
        private readonly Vector2 _messageSpawnPos = new(0, -Screen.height * 2.5f);
        private Image _curtains;

        private bool _isTutorialShown;
        private bool _isMessageActive;

        public void Initialize(GameData gameData)
        {
            if (!_curtains)
            {
                _curtains = CreateCurtains();
            }

            _gameData = gameData;
            _oneButtonMessage = Instantiate(_gameData.OneButtonMessage, transform);
            _oneButtonMessage.gameObject.SetActive(false);
            _isTutorialShown = PlayerPrefs.GetInt("TutorialIsShown") == 1;

            GameEvents.OnLevelLoad += ShowLevelStartMessage;
            GameEvents.OnLevelEnd += ShowCollectLevelRewardMessage;
            GameEvents.OnGameLoose += ShowReplayMessage;
        }

        public void DeInitialize()
        {
            GameEvents.OnLevelLoad -= ShowLevelStartMessage;
            GameEvents.OnLevelEnd -= ShowCollectLevelRewardMessage;
            GameEvents.OnGameLoose -= ShowReplayMessage;
        }

        private void ShowLevelStartMessage(Level lvl)
        {
            if (!_isTutorialShown)
            {
                ShowTutorialMessage(lvl);
                return;
            }

            ShowOneButtonMessage($"<#FFF700>LEVEL {lvl.LvlNum}</color>\n\nTap the button to start!",
                "START",
                GameEvents.LevelStartClick);
        }

        private void ShowTutorialMessage(Level lvl)
        {
            ShowOneButtonMessage($"Swipe <#FFF700>LEFT</color> and <#FFF700>RIGHT</color> to move",
                "OK", () =>
                {
                    _isTutorialShown = true;
                    PlayerPrefs.SetInt("TutorialIsShown", 1);
                    ShowLevelStartMessage(lvl);
                });
        }

        private void ShowCollectLevelRewardMessage(int reward)
        {
            ShowOneButtonMessage($"GET <#F400FF>{reward} GEMS</color>\nand go to the next level!",
                "NEXT LEVEL",
                () => GameEvents.CollectLevelReward(reward));
        }

        private void ShowReplayMessage()
        {
            ShowOneButtonMessage($"<#F400FF>OOPS!</color>\nTry again!",
                "REPLAY", GameEvents.ReplayBtn);
        }

        //The Curtains is a semi-transparent image used as a background for messages.
        private Image CreateCurtains()
        {
            var curtains = new GameObject("Curtains").AddComponent<Image>();
            var curtainsTr = curtains.transform;

            curtainsTr.SetParent(transform);
            curtainsTr.localPosition = Vector3.zero;
            curtains.color = Color.black * 0.9f;
            curtains.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            return curtains;
        }

        private void ShowOneButtonMessage(string messageText, string buttonText, Action tapAction)
        {
            if (_isMessageActive)
                return;

            StartCoroutine(ShowMessageCoroutine());

            IEnumerator ShowMessageCoroutine()
            {
                _isMessageActive = true;
                _oneButtonMessage.transform.localPosition = _messageSpawnPos;
                _oneButtonMessage.Initialize(messageText, buttonText,
                    () =>
                    {
                        _oneButtonMessage.gameObject.SetActive(false);
                        _curtains.gameObject.SetActive(false);
                        _isMessageActive = false;
                        tapAction?.Invoke();
                    });

                _oneButtonMessage.gameObject.SetActive(true);
                _curtains.gameObject.SetActive(true);
                var awaitStep = Time.fixedDeltaTime;
                var liftStepsCount = _gameData.ShowMessageDuration / awaitStep;
                var liftStep = Math.Abs(_messageSpawnPos.y) / liftStepsCount;
                for (var i = 0; i < liftStepsCount; i++)
                {
                    _oneButtonMessage.transform.localPosition += Vector3.up * liftStep;
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
}