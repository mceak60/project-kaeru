using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
