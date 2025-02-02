using System.Linq;
using Components;
using Data;
using Helpers;
using UnityEngine;

namespace Controllers
{
//This is one of the most important and complex controllers in the game.
//It manages all combat processes involving crowds and bosses.
    public class FightController : MonoBehaviour
    {
        private Crowd _playerCrowd;
        private Crowd[] _enemyCrowds;
        private Boss _boss;

        private Vector3 _fightPosition;

        private float _crowdMeetDistance;
        private float _killDistance;
        private float _manFightSpeed;
        private float _manRotSpeed;

        private int _enemyCrowdInd;

        private bool _isReadyToCrowdFight;
        private bool _isReadyToBossFight;
        private bool _isLevelFinished;

        public void Initialize(GameData gameData)
        {
            _crowdMeetDistance = gameData.MetCrowdDist;
            _manFightSpeed = gameData.ManFightSpeedModifier * gameData.ManSpeed;
            _killDistance = gameData.FightDeathDist;
            _manRotSpeed = gameData.ManRotSpeed;

            GameEvents.OnLevelLoad += ReadLevel;
            GameEvents.OnGameUpdate += OnUpdate;
        }

        public void DeInitialize()
        {
            GameEvents.OnLevelLoad -= ReadLevel;
            GameEvents.OnGameUpdate -= OnUpdate;
        }

        private void ReadLevel(Level level)
        {
            _playerCrowd = level.PlayerCrowd;
            _enemyCrowds = level.EnemyCrowds.OrderBy(x => x.WorldPos.z).ToArray();
            _boss = level.Boss;
            _enemyCrowdInd = 0;
            _isReadyToBossFight = false;
            _isLevelFinished = false;
        }

        private void TurnToCrowdFightMode()
        {
            _fightPosition = (_enemyCrowds[_enemyCrowdInd].WorldPos + _playerCrowd.WorldPos) * 0.5f;
            _playerCrowd.SetAnimation(AnimationType.Fight);
            _enemyCrowds[_enemyCrowdInd].SetAnimation(AnimationType.Fight);
            GameEvents.CrowdFightStart();
            _isReadyToCrowdFight = true;
        }

        private void TurnToBossFightMode()
        {
            _fightPosition = _boss.transform.position + Vector3.back * _boss.FightDistance;
            _playerCrowd.RefreshMenPositioning(ValuesCounter.BossFightPositions);
            _boss.PlayAnim(AnimationType.Fight);
            _playerCrowd.SetAnimation(AnimationType.Fight);
            GameEvents.BossFightStart();
            _isReadyToBossFight = true;
        }

        //This method manages combat between units, inflicting damage and killing them within a certain range.
        private void FightBossOnDeathDistance(float deltaTime)
        {
            foreach (var man in _playerCrowd.Men)
            {
                var isManOnPos = ((man.TargetLocalPos + _fightPosition) - man.transform.position).sqrMagnitude < 0.25f;
                if (isManOnPos && man.TryHit(deltaTime))
                {
                    var bossHp = _boss.GetDamage();
                    if (bossHp <= 0)
                    {
                        _isLevelFinished = true;
                        GameEvents.BossFightEnd(true);
                        return;
                    }
                }
            }

            if (_boss.TryHit(deltaTime))
            {
                _playerCrowd.KillSomeMen(_boss.Damage);
                _playerCrowd.RefreshMenPositioning(ValuesCounter.BossFightPositions);

                if (_playerCrowd.MenCount <= 0)
                {
                    _isLevelFinished = true;
                    GameEvents.BossFightEnd(false);
                    GameEvents.GameLoose();
                }
            }
        }

        //This method manages combat between units, killing them within a certain range.
        private void KillOnDeathDistance(Man playerMan)
        {
            var playerManPos = playerMan.transform.position;
            foreach (var enemyMan in _enemyCrowds[_enemyCrowdInd].Men)
            {
                if (enemyMan.IsInjured) continue;
                var enemyManPos = enemyMan.transform.position;
                var sqrDist = (enemyManPos - playerManPos).sqrMagnitude;
                if (sqrDist < _killDistance)
                {
                    _playerCrowd.InjureMan(playerMan);
                    _enemyCrowds[_enemyCrowdInd].InjureMan(enemyMan);
                    return;
                }
            }
        }

