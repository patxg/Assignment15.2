using Components;
using Data;
using UnityEngine;

namespace Controllers
{
//A core controller that handles basic game events such as GameStart and InitializeManagers.
    public class GameCore : MonoBehaviour
    {
        [SerializeField] private GameData _gameData;

        private InputManager _inputManager;
        private CameraController _camCtrl;
        private LevelLoader _levelLoader;
        private UIMessageController _uiMessageController;
        private PlayerCrowdMover _mover;
        private FightController _fightController;
        private FxSpawner _fxSpawner;

        private bool _allComponentsInitialized;

        public GameData GameData => _gameData;

        private void Awake()
        {
            var uiObj = Instantiate(_gameData.UIPrefab, transform);
            _uiMessageController = uiObj.GetComponentInChildren<UIMessageController>();
            _inputManager = gameObject.AddComponent<InputManager>();
            _camCtrl = gameObject.AddComponent<CameraController>();
            _levelLoader = gameObject.AddComponent<LevelLoader>();
            _mover = gameObject.AddComponent<PlayerCrowdMover>();
            _fightController = gameObject.AddComponent<FightController>();
            _fxSpawner = gameObject.AddComponent<FxSpawner>();
        }

        private void Start()
        {
            InitializeManagers();
            StartGame();
        }

        private void StartGame()
        {
            GameEvents.GameStart();
        }

        private void InitializeManagers()
        {
            _inputManager.Initialize();
            _uiMessageController.Initialize(GameData);
            _fxSpawner.Initialize(GameData.KillEffectPrefab.gameObject, GameData.FxLifeDuration,
                GameData.KillFxColorRandomizer);
            _mover.Initialize(GameData.PlayerCrowdSpeed, GameData.ManSpeed, GameData.ManRotSpeed);
            _fightController.Initialize(_gameData);
            _levelLoader.Initialize(GameData);
            _camCtrl.Initialize(_gameData.CameraGameOffset, _gameData.CameraFightOffset, _gameData.CameraSpeed);

            _allComponentsInitialized = true;
        }

        private void DeInitializeManagers()
        {
            _inputManager.DeInitialize();
            _uiMessageController.DeInitialize();
            _levelLoader.DeInitialize();
            _camCtrl.DeInitialize();
            _fightController.DeInitialize();
            _fxSpawner.DeInitialize();
            _mover.DeInitialize();
            _allComponentsInitialized = false;
        }

        //This is a standard MonoBehaviour method that refreshes all OnUpdate() methods subscribed to the OnGameUpdate action.
        private void Update()
        {
            if (!_allComponentsInitialized)
                return;

            GameEvents.GameUpdate(Time.deltaTime);
        }

        private void OnDisable()
        {
            DeInitializeManagers();
        }
    }
}