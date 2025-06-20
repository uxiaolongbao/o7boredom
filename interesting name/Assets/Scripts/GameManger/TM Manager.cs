using UnityEngine;
using TMPro; 
public class NewMonoBehaviourScript : MonoBehaviour
{
    public TextMeshProUGUI myText; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        myText.text = "hi"; 
    }
}
