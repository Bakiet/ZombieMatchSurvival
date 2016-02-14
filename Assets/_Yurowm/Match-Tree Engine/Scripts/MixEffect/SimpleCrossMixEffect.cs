using UnityEngine;
using System.Collections;

// Effect of mixing SimpleBomb and CrossBomb
// Destroys all the chips of the current horizontal and vertical by 3 lines
public class SimpleCrossMixEffect : BombMixEffect {

    Animation anim;
	void Start() {
        anim = GetComponent<Animation>();
		StartCoroutine (MixEffect ());
	}

	// Logic Effect
	IEnumerator MixEffect (){
		yield return 0;
		
		Chip chip = GetChip ();
		while (chip.parentSlot == null) yield return 0;
		transform.position = chip.parentSlot.transform.position;
		
		while (!SessionAssistant.main.CanIMatch()) yield return 0;
		
		SessionAssistant.main.matching ++;
		SessionAssistant.main.EventCounter ();
		
		anim.Play ();
        AudioAssistant.Shot("CrossBombCrush");
		
		int sx = chip.parentSlot.slot.x;
		int sy = chip.parentSlot.slot.y;
		
		yield return new WaitForSeconds(0.1f);

		FieldAssistant.main.JellyCrush(sx, sy);
		
		for (int i = 1; i < 12; i++) {
            SessionAssistant.main.EventCounter();
			Crush(sx+i, sy+1);
			Crush(sx+i, sy);
			Crush(sx+i, sy-1);
			Crush(sx-i, sy+1);
			Crush(sx-i, sy);
			Crush(sx-i, sy-1);
			Crush(sx-1, sy+i);
			Crush(sx, sy+i);
			Crush(sx+1, sy+i);
			Crush(sx-1, sy-i);
			Crush(sx, sy-i);
			Crush(sx+1, sy-i);

			Wave(sx+i, sy);
			Wave(sx-i, sy);
			Wave(sx, sy+i);
			Wave(sx, sy-i);

			yield return new WaitForSeconds(0.05f);
		}
		
		
		yield return new WaitForSeconds(0.1f);

		SessionAssistant.main.matching --;

        while (anim.isPlaying)
            yield return 0;
		FieldAssistant.main.BlockCrush(sx, sy, false);
		
		destroingLock = false;
		DestroyChipFunction ();
	}

	void Wave(int x, int y) {
        Slot s = Slot.GetSlot(x, y);
		if (s)
			AnimationAssistant.main.Explode(s.transform.position, 3, 10);
		}

	void Crush(int x, int y) {
        Slot s = Slot.GetSlot(x, y);
		FieldAssistant.main.BlockCrush(x, y, false, true);
		FieldAssistant.main.JellyCrush(x, y);

		if (s && s.GetChip()) {
            s.GetChip().SetScore(0.3f);
			s.GetChip().DestroyChip();
		}
	}
}
