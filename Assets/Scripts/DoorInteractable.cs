<<<<<<< Updated upstream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorInteractable : MonoBehaviour
{
	private const float DefaultInteractionRadius = 3f;
	private Transform	_playerTransform;
	public float InteractionRadius = DefaultInteractionRadius;

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
			SceneManager.LoadScene("InsideHouse");
		}
	}
}
=======
<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorInteractable : MonoBehaviour
{
	private const float DefaultInteractionRadius = 3f;
	private Transform	_playerTransform;
	public float InteractionRadius = DefaultInteractionRadius;

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
			SceneManager.LoadScene("InsideHouse");
		}
	}
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorInteractable : MonoBehaviour
{
	private const float DefaultInteractionRadius = 3f;
	private Transform	_playerTransform;
	public float InteractionRadius = DefaultInteractionRadius;

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
			SceneManager.LoadScene("InsideHouse");
		}
	}
}
>>>>>>> 9ca45ce5eb1eded52ea154d0b3a74154db97109c
>>>>>>> Stashed changes
