using Components;
using Data;
using UnityEngine;

namespace Controllers
{
//This component controls the game camera making it follow the player's crowd on a certain distance.
    public class CameraController : MonoBehaviour
    {
        private Crowd _playerCrowd;
        private Camera _camera;
        private Vector3 _targetPos;
        private Vector3 _targetOffset;
        private Vector3 _gameOffset;
        private Vector3 _fightOffset;
        private float _followSpeed;
        private bool _isFightMode;

        public void Initialize(Vector3 gameOffset, Vector3 fightOffset, float followSpeed)
        {
            _gameOffset = gameOffset;
            _fightOffset = fightOffset;
            _targetOffset = gameOffset;
            _followSpeed = followSpeed;
            _camera = Camera.main;

            GameEvents.OnLevelLoad += SetCamOnLevelLoad;
            GameEvents.OnCrowdFightStart += SetCamInCrowdFightMode;
            GameEvents.OnBossFightStart += SetCamInCrowdFightMode;
            GameEvents.OnCrowdFightEnd += SetCamInGameMode;
        }

        public void DeInitialize()
        {
            GameEvents.OnLevelLoad -= SetCamOnLevelLoad;
            GameEvents.OnCrowdFightStart -= SetCamInCrowdFightMode;
            GameEvents.OnBossFightStart -= SetCamInCrowdFightMode;
            GameEvents.OnCrowdFightEnd -= SetCamInGameMode;
        }

        private void SetCamOnLevelLoad(Level level)
        {
            _playerCrowd = level.PlayerCrowd;
            SetCamInGameMode();
        }

        private void SetCamInGameMode()
        {
            _isFightMode = false;
            _targetOffset = _gameOffset;
        }

        private void SetCamInCrowdFightMode()
        {
            _isFightMode = true;
            _targetOffset = _fightOffset;
            _targetPos.x = 0;
        }

        private void LateUpdate()
        {
            if (_playerCrowd && _playerCrowd.MenCount > 0)
            {
                if (!_isFightMode) _targetPos = _playerCrowd.WorldPos;
                var myPos = _camera.transform.position;
                var lookAtPos = _targetPos + _targetOffset;
                var deltaSpeed = Time.deltaTime * _followSpeed;
                _camera.transform.position = Vector3.Lerp(myPos, lookAtPos, deltaSpeed);
            }
        }
    }
}