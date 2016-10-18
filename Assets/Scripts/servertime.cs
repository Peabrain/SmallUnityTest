using UnityEngine;
using System.Collections;

public static class ServerTime {

    public static float time = 0.0f;	
	// Update is called once per frame
	public static void Update (float delta)
    {
        time += delta;
    }
}
