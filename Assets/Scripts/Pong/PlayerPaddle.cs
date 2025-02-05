using System;
using UnityEngine;

namespace Pong
{
    public class PlayerPaddle : Paddle
    {
        private Vector2 direction;
        public int ballHit = 0;

        private void Update()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                direction = Vector2.up;
            } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                direction = Vector2.down;
            } else {
                direction = Vector2.zero;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D obj)
        {
            if (obj.name == "Ball")
            {
                ballHit++;
            }
        }

        private void FixedUpdate()
        {
            if (direction.sqrMagnitude != 0) {
                rb.AddForce(direction * speed);
            }
        }

    }
}
