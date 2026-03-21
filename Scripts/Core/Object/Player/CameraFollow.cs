using UnityEngine;

namespace GameJam_URA
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new Vector3(0f, 1f, -10f);

        void LateUpdate()
        {
            if (!target) return;
            transform.position = target.position + offset;
        }
    }
}
