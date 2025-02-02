using System;
using Data;
using Helpers;
using UnityEngine;

namespace Components
{
//This component moves the Player Crowd according to the InputManager signals.
    public class PlayerCrowdMover : MonoBehaviour
    {
        private Crowd _playerCrowd;
        private Vector3 _leashPos;

        private float _crowdSpeed;
        private float _manSpeed;
        private float _manRotSpeed;
        private float _roadHalfWidth;

        private bool _isStopped = true;

        public void Initialize(float crowdSpeed, float manSpeed, float manRotSpeed)
        {
            _roadHalfWidth = ValuesCounter.ROAD_WIDTH * 0.5f;
            _crowdSpeed = crowdSpeed;
            _manSpeed = manSpeed;
            _manRotSpeed = manRotSpeed;

            GameEvents.OnDrag += MoveLeashX;
            GameEvents.OnLevelLoad += GetLevelData;
            GameEvents.OnLevelStartClick += RunTheCrowd;
            GameEvents.OnCrowdFightStart += SetCrowdFightMode;
            GameEvents.OnBossFightStart += SetCrowdFightMode;
            GameEvents.OnCrowdFightEnd += RunTheCrowd;
            GameEvents.OnFinishAchieved += StopTheCrowd;
            GameEvents.OnGameLoose += ReInit;
            GameEvents.OnGameUpdate += OnUpdate;
        }

        //The ReInit method is used to reload some fields on GameLoose, FinishAchieved and some other actions
        private void ReInit()
        {
            StopTheCrowd();
            _playerCrowd = null;
            _leashPos = Vector3.zero;
        }

        public void DeInitialize()
        {
            StopTheCrowd();
            GameEvents.OnGameUpdate -= OnUpdate;
            GameEvents.OnDrag -= MoveLeashX;
            GameEvents.OnLevelLoad -= GetLevelData;
            GameEvents.OnLevelStartClick -= RunTheCrowd;
            GameEvents.OnCrowdFightStart -= SetCrowdFightMode;
            GameEvents.OnBossFightStart -= SetCrowdFightMode;
            GameEvents.OnCrowdFightEnd -= RunTheCrowd;
            GameEvents.OnGameLoose -= ReInit;
            GameEvents.OnFinishAchieved -= StopTheCrowd;
        }

        //This method fills some fields with a certain level data.
        private void GetLevelData(Level level)
        {
            _playerCrowd = level.PlayerCrowd;
            _leashPos = _playerCrowd.WorldPos;
        }

        //The MoveLeashX method moves the Leash to the left and to the right.
        private void MoveLeashX(Vector2 dragDirection)
        {
            var step = ValuesCounter.ROAD_WIDTH * (dragDirection.x / Screen.width);
            _leashPos.x += step;
            _leashPos.x = Math.Clamp(_leashPos.x, -_roadHalfWidth, _roadHalfWidth);
        }

        //The MoveLeashX method moves the Leash forward.
        private void MoveLeashZ(float deltaSpeed)
        {
            _leashPos.z += deltaSpeed;
        }

        //This method moves the Leash, an abstract construct used to calculate target positions for members of the player's crowd.
        private void MoveMen(float deltaSpeed)
        {
            var men = _playerCrowd.Men;
            foreach (var man in men)
            {
                var manTr = man.transform;
                var manPos = manTr.position;
                var manRot = manTr.rotation;
                var targetPos = _leashPos + man.TargetLocalPos;
                var targetRot = Quaternion.LookRotation((targetPos - manPos).normalized);

                manTr.position = Vector3.Lerp(manPos, targetPos, deltaSpeed);
                manTr.rotation = Quaternion.Lerp(manRot, targetRot, deltaSpeed * _manRotSpeed);
            }
        }

        private void StopTheCrowd()
        {
            _isStopped = true;
        }

        private void RunTheCrowd()
        {
            _isStopped = false;
            _playerCrowd.SetAnimation(AnimationType.Run);
        }

        private void SetCrowdFightMode()
        {
            StopTheCrowd();
            _playerCrowd.SetAnimation(AnimationType.Fight);
            _isStopped = true;
        }

        //This method moves the Label which shows a current number of men in the player's crowd.
        private void UpdateLabelPositioning(float deltaSpeed)
        {
            var labelTr = _playerCrowd.MenCountLabel.transform;
            var curPos = labelTr.position;
            var targetPos = _leashPos;
            targetPos.y = curPos.y;
            labelTr.position = Vector3.Lerp(curPos, targetPos, deltaSpeed);
        }

        //This method is equivalent to the Update() method in MonoBehaviour.
        //All OnUpdate() methods are invoked through the OnUpdate action by the GameManager.
        private void OnUpdate(float deltaTime)
        {
            if (_isStopped || !_playerCrowd)
            {
                return;
            }

            var deltaCrowdSpeed = _crowdSpeed * deltaTime;
            var deltaManSpeed = _manSpeed * deltaTime;

            MoveLeashZ(deltaCrowdSpeed);
            MoveMen(deltaManSpeed);
            UpdateLabelPositioning(deltaCrowdSpeed);
        }
    }
}