using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateParrent : EditorWindow
{

    [MenuItem("Edit/Make parent &p")]
    static void SelectParentOfObject()
    {
        var select = Selection.activeGameObject.transform;
		var go = new GameObject();
		
		go.transform.SetPositionAndRotation(select.position, select.rotation);
		go.transform.localScale = select.localScale;
		
		select.parent = go.transform;

		//select.lo(Vector3.zero, Quaternion.identity);
		//select.localScale = Vector3.one;

		go.name = $"{select.name}'s mom";
		Selection.activeGameObject = go;

    }
}