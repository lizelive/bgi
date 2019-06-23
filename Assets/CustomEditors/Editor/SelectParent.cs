using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectParent : EditorWindow
{

    [MenuItem("Edit/Select parent &z")]
    static void SelectParentOfObject()
    {
        var select = Selection.activeGameObject.transform;
        while (select.parent)
        {
            select = select.parent;
        }

        Selection.activeGameObject = select.gameObject;

    }

    [MenuItem("Edit/Move To 0 but stay same")]
    static void MoveParentSoToUnfuckThis()
    {
        var select = Selection.activeGameObject.transform;
        var pos = select.position;
        var rot = select.rotation;
        var scl = select.TransformVector(select.localScale);

        
        
        select.parent.position = pos;
        select.parent.rotation = rot;
        select.localScale = select.localScale.Mul(scl);

        select.localRotation = Quaternion.identity;
        select.localPosition = Vector3.zero;
        select.localScale = Vector3.one;
    }
}