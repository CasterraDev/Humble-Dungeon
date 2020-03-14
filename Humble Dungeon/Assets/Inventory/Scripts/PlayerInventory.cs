using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public GameController GC;

    private void Start()
    {
        GC = GameObject.Find("GameController").GetComponent<GameController>();
    }
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0)), out hit))
        {
            if (hit.collider.CompareTag("Item"))
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickUpSystem(hit);
                }
            }

        }
    }
    private void PickUpSystem(RaycastHit hit)
    {
        var gItem = hit.transform.gameObject.GetComponent<GroundItem>();
        inventory.AddItem(new Item(gItem.item), gItem.amount);
        Destroy(hit.transform.gameObject);
    }

    private void OnApplicationQuit()
    {
        inventory.container.items = new InventorySlot[inventory.maxItems];
    }
}
