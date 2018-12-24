using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour, MPUpdateListener {

	public GameObject myCar;
	public GuiController guiObject;
	public GUISkin guiSkin;
	public GameObject background;
	public Sprite[] backgroundSprites;
	public float[] startTimesPerLevel;
	public int[] lapsPerLevel;

	public bool _paused;
	private float _timeLeft;
	private float _timePlayed;
	private int _lapsRemaining;
	private bool _showingGameOver;
	private bool _multiplayerGame;
	private string gameOvertext;
	private float _nextCarAngleTarget = Mathf.PI;
	private const float FINISH_TARGET = Mathf.PI;

    // Multiplayer
    public GameObject opponentPrefab;

    private bool _multiplayerReady;
    private string _myParticipantID;
    private Vector2 _startingPoint = new Vector2(0.09675431f, -1.752321f);
    private float _startingPointYOffset = 0.2f;
    private Dictionary<string, OpponentCarController> _opponentScripts;
    private Dictionary<string, float> _finishTimes;

    private float _nextBroadcastTime = 0;

	// Use this for initialization
	void Start () {
		RetainedUserPicksScript userPicksScript = RetainedUserPicksScript.Instance;
		_multiplayerGame = userPicksScript.multiplayerGame;
		if (! _multiplayerGame) {
			// Can we get the car number from the previous menu?
			myCar.GetComponent<CarController>().SetCarChoice(userPicksScript.carSelected, false);
			// Set the background
			background.GetComponent<SpriteRenderer>().sprite = backgroundSprites[userPicksScript.diffSelected - 1];
			// Set our time left and laps remaining
			_timeLeft = startTimesPerLevel[userPicksScript.diffSelected - 1];
			_lapsRemaining = lapsPerLevel[userPicksScript.diffSelected - 1];

			guiObject.SetTime (_timeLeft);
			guiObject.SetLaps (_lapsRemaining);
		} else {
            SetupMultiplayerGame();
		}

	}

    void SetupMultiplayerGame() {
        // TODO: Fill this out!

        MultiplayerController.Instance.updateListener = this;

        _myParticipantID = MultiplayerController.Instance.GetMyParticipantID();
        List<Participant> allPlayers = MultiplayerController.Instance.GetAllPlayer();
        _opponentScripts = new Dictionary<string, OpponentCarController>(allPlayers.Count - 1);
        _finishTimes = new Dictionary<string, float>(allPlayers.Count);
        for(int i = 0; i< allPlayers.Count; i++)
        {
            Vector3 startPosition = new Vector3(_startingPoint.x, _startingPoint.y + (i * _startingPointYOffset), 0);
            string participantID = allPlayers[i].ParticipantId;
            _finishTimes[participantID] = -1;
            if(participantID == _myParticipantID)   //player car
            {
                myCar.transform.position = startPosition;
                myCar.GetComponent<CarController>().SetCarChoice(i + 1, true);
            }
            else
            {
                GameObject opponentCar = Instantiate(opponentPrefab, startPosition, Quaternion.identity);

                OpponentCarController opponentCarScript = opponentCar.GetComponent<OpponentCarController>();
                opponentCarScript.SetCarNumber(i + 1);
                _opponentScripts[participantID] = opponentCarScript;
            }
        }
        _lapsRemaining = 3;
        _timePlayed = 0;
        guiObject.SetLaps(_lapsRemaining);
        guiObject.SetTime(_timePlayed);
        _multiplayerReady = true;
    }


	void PauseGame() {
		_paused = true;
		myCar.GetComponent<CarController>().SetPaused(true);
	}
	
	void ShowGameOver(bool didWin) {
		gameOvertext = (didWin) ? "Woo hoo! You win!" : "Awww... better luck next time";
		PauseGame ();
		_showingGameOver = true;
		Invoke ("StartNewGame", 3.0f);
	}

	void StartNewGame() {
        SceneManager.LoadScene("MainMenu");
	}

	void OnGUI() {
		if (_showingGameOver) {
			GUI.skin = guiSkin;
			GUI.Box(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.5f), gameOvertext);

		}
	}
    
    void DoMultiplayerUpdate() {
        // In a multiplayer game, time counts up!
        _timePlayed += Time.deltaTime;
        guiObject.SetTime(_timePlayed);

        // We will be doing more here

        // Here you send all information the other players need to display the local player's car appropriately: their x and y coordinates, z-axis rotaion, and the car's current velocity.

        if(Time.time > _nextBroadcastTime)
        {
            MultiplayerController.Instance.SendMyUpdate(myCar.transform.position.x, myCar.transform.position.y, myCar.GetComponent<Rigidbody2D>().velocity, myCar.transform.rotation.eulerAngles.z);
            _nextBroadcastTime = Time.time + 0.16f;
        }        
    }
	
	void Update () {
		if (_paused) {
			return;
		}

		if (_multiplayerGame) {
            DoMultiplayerUpdate();
		} else {
			_timeLeft -= Time.deltaTime;
			guiObject.SetTime (_timeLeft);
			if (_timeLeft <= 0) {
				ShowGameOver (false);
			}
		}

		float carAngle = Mathf.Atan2 (myCar.transform.position.y, myCar.transform.position.x) + Mathf.PI;
		if (carAngle >= _nextCarAngleTarget && (carAngle - _nextCarAngleTarget) < Mathf.PI / 4) {
			_nextCarAngleTarget += Mathf.PI / 2;
			if (Mathf.Approximately(_nextCarAngleTarget, 2*Mathf.PI)) _nextCarAngleTarget = 0;
			if (Mathf.Approximately(_nextCarAngleTarget, FINISH_TARGET)) {
				_lapsRemaining -= 1;
				Debug.Log("Next lap finished!");
				guiObject.SetLaps (_lapsRemaining);
				myCar.GetComponent<CarController>().PlaySoundForLapFinished();
				if (_lapsRemaining <= 0) {
					if (_multiplayerGame) {
                        // TODO: Properly finish a multiplayer game
                        myCar.GetComponent<CarController>().Stop();
                        MultiplayerController.Instance.SendMyUpdate(myCar.transform.position.x, myCar.transform.position.y, new Vector2(0, 0), myCar.transform.rotation.eulerAngles.z);

                        MultiplayerController.Instance.SendFinishMessage(_timePlayed);

                        // you need to tell the local device that the game is done, as calling SendMessageToAll() doesn't send a message to the local player.
                        PlayerFinished(_myParticipantID, _timePlayed);
                    } else {
						ShowGameOver(true);
					}
				}
			}
		}

	}

    public void UpdateReceived(string senderId, float posX, float posY, float velX, float velY, float rotZ)
    {
        if(_multiplayerReady)
        {
            OpponentCarController opponent = _opponentScripts[senderId];
            if(opponent != null)
            {
                opponent.SetCarInformation(posX, posY, velX, velY, rotZ);
            }

        }
    }

    public void PlayerFinished(string senderId, float finalTime)
    {
        if (_multiplayerReady)
        {
            if(_finishTimes[senderId] == -1)
            {
                _finishTimes[senderId] = finalTime;
            }

            CheckForMPGameOver();
        }
    }

    private void CheckForMPGameOver()
    {
        float myTime = _finishTimes[_myParticipantID];
        int myLevel = 0;

        foreach(var nextTime in _finishTimes.Values)
        {
            if(nextTime == -1)  //someone haven't finished yet. waiting
            {
                return;
            }
            if(nextTime < myTime)
            {
                myLevel++;
            }
        }

        string[] ranks = new string[] { "1st", "2nd", "3rd", "4th" };

        gameOvertext = "Congratulation! You are in " + ranks[myLevel] + " with " + myTime + "s";
        PauseGame();
        _showingGameOver = true;

        // TODO: Leave the room and go back to the main menu
    }
}
