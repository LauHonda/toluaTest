using UnityEngine;
public class ButtonNormalEvent : ButtonEventBase {

    [SerializeField]
    private GameObject ShowObj, HideObj, hideSecondaryPanel, showFirstPanel;
    [TextArea(2,5)]
    public string Message;
    [SerializeField]
    private HttpModel Model;

    public override void Start()
    {
        base.Start();
        if (Model != null)
            ActionEvent += SendModel;
        ActionEvent += OnClick;
    }

    void SendModel()
    {
        Model.Get();
    }

    void OnClick()
    {
        if (ShowObj != null)
            WindowsManager.GetWindowsManager.OpenWindow(ShowObj.transform);
        if (HideObj != null)
            WindowsManager.GetWindowsManager.CloseWindow(HideObj.transform);
        if (Message != string.Empty)
            MessageManager.GetMessageManager.WindowShowMessage(Message);
        if (hideSecondaryPanel != null && showFirstPanel != null)
        {
            hideSecondaryPanel.SetActive(false);
            showFirstPanel.SetActive(true);
        }
    }

    public void SetObjActive(GameObject Obj,bool State)
    {
        if(Obj!=null)
        Obj.SetActive(State);
    }

    public void PlayAnimationGo(Animator GetAnimtor)
    {
        GetAnimtor.SetBool("Go",true);
    }

    public void PlayAnimationBack(Animator GetAnimtor)
    {
        GetAnimtor.SetBool("Back", true);
    }
}
