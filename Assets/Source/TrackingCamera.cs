using UnityEngine;

namespace SnakeVsBlocks
{
    public class TrackingCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _offsetY;

        private Vector3 _targetPosition => new Vector3(transform.position.x, _target.position.y, transform.position.z) + Vector3.up * _offsetY;
        private float _previousSnakePositonY;

        private void Start()
        {
            _previousSnakePositonY = _target.position.y;
        }

        private void LateUpdate()
        {
            if (_target.position.y > _previousSnakePositonY)
            {
                transform.position = _targetPosition;
                _previousSnakePositonY = _target.position.y;
            }
        }
    }
}
