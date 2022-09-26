using SnakeVsBlocks.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeVsBlocks
{
    public class SnakeController : IUpdatable, IDisposable
    {
        private Camera _camera;
        private Snake _snake;
        private SnakeInput _input;

        public SnakeController(Camera camera, Snake snake)
        {
            _camera = camera;
            _snake = snake;
            _input = new SnakeInput();

            OnEnable();
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        public void Update(float deltaTime)
        {
            Vector2 mousePosition = _input.Snake.Look.ReadValue<Vector2>();

            mousePosition = _camera.ScreenToViewportPoint(mousePosition);
            mousePosition.y = 1;
            mousePosition = _camera.ViewportToWorldPoint(mousePosition);

            

            _snake.LookAt(mousePosition);
        }

        public void Dispose()
        {
            OnDisable();
        }
    }
}