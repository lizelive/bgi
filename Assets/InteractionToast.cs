using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionToast : MonoBehaviour
{
	public Player player;
	public Text text;
    // Start is called before the first frame update
    void Start()
    {
		if (!player)
			player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
		var thingYouCanInteractWith = player.gameObject.Find<Interactable>(player.interactionRange).Closest(player.gameObject);
		
		//gameObject.SetActive(thingYouCanInteractWith);

		if (thingYouCanInteractWith)
		{
			text.text = thingYouCanInteractWith.message;
			var castFrom = Camera.main.pos();
			var dir = thingYouCanInteractWith.pos()- castFrom;

			transform.LookAt(castFrom);

			if (Physics.Raycast(castFrom, dir, out var hit))
				transform.position = hit.point + hit.normal;
		}
	}
}
