using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
	private LevelLoader levelLoad;
	public string entryPoint = "";

	private void Start()
	{
		levelLoad = Object.FindObjectOfType<LevelLoader>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == 8)
		{
			levelLoad.LoadNextLevel(this.name, entryPoint);
		}
	}
}
