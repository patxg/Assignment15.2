using Components;
using Controllers;
using UnityEngine;

namespace Data
{
//This scriptable object stores game data in a specific file.
    [CreateAssetMenu(fileName = "Game Data", menuName = "Game Configurations/Game Data", order = 51)]
    public class GameData : ScriptableObject
    {
        [Header("Men Animations"), Space(5f)] [SerializeField]
        private AnimationClip _manAnimation_Idle;

        [SerializeField] private AnimationClip _manAnimation_Run;
        [SerializeField] private AnimationClip _manAnimation_Fight;
        [SerializeField] private AnimationClip _manAnimation_Win;

        [Header("Crowd Parameters"), Space(5f)] [SerializeField]
        private Man _playerManPrefab;

        [SerializeField] private Man _enemyManPrefab;
        [SerializeField] private MenCountLabel _menCountLabelPrefab;
        [Min(0.1f), SerializeField] private float _playerCrowdSpeed = 5f;
        [Min(0.1f), SerializeField] private float _manSpeed = 5f;
        [Min(0.1f), SerializeField] private float _manRotSpeed = 5f;

        [Header("Game Parameters"), Space(5f)] [Min(0), SerializeField]
        private int _killReward = 1;

        [SerializeField] private Level[] _levelPrefabs;
        [SerializeField] private Vector3 _cameraGameOffset = Vector3.back;
        [SerializeField] private Vector3 _cameraFightOffset = Vector3.back;
        [SerializeField] private float _cameraSpeed = 5f;

        [Header("Gates Parameters"), Space(5f)] [SerializeField]
        private Color _positiveColor;

        [SerializeField] private Color _negativeColor;

        [Header("Fight Parameters"), Space(5f)] [Min(0.01f), SerializeField]
        private float _fightDeathDist = 0.5f;

        [Min(0.01f), SerializeField] private float _metCrowdDist = 0.5f;
        [Range(0.1f, 2f), SerializeField] private float _manFightSpeedModifier = .5f;
        [Range(0f, 1f), SerializeField] private float _manHitMoment;

        [Header("VFX Parameters"), Space(5f)] [SerializeField]
        private ParticleSystem _killEffectPrefab;

        [Range(0f, 1f), SerializeField] private float _killFxColorRandomizer = 0.5f;
        [SerializeField] private float _fxLifeDuration;

        [Header("UI Parameters"), Space(5f)] 
        [SerializeField] private GameObject _uiPrefab;
        [SerializeField] private UIMessage _oneButtonMessage;

        [Range(0.1f, 5f), SerializeField] private float _showMessageDuration = 0.5f;

        public Man PlayerManPrefab => _playerManPrefab;
        public Man EnemyManPrefab => _enemyManPrefab;
        public MenCountLabel MenCountLabelPrefab => _menCountLabelPrefab;

        public float PlayerCrowdSpeed => _playerCrowdSpeed;
        public float ManSpeed => _manSpeed;
        public float ManRotSpeed => _manRotSpeed;
        public int KillReward => _killReward;
        public float FightDeathDist => _fightDeathDist;
        public float ManFightSpeedModifier => _manFightSpeedModifier;
        public float ManHitMoment => _manHitMoment;
        public float MetCrowdDist => _metCrowdDist;
        public ParticleSystem KillEffectPrefab => _killEffectPrefab;
        public float KillFxColorRandomizer => _killFxColorRandomizer;
        public float FxLifeDuration => _fxLifeDuration;

        public Level[] LevelPrefabs => _levelPrefabs;

        public Vector3 CameraGameOffset => _cameraGameOffset;
        public Vector3 CameraFightOffset => _cameraFightOffset;
        public float CameraSpeed => _cameraSpeed;

        public Color PositiveColor => _positiveColor;
        public Color NegativeColor => _negativeColor;

        public AnimationClip ManAnimation_Idle => _manAnimation_Idle;
        public AnimationClip ManAnimation_Run => _manAnimation_Run;
        public AnimationClip ManAnimation_Fight => _manAnimation_Fight;
        public AnimationClip ManAnimation_Win => _manAnimation_Win;

        public GameObject UIPrefab => _uiPrefab;
        public UIMessage OneButtonMessage => _oneButtonMessage;
        public float ShowMessageDuration => _showMessageDuration;
    }
}