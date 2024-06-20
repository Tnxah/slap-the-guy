using UnityEngine;

public class BorderPortals : MonoBehaviour
{        
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ThrownItem throwable))
        {
            if (throwable.teleportations <= 0)
                Destroy(collision.gameObject);

            throwable.teleportations--;
            throwable.transform.position = new Vector3(-throwable.transform.position.x, throwable.transform.position.y, throwable.transform.position.z); 
        }
    }
}
