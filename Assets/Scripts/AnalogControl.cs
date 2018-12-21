using UnityEngine;
using System.Collections;

public class AnalogControl : MonoBehaviour {

	public GameObject basePad;
	public GameObject steeringKnob;

	private Vector2 _direction;
	private Vector3 _calibration;
	private float _maxRadius;
	private float _tiltMultiplier = 1.75f;
	private bool _onMobile;
	private bool _isUsingPad;

	// Use this for initialization
	void Start () {
		// A bit of a hack to make sure the gamepad is on screen for short devices like the iPad
		
		
		float padWidth = basePad.GetComponent<SpriteRenderer>().GetComponent<Renderer>().bounds.size.x;
		Vector3 padLeft = Camera.main.WorldToViewportPoint(basePad.transform.position - new Vector3(padWidth, 0, 0));
		Vector3 padRight = Camera.main.WorldToViewportPoint(basePad.transform.position + new Vector3(padWidth, 0, 0));
		
		Debug.Log("Pad left is at " + padLeft + " and right is at " +padRight);
		if (padLeft.x < 0) {
			float scaleFactor = padWidth / (padRight.x - padLeft.x);
			transform.position = new Vector3(transform.position.x + (-padLeft.x * scaleFactor), transform.position.y, transform.position.z);
		}
		
		    
		            
		_maxRadius = basePad.GetComponent<Renderer>().bounds.size.x / 2;
		_calibration = new Vector3 (0.0f, -0.6f, 0.0f);
		// Only considering these two platforms for now
		_onMobile = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
		_isUsingPad = false;
	}

	public Vector2 GetNormalizedSteering() {
		return _direction;
	}

	

	// Update is called once per frame
	void Update () {

		// Is the user pressing down near the joystick? Track that.
		// Note: There may be a better way to do this. Feel free to tell me in the comments!
		if (Input.GetButtonDown ("Fire1")) {
			// Put the steering knob at the mouse
			Vector3 fingerHit = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	
			float distanceToKnob = Vector2.Distance(new Vector2(fingerHit.x, fingerHit.y),
			                                          new Vector2(steeringKnob.transform.position.x, steeringKnob.transform.position.y));
			if (distanceToKnob < _maxRadius) {
				_isUsingPad = true;
			}
		} else if (Input.GetButtonUp ("Fire1")) {
			_isUsingPad = false;
		}
		
		if (_isUsingPad) {

			steeringKnob.transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			// Where is it locally?
			Vector2 knobPosition = new Vector2 (steeringKnob.transform.localPosition.x, steeringKnob.transform.localPosition.y);
			// Okay, let's keep it within the bounds of the base
			knobPosition = Vector2.ClampMagnitude (knobPosition, _maxRadius);
			steeringKnob.transform.localPosition = new Vector3 (knobPosition.x, knobPosition.y, 0);

			// And let's convert this to a 0-1 scale.
			_direction = knobPosition / _maxRadius;
		} else {
			// Move the joystick back to center
			_direction = Vector2.zero;
			steeringKnob.transform.localPosition = Vector3.Lerp(steeringKnob.transform.localPosition, Vector3.zero, 12 * Time.deltaTime);
		}

		// If we're on mobile and not using the joystick, use the tilt sensor!
		if (!_isUsingPad && _onMobile) {

			steeringKnob.transform.localPosition = Vector3.Lerp(steeringKnob.transform.localPosition, Vector3.zero, 12 * Time.deltaTime);

			// We're on mobile. Let's grab the acceleration data!
			float xVal = (Input.acceleration.x - _calibration.x) * _tiltMultiplier;
			float yVal = (Input.acceleration.y - _calibration.y) * _tiltMultiplier;

			// A bit of a hack to workaround the fact that we have .4 movement in one direction and .6 movement in the other
			if (Input.acceleration.y < _calibration.y) {
				yVal /= (_calibration.y +1);
			} else {
				yVal /= -(_calibration.y);
			}
			_direction = new Vector2(xVal, yVal);
			_direction = Vector2.ClampMagnitude(_direction, 1.0f);
		} 
		
		

	}
}
