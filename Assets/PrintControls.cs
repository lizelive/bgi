using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintControls : MonoBehaviour
{

	//Doctor Who - The Horns of Nimon

	// Start is called before the first frame update
	void Start()
	{
		var keycodes = System.Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();

		var c = Default.I.controls;
		var fields = c.GetType().GetFields();


		var keybinds = fields
			.Where(x => x.FieldType == typeof(KeyCode))
			.ToDictionary(
				k => k.Name,
				k => k.GetValue(c)
				);

		var keysUsed = keybinds.Values.Distinct().ToArray();

		text.text = string.Join("\n", keybinds.Select(x => $"{x.Key} = {x.Value}"));
		unusedKeycodes = keycodes.Where(x => !keysUsed.Contains(x)).ToArray();
	}

	public KeyCode[] unusedKeycodes;
	UnityEngine.UI.Text text => GetComponent<UnityEngine.UI.Text>();
	public bool showHelp;
	// Update is called once per frame
	void Update()
	{
		var showHelp = unusedKeycodes.Any(Input.GetKey);
		//showHelp ^= toggle;
		text.enabled = showHelp;
	}
}
