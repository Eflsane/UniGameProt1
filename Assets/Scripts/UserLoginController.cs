using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserLoginController : MonoBehaviour
{
    public TMP_InputField loginField;
    private string userNamePref = "STroiKaGame_UserName";
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey(userNamePref))
            SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateUserAndStart()
    {
        if(loginField.text != "")
        {
            PlayerPrefs.SetString(userNamePref, loginField.text);
            SceneManager.LoadScene(1);
        } 
    }
}
