using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    public class TransformSmoother : MonoBehaviour
    {
        public Vector3 currPos => transform.position;
        public Vector3 nextPos;
        private float positionTime;

        public Vector3 currRot => transform.eulerAngles;
        public Vector3 nextRot;

        public float interpolPeriod = .1f;
        private uint frame;
        private bool wait = true;
        void Update()
        {
            if (!(frame > 10))
            {
                frame++;
            }
            else
            {
                
                float t = 1.0f - ((positionTime - Time.time) / interpolPeriod);
                transform.position = Vector3.Lerp(currPos, nextPos, t);

                transform.rotation = Quaternion.Slerp(Quaternion.Euler(currRot), Quaternion.Euler(nextRot), t);

                positionTime = Time.time + interpolPeriod;
            }
        }
    }
}
