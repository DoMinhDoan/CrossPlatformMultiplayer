using UnityEngine;
using System.Collections;

/**
 * Used just to pass information from one scene to the next
 */

public class RetainedUserPicksScript {

	public int carSelected;
	public int diffSelected;
	public bool multiplayerGame;
	private static RetainedUserPicksScript _instance = null;
	
	private RetainedUserPicksScript() {
		// Anything to init would go here
	}
	
	public static RetainedUserPicksScript Instance {
		get {
			if (_instance == null) {
				_instance = new RetainedUserPicksScript();
			}
			return _instance;
		}
	}
	
}
