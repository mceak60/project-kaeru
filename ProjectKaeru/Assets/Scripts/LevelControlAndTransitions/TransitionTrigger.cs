using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
	private LevelLoader levelLoad; // The level loader script that should be attached to some gameObject in the scene. There should be one and only one or this script will not work!
	public string entryPoint = ""; // The name of the point that the player will start at in the new scene. It should be a child of the gameObject with the level manager script on it

	private void Start()
	{
		levelLoad = Object.FindObjectOfType<LevelLoader>(); //Finds levelLoader script
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == 8)
		{
			levelLoad.LoadNextLevel(this.name, entryPoint); // Loads the scene that the NAME of this gmaeObject points to and sends the point that the player will start at in the new scene
		}
	}
}
