using System;
using Components;
using UnityEngine;

namespace Data
{
//A static class which is used to store and call the game actions.
    public static class GameEvents
    {
        //Game
        public static event Action<float> OnGameUpdate;
        public static event Action OnGameStart;
        public static event Action OnGameLoose;

        //Level
        public static event Action<Level> OnLevelLoad;
        public static event Action OnLevelStartClick;
        public static event Action OnFinishAchieved;
        public static event Action<int> OnLevelEnd;
        public static event Action<int> OnLevelRewardCollected;

        //Input
        public static event Action OnReplayBtn;
        public static event Action<Vector2> OnDrag;

        //Fight

        public static event Action OnCrowdFightStart;
        public static event Action OnCrowdFightEnd;
        public static event Action OnBossFightStart;
        public static event Action<bool> OnBossFightEnd;
        public static event Action<Vector3, Color> OnManDead;


        public static void GameUpdate(float deltaTime) => OnGameUpdate?.Invoke(deltaTime);
        public static void GameStart() => OnGameStart?.Invoke();
        public static void GameLoose() => OnGameLoose?.Invoke();

        public static void LevelLoad(Level level) => OnLevelLoad?.Invoke(level);
        public static void LevelStartClick() => OnLevelStartClick?.Invoke();
        public static void FinishAchieved() => OnFinishAchieved?.Invoke();
        public static void LevelEnd(int reward) => OnLevelEnd?.Invoke(reward);
        public static void CollectLevelReward(int value) => OnLevelRewardCollected?.Invoke(value);

        public static void ReplayBtn() => OnReplayBtn?.Invoke();
        public static void Drag(Vector2 dragDirection) => OnDrag?.Invoke(dragDirection);

        public static void CrowdFightStart() => OnCrowdFightStart?.Invoke();
        public static void FightEnd() => OnCrowdFightEnd?.Invoke();
        public static void BossFightStart() => OnBossFightStart?.Invoke();
        public static void BossFightEnd(bool isPlayerWin) => OnBossFightEnd?.Invoke(isPlayerWin);
        public static void ManDead(Vector3 killPos, Color bloodColor) => OnManDead?.Invoke(killPos, bloodColor);
    }
}