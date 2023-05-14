using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class CameraController: MonoBehaviour
    {
        [SerializeField] private float cameraLookSpeed;
        [SerializeField] private float rotationDuration;
        [SerializeField] private float moveDuration;
        [SerializeField] private Transform target;
        [SerializeField] private UIHandler uiHandler;

        private bool _isAnimatingToTarget;
        private float _targetChangeTime;
        private Quaternion _startRotation;
        private Quaternion _endRotation;
        private Vector3 _startCamPosition;
        private Vector3 _endCamPosition;
        
        private void Start()
        {
            uiHandler.CameraFocusTargetChanged += OnCameraFocusTargetChanged;
            OnCameraFocusTargetChanged(target);
        }

        private void Update()
        {
            MoveAndRotateToTarget();
            HandleInput();
        }

        private void MoveAndRotateToTarget()
        {
            if (!_isAnimatingToTarget)
            {
                return;
            }
            
            var moveCompletion = (Time.time - _targetChangeTime) / moveDuration;
            var newPosition = Vector3.Lerp(_startCamPosition, _endCamPosition, moveCompletion);
            transform.position = newPosition;

            var rotationCompletion = (Time.time - _targetChangeTime) / rotationDuration;
            transform.rotation = Quaternion.Slerp(_startRotation, _endRotation, rotationCompletion);

            _isAnimatingToTarget = moveCompletion < 1 || rotationCompletion < 1;
        }

        private void HandleInput()
        {
            if(Input.GetMouseButton(1) && !_isAnimatingToTarget)
            {
                transform.RotateAround(target.transform.position,
                    transform.up,
                    Input.GetAxis("Mouse X") * cameraLookSpeed * Time.deltaTime);
            }
        }

        private void OnCameraFocusTargetChanged(Transform newTarget)
        {
            _isAnimatingToTarget = true;
            target = newTarget;
            _startCamPosition = transform.position;
            _endCamPosition = target.position +
                                 (transform.position - target.position).normalized * 3;
            
            _targetChangeTime = Time.time;
            _startRotation = transform.rotation;
            _endRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        }
    }
}