using UnityEngine;
using System.Collections;

public class GuiController : MonoBehaviour {

	public GUIText lapText;
	public GUIText timeText;

	private int _laps;
	private float _timeToShow;

	// Use this for initialization
	void Start () {
	
	}

	public void SetLaps(int laps) {
		_laps = laps;
	}

	public void SetTime(float timeToShow) {
		_timeToShow = timeToShow;
	}




	// Update is called once per frame
	void Update () {
		lapText.text = "Laps: " + _laps;
		timeText.text = "Time: " + _timeToShow.ToString("F1");

	}
}
