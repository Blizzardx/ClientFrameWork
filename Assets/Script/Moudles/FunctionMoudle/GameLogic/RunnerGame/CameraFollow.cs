using UnityEngine;
using System.Collections;

namespace RunnerGame
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float distance = -6f; //Distance target
        public float height = 6f; // Height camera
        public float angle = 30f;
        public float moveSpeed = 5f;
        public float rotationSpeed = 2f;

        private Vector3 posCamera;
        private Vector3 angleCam;

        // Use this for initialization
        void Start()
        {

        }

        public void SetTaget(Transform target)
        {
            this.target = target;
            transform.position = target.position + target.forward * distance + target.up * height;
            //Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
            Vector3 forward = new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position;
            forward = forward.normalized;
            forward = new Vector3(forward.x, forward.y, forward.z);
            forward = Quaternion.Euler(angle, 0, 0) * forward;
            Quaternion rot = Quaternion.LookRotation(forward);
            transform.rotation = rot;
        }

        void LateUpdate()
        {
            if (target != null)
            {
                Vector3 pos = target.position + target.forward * distance + target.up *height;
                transform.position = Vector3.Lerp(transform.position, pos, moveSpeed * Time.deltaTime);
                Vector3 forward = new Vector3(target.position.x,transform.position.y, target.position.z) - transform.position;
                forward = forward.normalized;
                forward = new Vector3(forward.x,forward.y,forward.z);
                forward =  Quaternion.Euler(angle, 0,0) * forward;
                Quaternion rot = Quaternion.LookRotation(forward);
                //Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
            }
        }
    }

}
