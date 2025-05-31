using UnityEngine;

public class HideObjectZone : MonoBehaviour
{
    public GameObject[] targetObjects; // Listează toate obiectele în Inspector

    void Start()
    {
        foreach (GameObject obj in targetObjects)
        {
            Renderer r = obj.GetComponent<Renderer>();
            Collider c = obj.GetComponent<Collider>();
            if (r != null) r.enabled = true;
            if (c != null) c.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in targetObjects)
            {
                Renderer r = obj.GetComponent<Renderer>();
                Collider c = obj.GetComponent<Collider>();
                if (r != null) r.enabled = false;
                if (c != null) c.enabled = false;
            }
        }
    }
}
