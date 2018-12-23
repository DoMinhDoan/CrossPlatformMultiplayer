using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MPLobbyListener
{
    void SetLobbyStatusMessage(string message);
    void HideLobby();
}
