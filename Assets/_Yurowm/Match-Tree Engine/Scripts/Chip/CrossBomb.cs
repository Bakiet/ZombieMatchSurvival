using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The class is responsible for logic CrossBomb
[RequireComponent (typeof (Chip))]
public class CrossBomb : MonoBehaviour {
	
	Chip chip;
	int birth; // Event count at the time of birth SessionAssistant.main.eventCount
    int branchCount;

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
        anim = GetComponent<Animation>();
		chip = GetComponent<Chip>();
		chip.chipType = "CrossBomb";
		birth = SessionAssistant.main.eventCount;
		AudioAssistant.Shot ("CreateCrossBomb");
	}

    // Coroutine destruction / activation
    IEnumerator DestroyChipFunction() {
        if (birth == SessionAssistant.main.eventCount) {
            chip.destroying = false;
            yield break;
        }

        while (transform.position != chip.parentSlot.slot.transform.position)
            yield return 0;

        matching = true;

        anim.Play("CrossBump");
        AudioAssistant.Shot("CrossBombCrush");
        
        int sx = chip.parentSlot.slot.x;
        int sy = chip.parentSlot.slot.y;

        chip.ParentRemove();

        FieldAssistant.main.BlockCrush(sx, sy, false);
        FieldAssistant.main.JellyCrush(sx, sy);

        int count = 4;
        for (int path = 1; count > 0; path++) {
            count = 0;
            foreach (Side side in Utils.straightSides) {
                if (Freez(sx + Utils.SideOffsetX(side) * path, sy + Utils.SideOffsetY(side) * path))
                    count++;
            }
        }

        count = 4;
        for (int path = 1; count > 0; path++) {
            count = 0;
            yield return new WaitForSeconds(0.02f);
            foreach (Side side in Utils.straightSides)
                if (Crush(sx + Utils.SideOffsetX(side) * path, sy + Utils.SideOffsetY(side) * path))
                    count++;
        }

        yield return new WaitForSeconds(0.1f);

        matching = false;

        while (anim.isPlaying)
            yield return 0;
        Destroy(gameObject);
    }

    public static bool Crush(int x, int y) {
        Slot s = Slot.GetSlot(x, y);
        FieldAssistant.main.BlockCrush(x, y, false, true);
        FieldAssistant.main.JellyCrush(x, y);
        if (s && s.GetChip()) {
            s.GetChip().SetScore(0.3f);
            s.GetChip().DestroyChip();
            AnimationAssistant.main.Explode(s.transform.position, 3, 7);
        }
        return x >= 0 && y >= 0 && x < FieldAssistant.main.field.width && y < FieldAssistant.main.field.height;
    }

    public static bool Freez(int x, int y) {
        Slot s = Slot.GetSlot(x, y);
        if (s && s.GetChip() && s.GetChip().destroyable && !(s.GetBlock() && s.GetBlock() is Branch))
            s.GetChip().can_move = false;
        return x >= 0 && y >= 0 && x < FieldAssistant.main.field.width && y < FieldAssistant.main.field.height;
    }

    public List<Chip> GetDangeredChips(List<Chip> stack) {
        if (stack.Contains(chip) || !chip.parentSlot)
            return stack;

        stack.Add(chip);

        int sx = chip.parentSlot.slot.x;
        int sy = chip.parentSlot.slot.y;

        Slot s;

        for (int x = 0; x < FieldAssistant.main.field.width; x++) {
            if (x == sx)
                continue;
            s = Slot.GetSlot(x, sy);
            if (s && s.GetChip() && s.GetChip().id == chip.id) {
                stack = s.GetChip().GetDangeredChips(stack);
            }
        }

        for (int y = 0; y < FieldAssistant.main.field.height; y++) {
            if (y == sy)
                continue;
            s = Slot.GetSlot(sx, y);
            if (s && s.GetChip()) {
                stack = s.GetChip().GetDangeredChips(stack);
            }
        }

        return stack;
    }
	
}