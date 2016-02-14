using UnityEngine;

// Pause object
public class GamePauseObject : MonoBehaviour {

	public static int activeObjectsCount = 0; // Count of active objects of this type. If set to null, the game is paused.

    bool _status = false;
    bool status {
        get {
            return _status;
        }
        set {
            if (_status != value) {
                _status = value;
                if (_status)
                    activeObjectsCount++;
                else
                    activeObjectsCount--;
            }
        }
    }

	void OnEnable () {
        status = true;
	}

    void OnDisable() {
        status = false;
	}
}
