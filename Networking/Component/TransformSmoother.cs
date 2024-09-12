﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    [DisallowMultipleComponent]
    public class TransformSmoother : MonoBehaviour
    {
        /// <summary>
        /// Next rotation. The future rotation, this is the rotation the transform is smoothing to.
        /// </summary>
        public Vector3 nextRot;

        /// <summary>
        /// Next position. The future position, this is the position the transform is smoothing to.
        /// </summary>
        public Vector3 nextPos;

        /// <summary>
        ///  Interpolation Period. the speed at which the transform is smoothed.
        /// </summary>
        public float interpolPeriod = .1f;

        public Vector3 currPos => transform.position;
        private float positionTime;

        public Vector3 currRot => transform.eulerAngles;

        private uint frame;
        private bool wait = true;
        public void Update()
        {
            if (GetComponent<NetworkActor>() != null)
            {
                if (GetComponent<NetworkActor>().IsOwned)
                {
                    enabled = false;
                    return;
                }
            }
            if (!(frame > 10))
            {
                frame++;
            }
            else
            {
                
                float t = 1.0f - ((positionTime - Time.time) / interpolPeriod);
                transform.position = Vector3.Lerp(currPos, nextPos, t);

                transform.rotation = Quaternion.Lerp(Quaternion.Euler(currRot), Quaternion.Euler(nextRot), t);

                positionTime = Time.time + interpolPeriod;
            }
        }
    }
}
