using UnityEngine;

public class PotionScript : MonoBehaviour
{
    [SerializeField] private float healthValue;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Health>().Heal(healthValue);
            gameObject.SetActive(false);
        }   
    }    
}
