using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRMP.Networking
{
    // Custom version of `NetworkManagerHUD`
    public class NetworkingMainMenuUI : SRBehaviour
    {
        MultiplayerManager manager;

        public GameObject ui;

        public int offsetX;
        public int offsetY;

        ushort port;
        string ip;

        void Awake()
        {
            manager = GetComponent<MultiplayerManager>();

            ui = transform.GetChild(0).Find("MainMenu").gameObject;

            ui.GetChild(0).GetComponent<Button>().onClick.AddListener(Host);
            ui.GetChild(1).GetComponent<Button>().onClick.AddListener(Join);
            ui.GetChild(2).GetComponent<TMP_InputField>().onValueChanged.AddListener(input =>
            {
                try
                {
                    port = ushort.Parse(input);
                }
                catch { }
            });
            ui.GetChild(4).GetComponent<TMP_InputField>().onValueChanged.AddListener(input =>
            {
                ip = input;
            });
        }

        void Host()
        {
            manager.Host(port);
        }
        void Join()
        {
            
            manager.Connect(ip, port);
        }


        void OnDisable()
        {
            ui.SetActive(false);
        }
        void OnEnable()
        {
            ui.SetActive(true);
        }
    }
}
