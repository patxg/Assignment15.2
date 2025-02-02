using Components;
using Data;
using UnityEngine;

namespace Controllers
{
//This controller manages the level loading process.
    public class LevelLoader : MonoBehaviour
    {
        private Level _currentLevel;
        private GameData _gameData;

        private int _curLvl;
        private int _curLvlInd;

        private int CurLvlInd
        {
            get => _curLvlInd;
            set => _curLvlInd = value >= _gameData.LevelPrefabs.Length ? 0 : value;
        }

        public void Initialize(GameData gameData)
        {
            _gameData = gameData;
            _curLvl = PlayerPrefs.GetInt("CurrentLevel");
            CurLvlInd = PlayerPrefs.GetInt("CurrentLvlInd");

            GameEvents.OnGameStart += LoadLevel;
            GameEvents.OnLevelRewardCollected += LoadNextLevel;
            GameEvents.OnReplayBtn += LoadLevel;
        }

        public void DeInitialize()
        {
            GameEvents.OnGameStart -= LoadLevel;
            GameEvents.OnLevelRewardCollected -= LoadNextLevel;
            GameEvents.OnReplayBtn -= LoadLevel;
        }

        private void LoadNextLevel(int reward)
        {
            PlayerPrefs.SetInt("CurrentLevel", ++_curLvl);
            PlayerPrefs.SetInt("CurrentLvlInd", ++CurLvlInd);
            LoadLevel();
        }

        private void LoadLevel()
        {
            if (_currentLevel)
            {
                _currentLevel.DeInitialize();
                Destroy(_currentLevel.gameObject);
            }

            _currentLevel = Instantiate(_gameData.LevelPrefabs[CurLvlInd]);
            _currentLevel.Initialize(_curLvl + 1, _gameData);

            GameEvents.LevelLoad(_currentLevel);
        }
    }
}