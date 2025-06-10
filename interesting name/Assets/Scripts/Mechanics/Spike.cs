using UnityEngine;

public class Spike : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Hopefully implement a death animation
            Debug.Log("Player death!");

            //Destroy plyer
            Destroy(other.gameObject);

            //Hopefully restart level too
            //Code code code
        }
    }
}
