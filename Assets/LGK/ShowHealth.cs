using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHealth : MonoBehaviour
{
    private Image scrollbar;
    public Health health;

    // Start is called before the first frame update
    void Start()
    {
        scrollbar = GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        scrollbar.fillAmount = health.CurrentHealth / health.MaxHealth;
        
    }
}
