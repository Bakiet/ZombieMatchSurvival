using System.Collections;
using System.Text;
using System;
using UnityEngine;
using System.Collections.Generic;

// A set of useful functions
public class Utils {

    public static readonly Side[] allSides = {
                                                 Side.Top,
                                                 Side.TopRight,
                                                 Side.Right,
                                                 Side.BottomRight,
                                                 Side.Bottom,
                                                 Side.BottomLeft,
                                                 Side.Left,
                                                 Side.TopLeft
                                             };

	public static readonly Side[] straightSides = {Side.Top, Side.Bottom, Side.Right, Side.Left};
	public static readonly Side[] slantedSides = {Side.TopLeft, Side.TopRight, Side.BottomRight ,Side.BottomLeft};

    public static string waitingStatus = "";

    public static Side RotateSide(Side side, int steps) {
        int index = Array.IndexOf(allSides, side);
        index += steps;
        index = Mathf.CeilToInt(Mathf.Repeat(index, allSides.Length));
        return allSides[index];
    }

	public static Side MirrorSide(Side s) {
		switch (s) {
		case Side.Bottom: return Side.Top;
		case Side.Top: return Side.Bottom;
		case Side.Left: return Side.Right;
		case Side.Right: return Side.Left;
		case Side.BottomLeft: return Side.TopRight;
		case Side.BottomRight: return Side.TopLeft;
		case Side.TopLeft: return Side.BottomRight;
		case Side.TopRight: return Side.BottomLeft;
		}
		return Side.Null;
	}

	public static int SideOffsetX (Side s) {
		switch (s) {
		case Side.Top:
		case Side.Bottom: 
			return 0;
		case Side.TopLeft:
		case Side.BottomLeft:
		case Side.Left: 
			return -1;
		case Side.BottomRight:
		case Side.TopRight:
		case Side.Right: 
			return 1;
		}
		return 0;
	}
	
	public static int SideOffsetY (Side s) {
		switch (s) {
		case Side.Left: 
		case Side.Right: 
			return 0;
		case Side.Bottom: 
		case Side.BottomRight:
		case Side.BottomLeft:
			return -1;
		case Side.TopLeft:
		case Side.TopRight:
		case Side.Top:
			return 1;
		}
		return 0;
	}

	public static Side SideHorizontal (Side s) {
		switch (s) {
		case Side.Left: 
		case Side.TopLeft:
		case Side.BottomLeft:
			return Side.Left;
		case Side.Right:
		case Side.TopRight:
		case Side.BottomRight:
			return Side.Right;
		default:
			return Side.Null;
		}
	}

	public static Side SideVertical (Side s) {
		switch (s) {
		case Side.Top: 
		case Side.TopLeft:
		case Side.TopRight:
			return Side.Top;
		case Side.Bottom:
		case Side.BottomLeft:
		case Side.BottomRight:
			return Side.Bottom;
		default:
			return Side.Null;
		}
	}

	public static string StringReplaceAt(string value, int index, char newchar)
	{
		if (value.Length <= index)
			return value;
		StringBuilder sb = new StringBuilder(value);
		sb[index] = newchar;
		return sb.ToString();
	}

	// Coroutine wait until the function "Action" will be true for a "delay" seconds
	public static IEnumerator WaitFor (Func<bool> Action, float delay) {
		float time = 0;
		while (time <= delay) {
			if (Action())
				time += Time.deltaTime;
			else
				time = 0;
			yield return 0;
		}
		yield break;
	}

    public static string GenerateKey(int length) {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        System.Random rnd = new System.Random();
        while (0 < length--) {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }

    public static string ToTimerFormat(float _t) {
        string f = "";
        float t = Mathf.Ceil(_t);
        float min = Mathf.FloorToInt(t / 60);
        float sec = Mathf.FloorToInt(t - 60f * min);
        f += min.ToString();
        if (f.Length < 2)
            f = "0" + f;
        f += ":";
        if (sec.ToString().Length < 2)
            f += "0";
        f += sec.ToString();
        return f;
    }

    public static object GetDataValueForKey(Dictionary<string, object> dict, string key) {
        object objectForKey;
        if (dict.TryGetValue(key, out objectForKey)) {
            return objectForKey;
        } else {
            return null;
        }
    }

    public static Vector2 Vector3to2(Vector3 vector3) {
        return new Vector2(vector3.x, vector3.y);
    }
}

class EasingFunctions {
    // no easing, no acceleration
    public static float linear(float t) {
        return t;
    }
    // accelerating from zero velocity
    public static float easeInQuad(float t) {
        return t * t;
    }
    // decelerating to zero velocity
    public static float easeOutQuad(float t) {
        return t * (2 - t);
    }
    // acceleration until halfway, then deceleration
    public static float easeInOutQuad(float t) {
        return t < .5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }
    // accelerating from zero velocity 
    public static float easeInCubic(float t) {
        return t * t * t;
    }
    // decelerating to zero velocity 
    public static float easeOutCubic(float t) {
        return (--t) * t * t + 1;
    }
    // acceleration until halfway, then deceleration 
    public static float easeInOutCubic(float t) {
        return t < .5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
    }
    // accelerating from zero velocity 
    public static float easeInQuart(float t) {
        return t * t * t * t;
    }
    // decelerating to zero velocity 
    public static float easeOutQuart(float t) {
        return 1 - (--t) * t * t * t;
    }
    // acceleration until halfway, then deceleration
    public static float easeInOutQuart(float t) {
        return t < .5f ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
    }
    // accelerating from zero velocity
    public static float easeInQuint(float t) {
        return t * t * t * t * t;
    }
    // decelerating to zero velocity
    public static float easeOutQuint(float t) {
        return 1 + (--t) * t * t * t * t;
    }
    // acceleration until halfway, then deceleration 
    public static float easeInOutQuint(float t) {
        return t < .5f ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;
    }
}


// Directions. Used as an index for links to neighboring slots.
public enum Side {
	Null, Top, Bottom, Right, Left,
	TopRight, TopLeft,
	BottomRight, BottomLeft
}

public struct int2 {
	public int x;
	public int y;
}

public delegate void Action();