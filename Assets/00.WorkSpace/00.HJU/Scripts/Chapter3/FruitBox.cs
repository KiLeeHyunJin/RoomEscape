using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBox : ClickObject
{
    [SerializeField] GameObject brickBox1;
    [SerializeField] GameObject brickBox2;
    [SerializeField] bool initialized = true;
    [SerializeField] bool check = true;

    private void Update()
    {
        if (initialized)
        {
            initialized = false;
            brickBox2.SetActive(false);
        }

        if (state == 1 && check)
        {
            check = false;
            brickBox1.SetActive(false);
            brickBox2.SetActive(true);
        }
    }


    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                PopUp();
                break; 
                
            case 1:
                break; 

        }
    }
}
