using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

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
		Application.LoadLevel ("MainMenu");
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
					} else {
						ShowGameOver(true);
					}
				}
			}
		}

	}
}
