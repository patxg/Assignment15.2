using Data;
using UnityEngine;

namespace Controllers
{
//This manager processes the player's input and triggers the appropriate actions, which then initiate various game events.
    public class InputManager : MonoBehaviour
    {
        private GameCore _gameCore;

        private Vector2 _lastFingerPos;

        public void Initialize()
        {
            GameEvents.OnGameUpdate += OnUpdate;
        }

        public void DeInitialize()
        {
            GameEvents.OnGameUpdate -= OnUpdate;
        }

        private void OnDrag(Vector2 dragDirection)
        {
            GameEvents.Drag(dragDirection);
        }

        //This method is equivalent to the Update() method in MonoBehaviour.
        //All OnUpdate() methods are invoked through the OnUpdate action by the GameManager.
        private void OnUpdate(float deltaTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _lastFingerPos = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 fingerPos = Input.mousePosition;
                var vectorDirection = fingerPos - _lastFingerPos;
                if (vectorDirection.x != 0f)
                {
                    OnDrag(vectorDirection);
                    _lastFingerPos = fingerPos;
                }
            }
        }

        private void OnDisable()
        {
            DeInitialize();
        }
    }
}