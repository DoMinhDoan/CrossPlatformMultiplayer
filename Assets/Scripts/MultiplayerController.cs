using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.SceneManagement;

public class MultiplayerController : RealTimeMultiplayerListener
{
    private static MultiplayerController _instance = null;

    private uint minimumOpponents = 1;
    private uint maximumOpponents = 1;
    private uint gameVariation = 0;

    public MPLobbyListener lobbyListener;

    private MultiplayerController()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public static MultiplayerController Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MultiplayerController();

            return _instance;
        }
    }

    public void SignInAndStartMPGame()
    {
        if(!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.localUser.Authenticate((bool success) =>
            {
                if(success)
                {
                    Debug.Log("Signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                    StartMatchmaking();
                }
                else
                {
                    Debug.Log("Error with Signed In");
                }
            });
        }
        else
        {
            Debug.Log("Signed In Before");
        }
    }

    public void TrySilentSignIn()
    {
        if(!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate((bool success) =>
            {
                if(success)
                {
                    Debug.Log("Signed In! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                    StartMatchmaking();
                }
                else
                {
                    Debug.Log("Error When Signed In");
                }
            }, true);
        }
        else
        {
            Debug.Log("Already Signed In");
        }
    }

    public bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance.localUser.authenticated;
    }

    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }
    
    private void StartMatchmaking()
    {
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
    }

    private void ShowMPStatus(string message)
    {
        Debug.Log(message);
        if(lobbyListener != null)
        {
            lobbyListener.SetLobbyStatusMessage(message);
        }
    }

    public void OnRoomSetupProgress(float percent)
    {
        ShowMPStatus("We are " + percent + "% done with setup");
    }

    public void OnRoomConnected(bool success)
    {
        if(success)
        {
            ShowMPStatus("We are connected to the room! I would probably start out game now.");
            lobbyListener.HideLobby();
            lobbyListener = null;
            SceneManager.LoadScene("MainGame");
        }
        else
        {
            ShowMPStatus("Error when connecting to the room.");
        }
    }

    public void OnLeftRoom()
    {
        ShowMPStatus("We have left the room.");
    }

    public void OnParticipantLeft(Participant participant)
    {
        ShowMPStatus("Player '" + participant.DisplayName + "'  has left.");
    }

    public void OnPeersConnected(string[] participantIds)
    {
        foreach (var participantId in participantIds)
        {
            ShowMPStatus("Player " + participantId + " has connected.");
        }
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        foreach (var participantId in participantIds)
        {
            ShowMPStatus("Player " + participantId + " has disconnected.");
        }
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        ShowMPStatus("We have received some gameplay messages from participant ID:" + senderId);
    }
}
