using Data;
using Helpers;
using UnityEngine;

namespace Components
{
//This component stores specific level data, initializes it's units and checks if the level has been completed.
    public class Level : MonoBehaviour
    {
        private Boss _boss;
        private Finish _finish;

        private Crowd _playerCrowd;
        private Crowd[] _enemyCrowds;
        private Gates[] _gates;

        public int LvlNum { get; private set; }
        public Boss Boss => _boss;
        public Crowd PlayerCrowd => _playerCrowd;
        public Crowd[] EnemyCrowds => _enemyCrowds;

        private int _gateInd;
        private int _lvlReward;

        public void Initialize(int lvlNum, GameData gameData)
        {
            _boss = GetComponentInChildren<Boss>();
            _finish = GetComponentInChildren<Finish>();
            _enemyCrowds = GetComponentsInChildren<Crowd>();
            _gates = GetComponentsInChildren<Gates>();
            _playerCrowd = new GameObject("PlayerCrowd").AddComponent<Crowd>();
            _playerCrowd.transform.SetParent(transform);

            LvlNum = lvlNum;
            var animations = new[]
            {
                gameData.ManAnimation_Idle,
                gameData.ManAnimation_Run,
                gameData.ManAnimation_Fight,
                gameData.ManAnimation_Win,
            };

            var manHitDelay = gameData.ManHitMoment * gameData.ManAnimation_Fight.length;

            if (_boss) Boss.Initialize();
            PlayerCrowd.Initialize(gameData.PlayerManPrefab, manHitDelay, animations, gameData.MenCountLabelPrefab);

            foreach (var crowd in EnemyCrowds)
            {
                crowd.Initialize(gameData.EnemyManPrefab, manHitDelay, animations);
                _lvlReward += crowd.MenCount * gameData.KillReward;
            }

            foreach (var gate in _gates)
            {
                gate.Initialize(gameData.PositiveColor, gameData.NegativeColor);
            }

            GameEvents.OnBossFightEnd += EndLevel;
            GameEvents.OnGameUpdate += OnUpdate;
        }

        public void DeInitialize()
        {
            GameEvents.OnGameUpdate -= OnUpdate;
            GameEvents.OnBossFightEnd -= EndLevel;

            _lvlReward = 0;
            PlayerCrowd.DeInitialize();
            foreach (var crowd in EnemyCrowds)
            {
                crowd.DeInitialize();
            }
        }

        //Adjusts the player's unit count when the gates are achieved.
        private void CheckGatesAchieved()
        {
            if (_gateInd >= _gates.Length)
                return;

            var manPos = _playerCrowd.ForwardManWorldPos;
            var areGatesAchieved = manPos.z >= _gates[_gateInd].transform.position.z;
            if (areGatesAchieved)
            {
                var value = _gates[_gateInd++].Catalyse(manPos);
                _playerCrowd.ChangeMenCount(value);
            }
        }

        private void CheckFinishAchieved()
        {
            if (!_finish)
                return;

            var isFinishAchieved = _playerCrowd.WorldPos.z > _finish.transform.position.z;
            if (isFinishAchieved)
            {
                _playerCrowd.SetAnimation(AnimationType.Win);
                GameEvents.FinishAchieved();
                EndLevel(true);
            }
        }

        private void EndLevel(bool isPlayerWin)
        {
            if (isPlayerWin)
            {
                GameEvents.LevelEnd(_lvlReward);
            }

            DeInitialize();
        }

        //This method is equivalent to the Update() method in MonoBehaviour.
        //All OnUpdate() methods are invoked through the OnUpdate action by the GameManager.
        private void OnUpdate(float deltaTime)
        {
            CheckFinishAchieved();
            CheckGatesAchieved();
        }
        
        //Draws a primitive at each enemy man's position and player crowd position to display it in Editor Mode.
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            var positions = ValuesCounter.HexagonPositions;
            var unitSize = 0.5f;
            var unitScale = Vector3.one * unitSize;
            var heightOffset = Vector3.up * unitSize * 0.5f;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(Vector3.zero + heightOffset, unitScale);

            Gizmos.color = Color.red;
            foreach (var enemyCrowd in GetComponentsInChildren<Crowd>())
            {
                for (var i = 0; i < enemyCrowd.MenStartCount; i++)
                {
                    Gizmos.DrawCube(enemyCrowd.transform.position + positions[i] + heightOffset, unitScale);
                }
            }
            
        }
    }
}