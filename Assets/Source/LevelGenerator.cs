using SnakeVsBlocks.Model;
using SnakeVsBlocks.Presenters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeVsBlocks
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private PresenterFactory _factory;
        [SerializeField] private Camera _camera;

        [Header("Settings")]
        [SerializeField] [Range(15, 100)] private int _levelLength;
        [SerializeField] [Range(1, 20)] private int _offsetFullLineY;
        [SerializeField] [Range(1, 10)] private int _numberHorizontalCells;
        [SerializeField] [Range(0f, 1f)] private float _distanceBetweenBlocks;
        [SerializeField] [Range(0, 100)] private int _blockSpawnPercent;
        [SerializeField] [Range(0, 100)] private int _bonusSpawnPercent;

        private Vector3 _lastCameraPosition;
        private float _startY;
        private int _numberSectors;
        private float _numberRandomLines = 2;
        private List<Block> _createdBlocks = new List<Block>();
        private List<Border> _createdBorders = new List<Border>();
        private List<Bonus> _createdBonuses = new List<Bonus>();

        public Action<IEnumerable<Transformable>> Generated;

        public int LevelLength => _levelLength;
        public int OffsetFullLineY => _offsetFullLineY;
        private float HalfBlockWidth => _blockSize.x * 0.5f;
        private float _offsetRandomY => _offsetFullLineY * 0.33f;
        private float _stepSpawnDistance => _offsetFullLineY * 0.8f;
        private float MinX => _camera.ViewportToWorldPoint(new Vector2(0, 0)).x;
        private float MaxX => _camera.ViewportToWorldPoint(new Vector2(1, 0)).x;
        private float _freeSpace => (MaxX - MinX) - (_distanceBetweenBlocks * (_numberHorizontalCells - 1));
        private float _cellSize => (_freeSpace / _numberHorizontalCells);
        private Vector2 _blockSize => Vector2.one * _cellSize;
        private float _offsetX => _cellSize + _distanceBetweenBlocks;
        private float _leftBound => MinX + (_blockSize.x * 0.5f);
        private Vector2 _borderSize => new Vector2(_distanceBetweenBlocks, _cellSize * 2 + 0.1f);

        private void Start()
        {
            _lastCameraPosition = _camera.transform.position;
            _startY = transform.position.y;
            GenerateSector(ref _startY);
        }

        private void Update()
        {
            if (_numberSectors + 1 >= _levelLength)
                return;

            if (MathF.Abs(_camera.transform.position.y - _lastCameraPosition.y) > _stepSpawnDistance)
            {
                GenerateSector(ref _startY);
                _lastCameraPosition = _camera.transform.position;

                _numberSectors++;
            }
        }

        private void GenerateSector(ref float startY)
        {
            List<Transformable> objects = new List<Transformable>();

            startY += _offsetFullLineY;
            float startYRandomLine = startY + _offsetRandomY;

            for (int a = 0; a < _numberRandomLines; a++)
            {
               Transformable[] newObjects = GenerateRandomLine(startYRandomLine, 
                    () => UnityEngine.Random.Range(0, 100) <= _blockSpawnPercent, 
                    () => UnityEngine.Random.Range(0, 100) <= _bonusSpawnPercent);
                startYRandomLine += _offsetRandomY;

                foreach (var obj in newObjects)
                {
                    objects.Add(obj);
                }
            }

            Transformable[] newObjects2 = GenerateFullLine(startY);

            foreach (var obj in newObjects2)
            {
                objects.Add(obj);
            }

            Generated?.Invoke(objects);
        }

        private Transformable[] GenerateRandomLine(float y, Func<bool> spawnChance, Func<bool> bonusSpawnCondition)
        {
            List<Transformable> objects = new List<Transformable>();

            for (int l = 0; l < _numberHorizontalCells; l++)
            {
                Vector2 blockPosition = new Vector2(_leftBound + (_offsetX * l), y);

                if (spawnChance.Invoke())
                {
                    objects.Add(GenerateBlock(blockPosition, UnityEngine.Random.Range(1, 20)));

                    Vector2 position = CorrectBorderPosition(y, blockPosition);
                    objects.Add(GenerateBorder(position));
                }
                else
                {
                    if (bonusSpawnCondition.Invoke())
                    {  
                        objects.Add(GenerateBonus(blockPosition, () => UnityEngine.Random.Range(0, 10)));
                    }
                }
            }

            return objects.ToArray();
        }

        private Transformable[] GenerateFullLine(float y)
        {
            List<Transformable> objects = new List<Transformable>();

            for (int l = 0; l < _numberHorizontalCells; l++)
            {
                Vector2 blockPosition = new Vector2(_leftBound + (_offsetX * l), y);
                objects.Add(GenerateBlock(blockPosition, UnityEngine.Random.Range(1, 20)));
                Vector2 position = CorrectBorderPosition(y, blockPosition);
                objects.Add(GenerateBorder(position));
            }

            return objects.ToArray();
        }

        private Vector2 CorrectBorderPosition(float y, Vector2 blockPosition)
        {
            float yPositionSecondBorder = y - HalfBlockWidth;
            Vector2 position = new Vector2(blockPosition.x + (_cellSize + _borderSize.x) * 0.5f, yPositionSecondBorder);
            return position;
        }

        private Border GenerateBorder(Vector2 position)
        {
            Border newBorder = new Border(position, 0, _borderSize);
            _createdBorders.Add(newBorder);

            _factory.CreateBorder(newBorder);

            return newBorder;
        }

        private Block GenerateBlock(Vector2 position, int health)
        {
            Block newBlock = new Block(position, 0, _blockSize, health);
            _createdBlocks.Add(newBlock);

            _factory.CreateBlock(newBlock);

            return newBlock;
        }

        private Bonus GenerateBonus(Vector2 position, Func<int> valueProvider)
        {
            Bonus newBonus = new Bonus(position, 0, Vector2.one * 0.5f, valueProvider.Invoke());
            _createdBonuses.Add(newBonus);

            _factory.CreateBonus(newBonus);
            
            return newBonus;
        }
    }
}
