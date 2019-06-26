using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHealth : MonoBehaviour
{
    private Scrollbar scrollbar;
    public Health health;
    // Start is called before the first frame update
    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
        
    }

    // Update is called once per frame
    void Update()
    {
        scrollbar.size = health.CurrentHealth / health.MaxHealth;
        
    }
}
