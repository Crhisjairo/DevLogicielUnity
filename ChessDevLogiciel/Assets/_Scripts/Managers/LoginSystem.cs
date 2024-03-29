using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class LoginSystem : MonoBehaviour
{
    #region NotToTouch

    private Coroutine waitForClearDatabaseStatusCoroutine;
    
    public GameObject loginCanvas, registerCanvas, mainMenuCanvas;
    [SerializeField] private InputField loginEmailField, loginPasswordField;
    [SerializeField] private InputField registerEmailField, registerPasswordField1, registerPasswordField2, registerUsernameField;
    [SerializeField] private Button logInBtn, registerBtn, goToRegister, goToLogin, loginGuestBtn;
    [SerializeField] private Text DBStatusText;
    private string _avatarName;

    [SerializeField] private ToggleGroup _avatarsToggleGroup;

    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Text loadingTextPercentage;
    [SerializeField] private string sceneNameOnStartGame;

    [SerializeField] private string loginAsGuestText;
    [SerializeField] private string loginSuccesText;
    [SerializeField] private Text loginStatus;
    
    
    string loginEmail = "";
    string loginPassword = "";
    
    string registerEmail = "";
    string registerPassword1 = "";
    string registerPassword2 = "";
    string registerUsername = "";

    private bool isWorking = false;
    private bool registrationCompleted = false;
    //private bool isLoggedIn = false;
    
    //Logged-In user data
    private string userName = "";
    private string userEmail = "";

    private const string RootURL = "http://10.241.58.176/";

    [SerializeField] private GameManager _gameManager;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        DBStatusText.text = "";
        loginStatus.text = "";
        
    }

    public void OnClickLogIn()
    {
        //On disable l'intéraction avec le boutton.
        logInBtn.interactable = false;
        goToRegister.interactable = false;
        loginGuestBtn.interactable = false;
        
        loginEmail = loginEmailField.text;
        loginPassword = loginPasswordField.text;

        StartCoroutine(LoginEnumerator());
    }
    
    public void OnClickRegister()
    {
        //On disable l'intéraction avec le boutton.
        registerBtn.interactable = false;
        goToLogin.interactable = false;
        
        registerEmail = registerEmailField.text;
        registerUsername = registerUsernameField.text;
        registerPassword1 = registerPasswordField1.text;
        registerPassword2 = registerPasswordField2.text;

        //On recupère le checkbox
        Toggle toggle = _avatarsToggleGroup.ActiveToggles().FirstOrDefault();
        _avatarName = toggle.GetComponentInChildren<Image>().sprite.name;

        StartCoroutine(RegisterEnumerator());
    }

    public void OnClickGuest()
    {
        //PlayerData guestPlayerData = new PlayerData();
        
        //_gameManager.SetLocalPlayerData(guestPlayerData);

        loginStatus.text = loginAsGuestText;
    }
    IEnumerator LoginEnumerator()
    {
        if (!(waitForClearDatabaseStatusCoroutine is null))
            StopCoroutine(waitForClearDatabaseStatusCoroutine);
        
        isWorking = true;
        registrationCompleted = false;

        WWWForm form = new WWWForm();
        form.AddField("email", loginEmail);
        form.AddField("password", loginPassword);

        using (UnityWebRequest www = UnityWebRequest.Post(RootURL + "login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                DBStatusText.text = www.error;
                DBStatusText.color = Color.red;
            }
            else
            {
                string responseText = www.downloadHandler.text;

                if (responseText.StartsWith("Success"))
                {
                    string[] dataChunks = responseText.Split('|');
                    //On créé les données de l'utilisateur local.

                    int i = 0;
                    foreach (var data in dataChunks)
                    {
                        Debug.Log(i + ": " + data);
                        i++;
                    }
                    
                    
                    PlayerData localPlayerData = new PlayerData(dataChunks[1], dataChunks[2], dataChunks[3],
                        dataChunks[4], int.Parse(dataChunks[5]), int.Parse(dataChunks[6]),
                        dataChunks[7]);
                    
                    _gameManager.SetLocalPlayerData(localPlayerData);
                    
                    //isLoggedIn = true;

                    ResetValues();
                    DBStatusText.text = "Connection réussi!";
                    DBStatusText.color = Color.green;

                    loginStatus.text = loginSuccesText;
                    
                    //TODO à changer
                    loginCanvas.SetActive(false);
                    mainMenuCanvas.SetActive(true);
                    //GetComponent<Canvas>().enabled = false;
                    
                    ChangerCameraTo("MenuMenuPosition");
                    
                    
                }
                else
                {
                    DBStatusText.text = responseText;
                    DBStatusText.color = Color.red;
                }
                
                
            }
            
            //On initialise la coroutine pour changer le text du status de la DB.
            waitForClearDatabaseStatusCoroutine = StartCoroutine(WaitForClearDatabaseStatus());
            
            //On habilite l'intéraction avec le boutton
            logInBtn.interactable = true;
            goToRegister.interactable = true;
            loginGuestBtn.interactable = true;
        }

        isWorking = false;
    }
    
    IEnumerator RegisterEnumerator()
    {
        if (!(waitForClearDatabaseStatusCoroutine is null))
            StopCoroutine(waitForClearDatabaseStatusCoroutine);
        
        isWorking = true;
        registrationCompleted = false;

        WWWForm form = new WWWForm();
        form.AddField("email",registerEmail);
        form.AddField("username",registerUsername);
        form.AddField("password1", registerPassword1);
        form.AddField("password2", registerPassword2);
        form.AddField("avatar", _avatarName); //Pour ajouter l'avatar

        using (UnityWebRequest www = UnityWebRequest.Post(RootURL + "register.php",form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                DBStatusText.text = www.error;
                DBStatusText.color = Color.red;
            }
            else
            {
                string reponseTexte = www.downloadHandler.text;

                if (reponseTexte.StartsWith("Success"))
                {
                    ResetValues();
                    registrationCompleted = true;
                    DBStatusText.text = "Compte crée correctement";
                    DBStatusText.color = Color.green;
                    
                    //On affiche le canvas du login
                    loginCanvas.SetActive(true);
                    registerCanvas.SetActive(false);
                }
                else
                {
                    DBStatusText.text = reponseTexte;
                    DBStatusText.color = Color.red;
                }
                
            }
            
            //On initialise la coroutine pour changer le text du status de la DB.
            waitForClearDatabaseStatusCoroutine = StartCoroutine(WaitForClearDatabaseStatus());
            
            //On habilite l'intéraction avec le boutton
            registerBtn.interactable = true;
            goToLogin.interactable = true;
        }

        isWorking = true;
    }
    

    IEnumerator LoadBoardAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingCanvas.SetActive(true);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingSlider.value = progress;
            loadingTextPercentage.text = progress * 100f + "%";
            
            yield return null;
        }

        _gameManager.OnInitGame();
        
        StartCoroutine(DestroyScene());
    }

    IEnumerator WaitForClearDatabaseStatus()
    {
        yield return new WaitForSeconds(4);
        DBStatusText.text = "";
    }
    
    //Si jamais on veut le détruire après
    IEnumerator DestroyScene()
    {
        yield return new WaitForEndOfFrame();
        
        //Destroy(this.gameObject);
    }

    /*
     * La méthode QuitGame() fait que lorsque le joueur clique sur le bouton quitter,
     * le jeu arrête et l'exécutable se ferme.
     */
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void ResetValues()
    {
        DBStatusText.text = "";
        loginEmail = "";
        loginPassword = "";
        registerEmail = "";
        registerPassword1 = "";
        registerPassword2 = "";
        registerUsername = "";
    }
    #endregion

    [SerializeField] GameObject Authentification;
    [SerializeField] private Animator _drivenCamAnimator;

    public void LoadBoard()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadBoardAsync(sceneNameOnStartGame));
    }
    
    public void ChangerCameraTo(String state)
    {
        _drivenCamAnimator.Play(state);
    }


}
