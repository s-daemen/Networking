﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {



    [SerializeField]
    private GameObject _playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;


    private void Start() {

        if (!isLocalPlayer) return;

        playerUIInstance = Instantiate(_playerUIPrefab);
        playerUIInstance.name = _playerUIPrefab.name;


        PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
        if (ui == null)
            Debug.LogError("No PlayerUI component on PlayerUI prefab.");
        ui.SetPlayer(GetComponent<Player>());
        GetComponent<Player>().SetupPlayer();


        var username = "Loading...";
        username = AccountManager.IsLoggedIn ? AccountManager.User.UserName : transform.name;

        CmdSetUsername(transform.name, username);
    }

    [Command]
    private void CmdSetUsername(string playerId, string userName) {

        Player player = GameManager.GetPlayer(playerId);
        if (player == null) return;
        player.PlayerName = userName;
    }

    [ClientRpc]
    private void RpcSetUsername(string playerId, string userName)
    {
        if (!isLocalPlayer) return;
        Player player = GameManager.GetPlayer(playerId);
        if (player == null) return;
        player.PlayerName = userName;
    }

    public override void OnStartClient() {
        base.OnStartClient();

        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netId, player);
    }

    private void OnDisable() {
        GameManager.UnRegisterPlayer(transform.name);
    }
}