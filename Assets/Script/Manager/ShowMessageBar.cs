using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShowMessageBar : MonoBehaviour
{

    [SerializeField]
    private Text ShowMessage;
    Coroutine OpenM;
    private float CurrentTime;


    public void SetMessage(string Message, float wait = 0)
    {
        CurrentTime = wait;
        ActionMesaaage(Message);
    }

    private void ActionMesaaage(string Message)
    {
        if (Message == "(-1)")
            return;
        if (OpenM != null)
        {
            StopCoroutine(OpenM);
        }
        ShowMessage.text = Message;
        transform.localScale = new Vector3(0, 0, 0);
        OpenM = StartCoroutine("Open");
    }

    IEnumerator Open()
    {
        while (transform.localScale.y <= 1)
        {
            transform.localScale += new Vector3(0, 0.2f, 0);
            yield return 0;
        }
        transform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(1.5f + CurrentTime);

        while (transform.localScale.y > 0)
        {
            transform.localScale -= new Vector3(0, 0.2f, 0);
            yield return 0;
        }
        transform.localScale = new Vector3(0, 0, 0);
        ShowMessage.text = string.Empty;
    }
}
