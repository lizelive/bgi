using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMultiScene : MonoBehaviour
{
	public string[] loadmepls;
    // Start is called before the first frame update
    void Start()
    {
		foreach (var name in loadmepls)
		{

			SceneManager.LoadScene(name, LoadSceneMode.Additive);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
