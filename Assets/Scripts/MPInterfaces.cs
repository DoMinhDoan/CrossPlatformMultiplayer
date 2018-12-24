using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MPLobbyListener
{
    void SetLobbyStatusMessage(string message);
    void HideLobby();
}

public interface MPUpdateListener
{
    void UpdateReceived(string senderId, float posX, float posY, float velX, float velY, float rotZ);
    void PlayerFinished(string senderId, float finalTime);
}
