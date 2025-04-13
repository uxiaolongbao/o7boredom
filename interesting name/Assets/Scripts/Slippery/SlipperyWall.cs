using UnityEngine;

public class SlipperyBlock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var move = other.GetComponent<PlayerMove>();
            if (move != null)
                move.SetSlipping(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var move = other.GetComponent<PlayerMove>();
            if (move != null)
                move.SetSlipping(false);
        }
    }
}