        //This method manages combat between units, inflicting damage and killing them within a certain range.
        private void KillOnDeathDistances()
        {
            var playerMen = _playerCrowd.Men;
            foreach (var playerMan in playerMen)
            {
                if (playerMan.IsInjured) continue;
                KillOnDeathDistance(playerMan);
            }

            _playerCrowd.KillInjuredMen();
            _enemyCrowds[_enemyCrowdInd].KillInjuredMen();

            if (_playerCrowd.MenCount <= 0)
            {
                _isLevelFinished = true;
                _isReadyToCrowdFight = false;
                _enemyCrowds[_enemyCrowdInd].SetAnimation(AnimationType.Win);
                GameEvents.FightEnd();
                GameEvents.GameLoose();
            }
            else if (_enemyCrowds[_enemyCrowdInd].MenCount <= 0)
            {
                _isReadyToCrowdFight = false;
                GameEvents.FightEnd();
                _enemyCrowdInd++;
            }
        }

        //This method moves player's units closer to the enemies.
        private void MoveMenToFight(Crowd crowd, float deltaSpeed)
        {
            foreach (var man in crowd.Men)
            {
                var manTr = man.transform;
                var manPos = manTr.position;
                var manRot = manTr.rotation;
                var targetPos = _fightPosition + man.TargetLocalPos;
                var targetRot = Quaternion.LookRotation((_fightPosition - manPos).normalized);

                manTr.position = Vector3.Lerp(manPos, targetPos, deltaSpeed);
                if ((man.TargetLocalPos - manTr.localPosition).sqrMagnitude > 0.5f)
                    manTr.rotation = Quaternion.Lerp(manRot, targetRot, deltaSpeed * _manRotSpeed);
            }
        }

        //This method enables the player's crowd to interact with a boss by moving units closer to him.
        private void UpdateBossInteraction(float deltaTime, float meetPosZ)
        {
            if (!_boss)
                return;

            if (_isReadyToBossFight)
            {
                var deltaManSpeed = _manFightSpeed * deltaTime;
                MoveMenToFight(_playerCrowd, deltaManSpeed);
                FightBossOnDeathDistance(deltaTime);
                return;
            }

            var bossPosZ = _boss.transform.position.z;
            if (meetPosZ > bossPosZ)
            {
                TurnToBossFightMode();
            }
        }

        //This method enables the player's crowd to interact with the enemy's crowd by moving units closer together.
        private void UpdateEnemyCrowdInteraction(float deltaTime, float meetPosZ)
        {
            if (_isReadyToCrowdFight)
            {
                var deltaManSpeed = _manFightSpeed * deltaTime;
                MoveMenToFight(_playerCrowd, deltaManSpeed);
                MoveMenToFight(_enemyCrowds[_enemyCrowdInd], deltaManSpeed);
                KillOnDeathDistances();
                return;
            }

            var enemyCrowd = _enemyCrowds[_enemyCrowdInd];
            var enemyForwardPosZ = enemyCrowd.WorldPos.z + enemyCrowd.ForwardManLocalPos.z;
            if (meetPosZ > enemyForwardPosZ)
            {
                TurnToCrowdFightMode();
            }
        }

        //This method is equivalent to the Update() method in MonoBehaviour.
        //All OnUpdate() methods are invoked through the OnUpdate action by the GameManager.
        private void OnUpdate(float deltaTime)
        {
            if (_isLevelFinished || !_playerCrowd)
                return;

            var playerForwardPosZ = _playerCrowd.WorldPos.z + _playerCrowd.ForwardManLocalPos.z;
            var meetPosZ = playerForwardPosZ + _crowdMeetDistance;

            if (_enemyCrowdInd >= _enemyCrowds.Length) UpdateBossInteraction(deltaTime, meetPosZ);
            else UpdateEnemyCrowdInteraction(deltaTime, meetPosZ);

        }
    }
}