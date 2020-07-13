using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunningText : MonoBehaviour
{
    public Text txtMessage;
    private bool isRunning;
    private int counter;
    private float width;
    private float speed = 250f;

    private List<string> messages = new List<string> ();

    public void AddQueue ( string message )
    {
        messages.Add (message);
        CheckStatus ();
    }

    private void CheckStatus ()
    {
        if (isRunning)
            return;

        Run ();
    }

    private void Run ()
    {
        txtMessage.text = messages[0];
        Debug.LogError (txtMessage.text);
        width = txtMessage.preferredWidth;
        if (width < Screen.width)
            width = Screen.width;
        OutOfScreenStart ();
        gameObject.SetActive (true);
        isRunning = true;
        counter = 0;
    }

    private void Update ()
    {
        if (isRunning)
        {
            if (txtMessage.transform.localPosition.x > width * (-1))
            {
                txtMessage.transform.localPosition = new Vector3 (txtMessage.transform.localPosition.x - Time.deltaTime * speed, 0f);
            }
            else
            {
                counter++;
                OutOfScreenStart ();
                if (counter >= 3)
                {
                    OutOfScreenEnd ();
                    messages.RemoveAt (0);
                    Debug.LogError (messages.Count);
                    if (messages.Count > 0)
                        Run ();
                    else
                    {
                        isRunning = false;
                        gameObject.SetActive (false);
                    }
                }
            }
        }
    }

    private void OutOfScreenEnd ()
    {
        txtMessage.transform.localPosition = new Vector3 (width * (-1), 0f);
    }

    private void OutOfScreenStart ()
    {
        txtMessage.transform.localPosition = new Vector3 (width, 0f);
    }
}
