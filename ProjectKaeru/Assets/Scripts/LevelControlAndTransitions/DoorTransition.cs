using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script is the same as the TransitionTrigger script but you have to press the "Enter" key to change scenes
public class DoorTransition : MonoBehaviour
{
	private LevelLoader levelLoad;
	public string entryPoint = "";
	private bool canEnter = false;

	private void Start()
	{
		levelLoad = Object.FindObjectOfType<LevelLoader>();
	}

	void Update()
	{
		if (canEnter && Input.GetButtonDown("Enter"))
		{
			levelLoad.LoadNextLevel(this.name, entryPoint);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == 8)
		{
			canEnter = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer == 8)
		{
			canEnter = false;
		}
	}
}
