using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event Action<GameObject> PlayerHitReactableObject;
    [SerializeField] private float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput -= 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput += 1f;
        }

        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon)
        {
            transform.Translate(Vector3.right * (horizontalInput * speed * Time.deltaTime), Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enteringArea = other.gameObject.GetComponent<ReactableArea>();
        if (enteringArea != null)
        {
            PlayerHitReactableObject?.Invoke(enteringArea.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var enteringArea = other.gameObject.GetComponent<ReactableArea>();
        if (enteringArea != null)
        {
            PlayerHitReactableObject?.Invoke(enteringArea.gameObject);
        }
    }
}
