using UnityEngine;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            //Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
        }
        else
        {
            //Debug.Log("Raycast hit nothing.");
        }
    }
}
