using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The class is responsible for logic SimpleBomb
[RequireComponent (typeof (Chip))]
public class SimpleBomb : MonoBehaviour {

	Chip chip;
	int birth; // Event count at the time of birth SessionAssistant.main.eventCount

    Animation anim;
	bool mMatching = false;
	bool matching {
		set {
			if (value == mMatching) return;
			mMatching = value;
			if (mMatching)
				SessionAssistant.main.matching ++;
			else
				SessionAssistant.main.matching --;
		}
		
		get {
			return mMatching;
		}
	}
	void OnDestroy () {
		matching = false;
	}

	void  Awake (){
		chip = GetComponent<Chip>();
        chip.chipType = "SimpleBomb";
		birth = SessionAssistant.main.eventCount;
        anim = GetComponent<Animation>();
		AudioAssistant.Shot ("CreateBomb");
	}

	// Coroutine destruction / activation
	IEnumerator  DestroyChipFunction (){
		if (birth == SessionAssistant.main.eventCount) {
			chip.destroying = false;
			yield break;
		}
		
		matching = true;

        yield return new WaitForSeconds(0.1f);

        anim.Play("SimpleBump");
		AudioAssistant.Shot("BombCrush");

		int sx = chip.parentSlot.slot.x;
		int sy = chip.parentSlot.slot.y;

        chip.ParentRemove();
		
		yield return new WaitForSeconds(0.05f);

		FieldAssistant.main.JellyCrush(sx, sy);

        foreach (Side side in Utils.allSides)
            NeighborMustDie(sx + Utils.SideOffsetX(side), sy + Utils.SideOffsetY(side));


        AnimationAssistant.main.Explode(transform.position, 5, 10);
        		
		yield return new WaitForSeconds(0.1f);
		matching = false;
		
		while (anim.isPlaying) yield return 0;
		Destroy(gameObject);
	}
	
	void  NeighborMustDie ( int x ,   int y  ){
        Slot s = Slot.GetSlot(x, y);
		if (s) {
			if (s.GetChip()) {
                s.GetChip().SetScore(0.3f);
                s.GetChip().DestroyChip();
			}
			FieldAssistant.main.BlockCrush(x, y, false);
			FieldAssistant.main.JellyCrush(x, y);
		}
		
	}

    public List<Chip> GetDangeredChips(List<Chip> stack) {
        if (stack.Contains(chip))
            return stack;

        stack.Add(chip);

        Slot slot;

        foreach (Side s in Utils.allSides) {
            slot = chip.parentSlot[s];
            if (slot && slot.GetChip()) {
                stack = slot.GetChip().GetDangeredChips(stack);
            }
        }

        return stack;
    }
}