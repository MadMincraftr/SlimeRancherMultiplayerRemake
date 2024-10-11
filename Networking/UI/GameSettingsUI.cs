using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SRMP.Networking.UI
{
    public class GameSettingsUIButton : MonoBehaviour
    {
        public void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                var modSettings = MainSRML.modSettingsUI.Instantiate().GetComponent<GameSettingsUI>();
                NewGameUI newGameUI = Resources.FindObjectsOfTypeAll<NewGameUI>().FirstOrDefault(x => x.gameObject.scene.isLoaded);
                modSettings.createGameUI = newGameUI;
                newGameUI.gameObject.SetActive(false);
            });
        }
    }
    public class GameSettingsUI : BaseUI
    {
        public NewGameUI createGameUI;

        public Transform optionList;

        public SRToggle shareMoneyToggle;
        public SRToggle shareKeysToggle;
        public SRToggle shareUpgradesToggle;

        void Awake()
        {
            foreach (var text in transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if (text.name != "Desc")
                {
                    text.alignment = TextAlignmentOptions.Center;
                }
                else
                {
                    text.alignment = TextAlignmentOptions.TopLeft;
                }
                text.enabled = false;
                text.enabled = true;
            }

            optionList = transform.GetChild(0).GetChild(2).GetChild(0);
            transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(() => 
            {
                Close();
            });

            shareMoneyToggle = optionList.GetChild(0).GetComponent<SRToggle>();
            shareKeysToggle = optionList.GetChild(1).GetComponent<SRToggle>();
            shareUpgradesToggle = optionList.GetChild(2).GetComponent<SRToggle>();
        }

        void SetEventsOnToggles()
        {
            shareKeysToggle.onValueChanged.AddListener(val =>
            {
                SRNetworkManager.initialWorldSettings.shareKeys = val;
            });

            shareMoneyToggle.onValueChanged.AddListener(val =>
            {
                SRNetworkManager.initialWorldSettings.shareMoney = val;
            });

            shareUpgradesToggle.onValueChanged.AddListener(val =>
            {
                SRNetworkManager.initialWorldSettings.shareUpgrades = val;
            });
        }

        void Start()
        {

            SRNetworkManager.initialWorldSettings = new NetGameInitialSettings(true); 

            SetEventsOnToggles();

            onDestroy += () =>
            {
                createGameUI.gameObject.SetActive(true);
            };
        }
    }
}
