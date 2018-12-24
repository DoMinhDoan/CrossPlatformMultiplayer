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
    public MPUpdateListener updateListener;

    private byte _protocolVersion = 1;
    // Byte + Byte + 2 floats for position + 2 floats for velocity + 1 float for rotation z
        // 1 Byte = protocol
        // 1 Byte = 'U'
        // 8 Byte = 2 floats position
        // 8 Byte = 2 floats velocity
        // 4 Byte = 1 float rotaion x
        // 12 Byte = Total

    private int _updateMessageLength = 22;
    private List<byte> _updateMessage;

    private MultiplayerController()
    {
        _updateMessage = new List<byte>(_updateMessageLength);

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

    public List<Participant> GetAllPlayer()
    {
        // It's worth noting here that the library already sorts this list for you by participantId, so you don't need to sort it yourself.
        return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
    }

    public string GetMyParticipantID()
    {
        return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
    }

    public void SendMyUpdate(float posX, float posY, Vector2 velocity, float rotZ)
    {
        _updateMessage.Clear();
        _updateMessage.Add(_protocolVersion);
        _updateMessage.Add((byte)'U');
        _updateMessage.AddRange(System.BitConverter.GetBytes(posX));
        _updateMessage.AddRange(System.BitConverter.GetBytes(posY));
        _updateMessage.AddRange(System.BitConverter.GetBytes(velocity.x));
        _updateMessage.AddRange(System.BitConverter.GetBytes(velocity.y));
        _updateMessage.AddRange(System.BitConverter.GetBytes(rotZ));

        byte[] messageToSend = _updateMessage.ToArray();
        Debug.Log("Sending my update message " + messageToSend + " to all player in the room");
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, messageToSend);
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

        byte messageVersion = (byte)data[0];

        char messageType = (char)data[1];
        if(messageType == 'U' && data.Length == _updateMessageLength)
        {
            float posX = System.BitConverter.ToSingle(data, 2);
            float posY = System.BitConverter.ToSingle(data, 6);
            float velX = System.BitConverter.ToSingle(data, 10);
            float velY = System.BitConverter.ToSingle(data, 14);
            float rotZ = System.BitConverter.ToSingle(data, 18);

            Debug.Log("Player:" + senderId + " position:(" + posX + "," + posY + ") Velocity:(" + velX + "," + velY + ") Rotation:" + rotZ);

            if(updateListener != null)
            {
                updateListener.UpdateReceived(senderId, posX, posY, velX, velY, rotZ);
            }
        }

    }
}
