using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TutorialNetworkManager : MonoBehaviour
{

	private NetworkManager networkManager;

    void Start() {
        networkManager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update() {

    	// automatically start the server
        if (!NetworkServer.active) {
        	networkManager.StartHost();
        }
    }
}
