using UnityEngine;
using System.Collections;

public class PickDiffScript : MonoBehaviour {

	public Texture2D[] diffTextures;
	public Texture2D goBackTexture;
    private float buttonWidth;
    private float buttonHeight;
    
    private float backButtonWidth;
    private float backButtonHeight;
    
	void OnGUI() {
		for (int i = 0; i < 3; i++) {
			
			if (GUI.Button (new Rect ((float)Screen.width * 0.5f - (buttonWidth / 2),
			                          (float)Screen.height * (0.4f + (i * 0.2f)) - (buttonHeight / 2),
			                          buttonWidth,
			                          buttonHeight), diffTextures[i])) {
				Debug.Log("Diff " + (i + 1) + " was clicked!");
				
				// Let's save this
				RetainedUserPicksScript.Instance.diffSelected = i+1;
				Application.LoadLevel("MainGame");
			}
		}
		if (GUI.Button (new Rect (0.0f,
		                          (float)Screen.height - (backButtonHeight),
		                          backButtonWidth,
		                          backButtonHeight), goBackTexture)) {
			Debug.Log("Go back to car selection!");
			Application.LoadLevel("PickCarMenu");
		}
	}
    
    void Start() {
        // These button sizes were originally based on a screen width of 660    
        float screenToDesignRatio = Screen.width / 660.0f;
        buttonWidth = 301.0f * screenToDesignRatio;
        buttonHeight = 55.0f * screenToDesignRatio;
        
        backButtonWidth = 82.0f * screenToDesignRatio;
        backButtonHeight = 33.0f * screenToDesignRatio;
    }
}
