using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Steam
{
    public class SteamLobby : MonoBehaviour
    {

        public static SteamLobby instance;

        private List<GameObject> listOfLobbyListItems = new List<GameObject>();

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        protected Callback<LobbyDataUpdate_t> lobbyInfo;

        public ulong current_lobbyID;
        public List<CSteamID> lobbyIDS = new List<CSteamID>();

        private const string HostAddressKey = "HostAddress";

        private NetworkManager networkManager;

        struct LobbyMetaData
        {
            public string m_Key;
            public string m_Value;
        }

        struct LobbyMembers
        {
            public CSteamID m_SteamID;
            public LobbyMetaData[] m_Data;
        }
        struct Lobby
        {
            public CSteamID m_SteamID;
            public CSteamID m_Owner;
            public LobbyMembers[] m_Members;
            public int m_MemberLimit;
            public LobbyMetaData[] m_Data;
        }

        private void Start()
        {
            networkManager = GetComponent<NetworkManager>();

            if (!SteamManager.Initialized) { return; }
            MakeInstance();

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        }
        void MakeInstance()
        {
            if (instance == null)
                instance = this;
        }
        public void HostLobby()
        {
            networkManager.StartHost();
            //CreateNewLobby((ELobbyType)SRMLConfig.STEAM_LOBBY_MODE);
        }
        public void JoinLobby(CSteamID lobbyId)
        {
            Debug.Log("Will try to join lobby with steam id: " + lobbyId.ToString());
            SteamMatchmaking.JoinLobby(lobbyId);
        }
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            Debug.Log("Steam lobby created!");
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                return;
            }
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, $"{SteamUser.GetSteamID()}");

            networkManager.StartHost();

        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            Debug.Log($"Requested to join lobby-{callback.m_steamIDLobby}");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {

            current_lobbyID = callback.m_ulSteamIDLobby;
            Debug.Log($"Steam lobby entered. ID-{callback.m_ulSteamIDLobby}");
            if (NetworkServer.active) { return; }

            string hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey);

            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
        }
        void OnGetLobbyInfo(LobbyDataUpdate_t result)
        {
        }

        public void CreateNewLobby(ELobbyType lobbyType)
        {
            SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
        }
    }
}
