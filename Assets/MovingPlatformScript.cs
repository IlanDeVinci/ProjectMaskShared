using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MovingPlatformScript : MonoBehaviour
{
    [SerializeField] private List<Position> positions;
    private int currentPos = 0;
    private bool isGoingRight = true;
    private bool canMove = true;

    [Serializable]
    public class Position
    {
        [SerializeField] public Vector2 position;
        [SerializeField] public float speed;

    }

    // Start is called before the first frame update
    void Awake()
    {
        positions[0].position = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (!GlobalManager.isGamePaused)
        {
            if (canMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, positions[currentPos].position, positions[currentPos].speed * Time.deltaTime);
                if ((Vector2)transform.position == positions[currentPos].position)
                {
                    if (isGoingRight)
                    {
                        if (currentPos < positions.Count - 1)
                        {
                            currentPos++;
                        }
                        else
                        {
                            isGoingRight = false;
                        }
                    }
                    else
                    {
                        if (currentPos > 0)
                        {
                            currentPos--;
                        }
                        else
                        {
                            isGoingRight = true;
                        }
                    }

                }
            }
        }


    }
}