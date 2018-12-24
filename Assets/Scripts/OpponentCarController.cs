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

    public void SetCarInformation(float posX, float posY, float velX, float velY, float rotZ)
    {
        transform.position = new Vector3(posX, posY, 0);
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // we're going to do nothing with velocity ... for now
    }
}
