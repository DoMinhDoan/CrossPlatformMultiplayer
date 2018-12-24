using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCarController : MonoBehaviour
{
    public Sprite[] carSprites;

    private Vector3 _startPos;
    private Vector3 _destinationPos;

    private Quaternion _startRot;
    private Quaternion _destinationRot;

    private Vector3 _lastKnownVel;

    private float _lastTimeUpdate;
    private float _timePerUpdate = 0.16f;

    public float lastTimeUpdate
    {
        get
        {
            return _lastTimeUpdate;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;

        _lastTimeUpdate = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float interval = (Time.time - _lastTimeUpdate) / _timePerUpdate;

        if(interval <= 1.0)
        {
            transform.position = Vector3.Lerp(_startPos, _destinationPos, interval);
            transform.rotation = Quaternion.Slerp(_startRot, _destinationRot, interval);
        }
        else   //extrapolation 
        {
            // Adding this extrapolation means it takes you longer to bring the player to the point where they were a few milliseconds ago! 
            // And if your opponent's updates take too long to arrive, their car will calmly drive off the edge of the playing field.
            transform.position = transform.position + (_lastKnownVel * Time.deltaTime);
        }
    }

    public void SetCarNumber(int number)
    {
        GetComponent<SpriteRenderer>().sprite = carSprites[number - 1];
    }

    public void SetCarInformation(float posX, float posY, float velX, float velY, float rotZ)
    {
        // Let interpolation instead
        //transform.position = new Vector3(posX, posY, 0);
        //transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // Interpolation
        _startPos = transform.position;
        _startRot = transform.rotation;

        _destinationPos = new Vector3(posX, posY, 0);
        _destinationRot = Quaternion.Euler(0, 0, rotZ);

        _lastTimeUpdate = Time.time;

        // we're going to do nothing with velocity ... for now
        _lastKnownVel = new Vector3(velX, velY, 0);
    }

    public void HideCar()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }   
}
