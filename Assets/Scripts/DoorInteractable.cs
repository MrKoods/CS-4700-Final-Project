using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorInteractable : MonoBehaviour
{
	private const float DefaultInteractionRadius = 7f;
	private Transform	_playerTransform;
	public float InteractionRadius = DefaultInteractionRadius;

	[Header("Scene Settings")]
	public string sceneToLoad;

	[Header("UI")]
	public GameObject interactPrompt;


	void Start()
	{
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

		if (playerObj != null)
			_playerTransform = playerObj.transform;
		else
			Debug.LogWarning("No GameObject with tag 'Player' found.", this);

		if (interactPrompt != null)
      		interactPrompt.SetActive(false);
	}

	void Update()
	{
		if (_playerTransform == null)
			return;

		float distance = Vector3.Distance(transform.position, _playerTransform.position);

		bool inRange = distance <= InteractionRadius;

		if (interactPrompt != null)
			interactPrompt.SetActive(inRange);
		
		if (inRange && Input.GetKeyDown(KeyCode.E))
		{
			if(!string.IsNullOrEmpty(sceneToLoad))
				SceneManager.LoadScene(sceneToLoad);
			else
				Debug.LogWarning("No scene assigned to this door!", this);
		}
	}
}
