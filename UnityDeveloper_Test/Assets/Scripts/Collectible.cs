using UnityEngine;

public class Collectible : MonoBehaviour
{
    GameManager gm;

    void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gm != null)
                gm.CollectCube(gameObject); // pass this cube

            Destroy(gameObject);
        }
    }
}