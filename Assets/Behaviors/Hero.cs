using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Hero : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // implement

        //check if i can see the player.
        foreach (var player in FindObjectsOfType<Player>())
        {

        }
        /*
         * priorities
         * 1. kill anything nerby that is evil (including turrets and stuff)
         * 1b. Tbag your body
         * 2. steal EVERYTHING (regardless of team)
         * 3. 'Liberate' Villagers (of their gold)
         * 3d.  destory defences (implemente via 
         * 3b.  kill taxman
         * 3c.  blow up all red barrles
         * 4. be an idiot
         * 4a. dance at people
         * 4b. (try to) kill annoying children npcs
         */
    }
}
