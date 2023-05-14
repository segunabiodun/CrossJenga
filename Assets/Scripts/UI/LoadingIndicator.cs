using UnityEngine;

namespace UI
{
    public class LoadingIndicator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 10f;

        void Update()
        {
        
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}
