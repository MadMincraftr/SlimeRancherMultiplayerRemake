using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    public class NetworkPlayer : MonoBehaviour
    {
        internal int id;
        float transformTimer = 0.1f;
        public Camera cam;
        public void InitCamera()
        {
            cam = gameObject.GetComponentInChildren<Camera>();
        }

        public void StartCamera()
        {
            MultiplayerManager.Instance.currentPreviewRenderer = this;
            cam.enabled = true;
            cam.targetTexture = MultiplayerManager.Instance.playerCameraPreviewImage;
            MultiplayerManager.Instance.AddPreviewToUI();
        }
        public void StopCamera()
        {
            cam.enabled = false;
            cam.targetTexture = null;
        }

        public void Update()
        {
            transformTimer -= Time.deltaTime;
            if (transformTimer < 0)
            {
                transformTimer = 0.1f;


                var packet = new PlayerUpdateMessage()
                {
                    id = id,
                    pos = transform.position,
                    rot = transform.rotation
                };
                SRNetworkManager.NetworkSend(packet);
            }
            
        }
    }
}
