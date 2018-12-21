using UnityEngine;
using System.Collections;

public class PickCarScript : MonoBehaviour {

	public Texture2D[] carTextures;
	public Texture2D goBackTexture;
    private float buttonWidth;
    private float buttonHeight;

    private float backButtonWidth;
    private float backButtonHeight;


	void OnGUI() {
		for (int i = 0; i < 3; i++) {
			
			if (GUI.Button (new Rect ((float)Screen.width * ((float)(i+1) * 0.25f) - (buttonWidth / 2),
			                          (float)Screen.height * 0.6f - (buttonHeight / 2),
			                          buttonWidth,
			                          buttonHeight), carTextures[i])) {
				Debug.Log("Car " + (i + 1) + " was clicked!");
				Debug.Log ("Let's load up the menu!");
				// Let's save this
				RetainedUserPicksScript.Instance.carSelected = i+1;
				Application.LoadLevel("PickDiffMenu");
			}
		}
		if (GUI.Button (new Rect (0.0f,
		                         (float)Screen.height - (backButtonHeight),
		                         backButtonWidth,
		                         backButtonHeight), goBackTexture)) {
			Debug.Log("Go back to main menu!");
			Application.LoadLevel("MainMenu");
		}
	}
    
    void Start() {

        // These button sizes were originally based on a screen width of 660    
        float screenToDesignRatio = Screen.width / 660.0f;
        buttonWidth = 121.0f * screenToDesignRatio;
        buttonHeight = 170.0f * screenToDesignRatio;
        
        backButtonWidth = 82.0f * screenToDesignRatio;
        backButtonHeight = 33.0f * screenToDesignRatio;
        
    }
    
    

}
