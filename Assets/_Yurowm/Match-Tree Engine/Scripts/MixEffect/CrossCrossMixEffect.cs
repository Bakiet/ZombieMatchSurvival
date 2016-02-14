using UnityEngine;
using System.Collections;

// Effect of mixing 2 CrossBomb
// Destroys all the chips of the current horizontal, vertical and two diagonals
public class CrossCrossMixEffect : BombMixEffect {

	int sx = 0;
	int sy = 0;

    bool mMatching = false;
    bool matching {
        set {
            if (value == mMatching)
                return;
            mMatching = value;
            if (mMatching)
                SessionAssistant.main.matching++;
            else
                SessionAssistant.main.matching--;
        }

        get {
            return mMatching;
        }
    }

	void Start() {
		StartCoroutine (MixEffect ());
	}

	// Effect logic
	IEnumerator MixEffect (){
		yield return 0;
        Chip chip = GetComponent<Chip>();
        Animation anim = GetComponent<Animation>();

		while (chip.parentSlot == null) yield return 0;

		transform.position = chip.parentSlot.transform.position;

        while (!SessionAssistant.main.CanIMatch()) yield return 0;

        matching = true;

        anim.Play("CrossCrossBump");
        AudioAssistant.Shot("CrossBombCrush");

        int sx = chip.parentSlot.slot.x;
        int sy = chip.parentSlot.slot.y;

        chip.ParentRemove();

        FieldAssistant.main.BlockCrush(sx, sy, false);
        FieldAssistant.main.JellyCrush(sx, sy);

        int count = 4;
        for (int path = 1; count > 0; path++) {
            count = 0;
            foreach (Side side in Utils.allSides)
                if (CrossBomb.Freez(sx + Utils.SideOffsetX(side) * path, sy + Utils.SideOffsetY(side) * path))
                    count++;
        }

        count = 4;
        for (int path = 1; count > 0; path++) {
            count = 0;
            yield return new WaitForSeconds(0.02f);
            foreach (Side side in Utils.allSides)
                if (CrossBomb.Crush(sx + Utils.SideOffsetX(side) * path, sy + Utils.SideOffsetY(side) * path))
                    count++;
        }
        
        yield return new WaitForSeconds(0.1f);

        matching = false;

        while (anim.isPlaying)
            yield return 0;

		destroingLock = false;
		DestroyChipFunction ();
	}

	bool CheckTarget(int x, int y) {
		if (x == sx) return true;
		if (y == sy) return true;
		if (x - y == sx - sy) return true;
		if (x + y == sx + sy) return true;
		return false;
	}
}
