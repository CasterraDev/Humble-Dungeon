using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    public bool menuStopped;
    public GameObject InteractableTxt, chatGO;
    public ChatLogic chatLogic;
    public DisplayInventory displayInv;
    // Start is called before the first frame update
    void Start()
    {
        chatLogic = GetComponent<ChatLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        InteractableTxtShown();

        if (!chatLogic.inputOpened)
        {
            displayInv.invEnabled = true;
            //chatGO.SetActive(false);
        }
        else
        {
            displayInv.invEnabled = false;
            //chatGO.SetActive(true);
        }
    }

    public void SendChatMessage(string text)
    {
        chatLogic.SendChatMessage(text);
    }

    public void ShowInteractableText(string _text, int _font)
    {
        InteractableTxt.GetComponent<TextMeshProUGUI>().text = _text.ToString();
        InteractableTxt.GetComponent<TextMeshProUGUI>().fontSize = _font;
    }

    public void ShowInteractableText(string _text)
    {
        InteractableTxt.GetComponent<TextMeshProUGUI>().text = _text.ToString();
        InteractableTxt.GetComponent<TextMeshProUGUI>().enableAutoSizing = true;
    }

    public void InteractableTxtShown()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0)), out hit, 10f))
        {
            string text = "";

            if (hit.collider.CompareTag("Item"))
            {
                text = "Press F to Pickup " + hit.collider.GetComponent<GroundItem>().item.name;
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                text = "";
            }

            ShowInteractableText(text);
        }
        else
        {
            ShowInteractableText("");
        }
    }
}
