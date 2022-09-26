using System;
using UnityEngine;
using SnakeVsBlocks.Presenters;
using SnakeVsBlocks.Model;

namespace SnakeVsBlocks
{
    public class Root : MonoBehaviour
    {
        public const float OffsetFinishPointY = 5f;

        [SerializeField] private Camera _camera;
        [SerializeField] private PresenterFactory _factory;
        [SerializeField] private SnakePresenter _snakePresenter;
        [SerializeField] private LevelGenerator _levelGenerator;

        public float MinX => _camera.ViewportToWorldPoint(new Vector2(0, 0)).x;
        public float MaxX => _camera.ViewportToWorldPoint(new Vector2(1, 0)).x;
        public Vector2 SnakeScale => Vector2.one * 0.6f;
        private float HalfSnakeWidth => SnakeScale.x * 0.5f;

        private float _finishPointY;
        private int _snakeLength = 30;

        private SnakeCollisionHandler _collisionHandler;
        private BlocksDestroyer _blocksDestroyer;

        private SnakeController _controller;

        private Snake _snake;

        public Action LevelEnded;

        private void Awake()
        {
            _snake = new Snake(Vector2.zero, 0, SnakeScale);
            _snakePresenter.Init(_snake);

            for (int i = 0; i < _snakeLength; i++)
            {
                _snake.IncreaseLength();
            }

            _controller = new SnakeController(_camera, _snake);

            _blocksDestroyer = new BlocksDestroyer(_camera, _levelGenerator);
            _collisionHandler = new SnakeCollisionHandler(_snake, _levelGenerator, MinX + HalfSnakeWidth, MaxX - HalfSnakeWidth);

            _finishPointY = _snake.Position.y + (_levelGenerator.LevelLength * _levelGenerator.OffsetFullLineY) + OffsetFinishPointY;
        }


        private void Update()
        {
            _controller.Update(Time.deltaTime);
            _blocksDestroyer.Update(Time.deltaTime);

            if(_snake.Position.y >= _finishPointY)
            {
                _snake.Pause();
                LevelEnded?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            _collisionHandler.FixedUpdate(Time.fixedDeltaTime);
        }
    }
}