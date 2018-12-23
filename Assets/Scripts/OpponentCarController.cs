using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCarController : MonoBehaviour
{
    public Sprite[] carSprites;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCarNumber(int number)
    {
        GetComponent<SpriteRenderer>().sprite = carSprites[number - 1];
    }
}
