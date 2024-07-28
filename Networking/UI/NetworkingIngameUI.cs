using Mirror;
using SRMP.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRMP.Networking.UI
{
    public class NetworkingIngameUI : SRBehaviour
    {
        public NetworkManager manager;

        public GameObject ui;

        public int offsetX;
        public int offsetY;

        void OnDisable()
        {
            ui.SetActive(false);
        }
        void OnEnable()
        {
            ui.SetActive(true);
        }
        private ushort port = 7777;
        void Awake()
        {
            manager = GetComponent<NetworkManager>();
            ui = transform.GetChild(0).Find("DisconnectedInGame").gameObject;
            ui.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
                MultiplayerManager.Instance.Host(port);
                enabled = false;
            });
            ui.GetChild(1).GetComponent<TMP_InputField>().onValueChanged.AddListener(input =>
            {
                try
                {
                    port = ushort.Parse(input);
                }
                catch { }
            });
            ui.GetChild(1).GetComponent<TMP_InputField>().text = SRMLConfig.DEFAULT_PORT.ToString();
        }
    }
}
