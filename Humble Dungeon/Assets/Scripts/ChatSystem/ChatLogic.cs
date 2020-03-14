using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatLogic : MonoBehaviour
{
    List<Message> messageList = new List<Message>();
    public int maxMessages = 25;
    public GameObject textObject, chatPanel;
    public TMP_InputField chatBox;
    public bool inputOpened = false;
    private GameController GC;

    // Start is called before the first frame update
    void Start()
    {
        GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendChatMessage(chatBox.text);
                chatBox.text = "";
                inputOpened = false;
                chatBox.DeactivateInputField();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputOpened = true;
                chatBox.ActivateInputField();
            }
        }

        if (inputOpened)
        {
            GC.menuStopped = true;
            chatBox.gameObject.SetActive(true);
        }
        else
        {
            chatBox.gameObject.SetActive(false);
        }
    }

    public void SendChatMessage(string text)
    {
        if (text.Substring(0, 1) == "/")
        {
            ChatCommands(text);
            return;
        }

        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObj.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObj = newText.GetComponent<Text>();
        newMessage.textObj.text = newMessage.text;

        messageList.Add(newMessage);
    }

    void ChatCommands(string tempStr)
    {
        string[] strArr = tempStr.Split(' ');

        switch (strArr[0].ToLower())
        {
            case "/hi":
                Debug.Log("Hi");
                break;
        }
        Debug.Log(strArr[0].ToLower() + "lkjlk");
    }
}

public class Message
{
    public string text;
    public Text textObj;
}
