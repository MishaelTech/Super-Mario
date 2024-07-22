using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Transform connection;
    public KeyCode emterKeyCode = KeyCode.S;
    public Vector3 enterDirection = Vector3.down;
    public Vector3 exitDirection = Vector3.up;

    private void OnTriggerStay(Collider other)
    {
        if (connection != null && other.CompareTag("Player"))
        {
            if (Input.GetKey(emterKeyCode))
            {
                StartCoroutine(Enter(other.transform));
            }
        }
    }

    private IEnumerator Enter(Transform player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;

        Vector3 enterPosition = transform.position + enterDirection;
        Vector3 enteredScale = Vector3.one * 0.5f;



        yield return Move(player, enterPosition, enteredScale);
    }

    private IEnumerator Move(Transform player, Vector3 endPosition, Vector3 endScale)
    {
        float elasped = 0f;
        float duration = 1f;

        Vector3 startPosition = player.position;
        Vector3 startScale = player.localScale;

        while(elasped < duration)
        {
            float t = elasped / duration;

            player.position = Vector3.Lerp(startPosition, endPosition, t);  
            player.localScale = Vector3.Lerp(startScale, endScale, t);

            elasped += Time.deltaTime;

            yield return null;
        }

        player.position = endPosition;
        player.localScale = endScale;
    }
}
