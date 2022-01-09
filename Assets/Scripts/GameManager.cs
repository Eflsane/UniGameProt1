using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Timers;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool isConstructMode = false;
    public static bool isPlacingMode = false;
    public static bool isConstructElemSelectionMode = false;
    public static bool isSettingsMode = false;
    public static bool isShopMode = false;
    public static bool isColoringGrid = false;
    public static bool canPlace = false;
    public bool canClick = true;

    public ConstructElemController[] constructElemsPrefabs;
    public ConstructElemController constructElemParamsKeeperPrefab;
    public ConstructElemController[] constructElemsParamsKeepers;
    public ConstructElemUIController constructElemUIPrefab;


    public static ConstructElemController currentElemSelected;
    public static GridElemController currentGridElemSelected;
    public static GridElemController gridElemCollide;

    public GameObject mainPanel;
    public GameObject constructPanel;
    public GameObject constructPanelElemContent;
    public GameObject placingItemPanel;
    public GameObject placeButton;
    public GameObject elemSelectedPanel;
    public TextMeshProUGUI selectedElemText;
    public GameObject elemUpgradeButton;
    public GameObject shopPanel;
    public Button booster2xButton;
    public Button booster3xButton;
    public TextMeshProUGUI boosterTime;
    public GameObject settingsPanel;
    public TextMeshProUGUI userNameText;
    public Button upgradeGridButton;
    public Slider upgradeGridSlider;
    public TextMeshProUGUI upgradeGridText;


    public Slider soundSlider;
    private AudioSource audioSource;

    public TMPro.TextMeshProUGUI scoreCounterText;
    public static float scoreCounter;
    public float startScore;
    public static float income;
    public float startIncome;

    public static float speed;
    public TimeSpan boosterTimeLeft;
    public DateTime boosterStarted;
    private Timer boosterTimer;

    public TextMeshProUGUI premMoneyText;
    public int premMoney;

    public int gridLevel;
    public float multiplier;
    public float scoreToUpgradeGridBasic;
    public float scoreToUpgradeGridTotal;

    public string userName;
    private UserParams userParams;
    private string soundVolumePref = "STroiKaGame_SoundVolume";
    private string quitTimePref = "STroiKaGame_QuitTime";
    private string userNamePref = "STroiKaGame_UserName";
    private string userParamsPref = "STroiKaGame_UserParams";
    private string elemsSavedPref = "STroiKaGame_ElemsSave";

    public string jSonGrid;
    private GridElemController[,] __gridElems;
    public ConstructElemSaveCompress compress;
    public List<ConstructElemSaveCompress> savedElems;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);

        LoadConstructElemsPrefabs();

        audioSource = GetComponent<AudioSource>();



        /*for(int i = 0; i < constructElem.Length; i++)
        {
            Debug.Log(i + constructElem[i].name);

        }*/

        CreateConstructionElemsUI();

        scoreCounter = startScore;
        income = startIncome;
        scoreToUpgradeGridTotal = scoreToUpgradeGridBasic;
        speed = 1.0f;
        scoreCounterText.text = "$" + scoreCounter;

        gridLevel = 1;


        LoadPlayerPrefs();
        userNameText.text = userName;

        UpgradeGridSet();
        UpdateConstructElemsUI(true);
    }

    // Update is called once per frame
    void Update()
    {
        scoreCounterText.text = "$" + string.Format("{0:0.0}", scoreCounter);
        scoreCounter += income * speed * Time.deltaTime;

        ActivateConstructUIElems();

        ActivatePlacingOKButt();

        ActivateElemUpgradeButton();

        UpgradeGridCheck();

        ActivateBoosterButtons();
    }

    public void LoadConstructElemsPrefabs()
    {
        constructElemsPrefabs = Resources.LoadAll<ConstructElemController>("ConstructElemsPrefabs/");

        constructElemsParamsKeepers = new ConstructElemController[constructElemsPrefabs.Length];
        for (int i = 0; i < constructElemsPrefabs.Length; i++)
        {
            ConstructElemController elem = Instantiate(constructElemParamsKeeperPrefab);
            elem.elemName = constructElemsPrefabs[i].elemName;
            elem.price = constructElemsPrefabs[i].price;
            elem.upgradePrice = constructElemsPrefabs[i].upgradePrice;
            elem.incomeAscending = constructElemsPrefabs[i].incomeAscending;
            elem.multiply = constructElemsPrefabs[i].multiply;
            elem.level = 1;
            elem.count = 0;

            elem.totalPrice = elem.price;
            elem.totalIncomeAscending = elem.incomeAscending;
            elem.totalUpgradePrice = elem.upgradePrice;


            constructElemsParamsKeepers[i] = elem;
        }
    }

    private void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(soundVolumePref))
            ChangeSoundVolume(PlayerPrefs.GetFloat(soundVolumePref), true);

        if (PlayerPrefs.HasKey(userNamePref))
            userName = PlayerPrefs.GetString(userNamePref);


        if (PlayerPrefs.HasKey(userParamsPref))
        {
            userParams = JsonUtility.FromJson<UserParams>(PlayerPrefs.GetString(userParamsPref));


            UpgradeGrid(userParams.gridLevel);

            income = userParams.income;

            premMoney = userParams.premMoney;
            premMoneyText.text = premMoney.ToString() + "G";


            if (PlayerPrefs.HasKey(elemsSavedPref))
            {
                GrideController.instance.LoadGridFromPrefs(PlayerPrefs.GetString(elemsSavedPref));
            }
            //уровни при подсчете очков продумай
            //как сохранять сетку и объекты в ней?
            //короче, идея следующая: создай шаблон для сохранение клеток с их индексами и с названиями объекта лежащего. удачи чело! 
            //GrideController.instance.CreateGridWithParams(GrideController.instance.startGridSize + gridLevel);

            DateTime quitTime = new DateTime(userParams.gameQuitTimeTicks);
            TimeSpan timeFromLastPlay = DateTime.Now - quitTime;

            TimeSpan speedBoosterWorkingTimeLeft = new TimeSpan(0, 0, (int)userParams.speedBoosterWorkingTimeLeftSeconds);

            if (userParams.speed == 1 || userParams.speedBoosterWorkingTimeLeftSeconds <= 0)
            {
                speed = 1;
                scoreCounter = (float)(userParams.score + userParams.income * timeFromLastPlay.TotalSeconds);
                return;
            }

            if (timeFromLastPlay > speedBoosterWorkingTimeLeft)
            {
                speed = 1;
                scoreCounter = (float)(userParams.score + userParams.income * userParams.speed * speedBoosterWorkingTimeLeft.TotalSeconds + userParams.income * (timeFromLastPlay - speedBoosterWorkingTimeLeft).TotalSeconds);
                return;
            }

            speed = userParams.speed;
            scoreCounter = (float)(userParams.score + userParams.income * userParams.speed * timeFromLastPlay.TotalSeconds);
            boosterTimeLeft = speedBoosterWorkingTimeLeft;

            BuyBooster((int)speed, speedBoosterWorkingTimeLeft - timeFromLastPlay);
        }
    }

    public void SwitchConstructionMode(bool state)
    {
        isColoringGrid = true;

        /*if (isConstructMode)
        {
            isConstructMode = false;
            mainPanel.SetActive(true);
            constructPanel.SetActive(false);
        }

        else
        {
            isConstructMode = true;
            mainPanel.SetActive(false);
            constructPanel.SetActive(true);
        }*/

        isConstructMode = state;
        mainPanel.SetActive(!state);
        constructPanel.SetActive(state);

        if (state)
            GrideController.instance.ColorizeAllGrid(GrideController.instance.buildingOKColor);
        else
            GrideController.instance.ColorizeAllGrid(GrideController.instance.baseColor);
    }

    public void SwitchPlacingMode(bool state)
    {
        isColoringGrid = true;
        /*if (isConstructMode && placingItemPanel.activeInHierarchy)
        {
            constructPanel.SetActive(true);
            placingItemPanel.SetActive(false);

        }

        else
        {
            constructPanel.SetActive(false);
            placingItemPanel.SetActive(true);
        }*/

        isPlacingMode = state;
        placingItemPanel.SetActive(state);

        if (state)
            GrideController.instance.ColorizeAllGrid(GrideController.instance.buildingOKColor);
        else
            GrideController.instance.ColorizeAllGrid(GrideController.instance.baseColor);

        if (isConstructElemSelectionMode)
        {
            elemSelectedPanel.SetActive(!state);
            return;
        }


        constructPanel.SetActive(!state);
    }

    public void CancelPlacing()
    {
        if (!isConstructElemSelectionMode)
        {
            if (currentElemSelected.Equals(currentGridElemSelected.installedObject))
                currentGridElemSelected.ChangeColor(GrideController.instance.buildingOKColor);
            Destroy(currentElemSelected.gameObject);
            currentGridElemSelected = null;
            SwitchPlacingMode(false);
            return;
        }


        currentGridElemSelected = currentElemSelected.gridElemPlaced;
        currentGridElemSelected.TryPlaceItem(false);

        SwitchPlacingMode(false);
    }

    public void FinishPlacingItemOnGrid()
    {
        if (!isConstructElemSelectionMode)
        {
            ConstructElemController elem = constructElemsParamsKeepers.Where<ConstructElemController>(x => x.elemName == currentElemSelected.elemName).ToArray()[0];
            income += currentElemSelected.totalIncomeAscending;
            scoreCounter -= elem.totalPrice;
            currentElemSelected.gridElemPlaced = currentGridElemSelected;


            elem.PriceUpgrade();
            UpdateConstructElemsUI();

            currentElemSelected = null;
            currentGridElemSelected = null;
            SwitchPlacingMode(false);
            return;
        }


        currentElemSelected.gridElemPlaced = currentGridElemSelected;
        SwitchPlacingMode(false);
    }

    public void UpdateConstructElemsUI()
    {
        ConstructElemController elem = constructElemsParamsKeepers.Where<ConstructElemController>(x => x.elemName == currentElemSelected.elemName).ToArray()[0];
        ConstructElemUIController elemUI = constructPanelElemContent.GetComponentsInChildren<ConstructElemUIController>().Where<ConstructElemUIController>(x => x.elemName.text == currentElemSelected.elemName).ToArray()[0];

        elemUI.price.text = "$" + elem.totalPrice.ToString();
    }

    public void UpdateConstructElemsUI(bool isLoading)
    {
        if(isLoading)
        {
            for(int i = 0; i < constructElemsParamsKeepers.Length; i++)
            {
                constructElemsParamsKeepers[i].PriceUpgrade(constructElemsParamsKeepers[i].count);
            }
        }
        for (int i = 0; i < constructElemsParamsKeepers.Length; i++)
        {
            constructPanelElemContent.GetComponentsInChildren<ConstructElemUIController>()[i].price.text = "$" + constructElemsParamsKeepers[i].totalPrice.ToString();
        }
    }

    //Test method
    public void CreateConstructElem()
    {
        SwitchPlacingMode(true);

        //constructElems[0].transform.localScale = new Vector3(GrideController.firstElem.transform.localScale.x * 5, constructElems[0].transform.localScale.y, GrideController.firstElem.transform.localScale.z * 5);
        currentGridElemSelected = GrideController.firstElem;
        currentElemSelected = Instantiate(constructElemsPrefabs[0], new Vector3(currentGridElemSelected.transform.position.x, 0.5f, currentGridElemSelected.transform.position.z), currentGridElemSelected.transform.rotation);
        currentElemSelected.transform.localScale = new Vector3(GrideController.firstElem.transform.localScale.x * 5, constructElemsPrefabs[0].transform.localScale.y, GrideController.firstElem.transform.localScale.z * 5);
        currentGridElemSelected.TryPlaceItem(false);
    }

    public void CreateConstructElem(int elemIndex)
    {
        SwitchPlacingMode(true);

        Debug.Log(elemIndex);
        //constructElems[elemIndex].transform.localScale = new Vector3(GrideController.firstElem.transform.localScale.x * 5, constructElems[elemIndex].transform.localScale.y, GrideController.firstElem.transform.localScale.z * 5);
        currentGridElemSelected = GrideController.firstElem;
        currentElemSelected = Instantiate(constructElemsPrefabs[elemIndex], new Vector3(currentGridElemSelected.transform.position.x, 0.5f, currentGridElemSelected.transform.position.z), currentGridElemSelected.transform.rotation);
        currentElemSelected.level = 1;
        currentElemSelected.transform.localScale = new Vector3(GrideController.firstElem.transform.localScale.x * 5, constructElemsPrefabs[elemIndex].transform.localScale.y, GrideController.firstElem.transform.localScale.z * 5);
        currentGridElemSelected.TryPlaceItem(false);
    }

    public void CreateConstructionElemsUI()
    {
        for (int i = 0; i < constructElemsParamsKeepers.Length; i++)
        {
            ConstructElemUIController cUI = Instantiate(constructElemUIPrefab, constructElemUIPrefab.transform);
            cUI.elemIndex = i;

            if (constructElemsPrefabs[i].elemImage != null)

            if(constructElemsPrefabs[i].elemImage != null)
            {
                cUI.image = constructElemsPrefabs[i].elemImage;
                cUI.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = cUI.image;
            }

            cUI.elemName.text = constructElemsParamsKeepers[i].elemName;
            cUI.name = constructElemsParamsKeepers[i].elemName;
            cUI.price.text = constructElemsParamsKeepers[i].price.ToString();

            Button butt = cUI.GetComponent<Button>();
            butt.onClick.AddListener(delegate { CreateConstructElem(cUI.elemIndex); });

            cUI.GetComponent<RectTransform>().SetParent(constructPanel.transform, false);
            cUI.transform.SetParent(constructPanelElemContent.transform, false);
        }
    }

    public void ActivateConstructUIElems()
    {
        if (isConstructMode)
        {
            for (int i = 0; i < constructPanelElemContent.transform.childCount; i++)
            {
                GameObject ui = constructPanelElemContent.transform.GetChild(i).gameObject;
                if (ui.GetComponent<ConstructElemUIController>() != null)
                {
                    if (scoreCounter < constructElemsParamsKeepers[i].totalPrice)
                    {
                        ui.GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        ui.GetComponent<Button>().interactable = true;
                    }

                    /*if(counter < constructElems.Length - 1)
                        counter++;*/
                }
            }
        }
    }

    public void ActivatePlacingOKButt()
    {
        if (isPlacingMode)
        {
            if (currentElemSelected != null && currentGridElemSelected != null)
            {
                if (!currentElemSelected.Equals(currentGridElemSelected.installedObject))
                {
                    placeButton.GetComponent<Button>().interactable = false;
                    //Debug.Log("false");
                }
                else
                {
                    placeButton.GetComponent<Button>().interactable = true;
                    //Debug.Log("true");
                }
            }
        }
    }

    public void SwitchElemSelectingMode(bool state)
    {
        isConstructElemSelectionMode = state;
        mainPanel.SetActive(!state);
        elemSelectedPanel.SetActive(state);

        if (state)
            selectedElemText.text = currentElemSelected.elemName + ": " + currentElemSelected.level + "\n" + "Upgrade Price: " + currentElemSelected.upgradePrice;
    }

    public void DeleteSelectedObject()
    {
        income -= currentElemSelected.totalIncomeAscending;

        ConstructElemController elem = constructElemsParamsKeepers.Where<ConstructElemController>(x => x.elemName == currentElemSelected.elemName).ToArray()[0];
        elem.count--;
        currentElemSelected.PriceUpgrade(elem.count);
        scoreCounter += elem.totalPrice;
        scoreCounterText.text = "$" + string.Format("{0:0.0}", scoreCounter);
        UpdateConstructElemsUI(false);

        Destroy(currentElemSelected.gameObject);
        currentGridElemSelected.installedObject = null;

        currentGridElemSelected.ChangeColor(GrideController.instance.baseColor);
        SwitchElemSelectingMode(false);
    }

    public void ActivateElemUpgradeButton()
    {
        if (isConstructElemSelectionMode && currentElemSelected != null)
        {
            if (scoreCounter < currentElemSelected.totalUpgradePrice)
                elemUpgradeButton.GetComponent<Button>().interactable = false;
            else
                elemUpgradeButton.GetComponent<Button>().interactable = true;

        }
    }

    public void UpgradeSelection()
    {
        income -= currentElemSelected.totalIncomeAscending;
        currentElemSelected.UpgradeChange();
        income += currentElemSelected.totalIncomeAscending;

        selectedElemText.text = currentElemSelected.elemName + ": " + currentElemSelected.level + "\n" + "Upgrade Price: " + currentElemSelected.totalUpgradePrice;
    }

    public void UpgradeGrid()
    {
        int newGridSize = GrideController.instance.GridSize + 1;
        GrideController.instance.DestroyGrid();
        GrideController.instance.CreateGridWithParams(newGridSize);

        //need to change some score level multiplyer(Maybe. It's just an first idea)
        gridLevel++;

        scoreToUpgradeGridTotal = scoreToUpgradeGridBasic * Mathf.Pow(multiplier, gridLevel - 1);
        UpgradeGridSet();

        income = 0;
        scoreCounter = startScore * Mathf.Pow(multiplier, gridLevel - 1);

        premMoney += 10;
        premMoneyText.text = premMoney.ToString();

        for (int i = 0; i < constructElemsParamsKeepers.Length; i++)
        {
            constructElemsParamsKeepers[i].PriceUpgrade(0);
        }


        UpdateConstructElemsUI(true);
    }

    //for loading grid
    public void UpgradeGrid(int newLevel)
    {
        if (newLevel > 1)
        {
            int newGridSize = GrideController.instance.GridSize + newLevel - 1;
            GrideController.instance.DestroyGrid();
            GrideController.instance.CreateGridWithParams(newGridSize);

            //need to change some score level multiplyer(Maybe. It's just an first idea)
            gridLevel = newLevel;

            scoreToUpgradeGridTotal = scoreToUpgradeGridBasic * Mathf.Pow(multiplier, gridLevel - 1);
            UpgradeGridSet();
            premMoneyText.text = premMoney.ToString() + "G";
        }


        /*income = 0;
        scoreCounter = startScore * Mathf.Pow(multiplier, gridLevel - 1);

        premMoney += 15;*/
    }

    public void UpgradeGridCheck()
    {
        if (upgradeGridSlider.value < upgradeGridSlider.maxValue)
        {
            upgradeGridButton.interactable = false;
            upgradeGridButton.gameObject.SetActive(false);
            upgradeGridSlider.gameObject.SetActive(true);
        }

        else
        {
            upgradeGridButton.interactable = true;
            upgradeGridButton.gameObject.SetActive(true);
            upgradeGridSlider.gameObject.SetActive(false);
        }


        upgradeGridSlider.value = scoreCounter;
        upgradeGridText.text = string.Format("{0:0.0} / {1:0.0}", scoreCounter, upgradeGridSlider.maxValue);
    }

    public void UpgradeGridSet()
    {
        upgradeGridSlider.maxValue = scoreToUpgradeGridTotal;
        upgradeGridSlider.value = 0;
    }

    public void SwitchShopMode(bool state)
    {
        shopPanel.SetActive(state);
        isShopMode = state;
    }

    public void BuyBooster(int boost)
    {
        if (boost == 2)
            premMoney -= 10;
        if (boost == 3)
            premMoney -= 40;
        speed = boost;

        premMoneyText.text = premMoney.ToString() + "G";


        boosterTimeLeft = new TimeSpan(0, 5, 0);
        boosterTimer = new Timer();
        boosterTimer.Interval = boosterTimeLeft.TotalMilliseconds;
        boosterTimer.Elapsed += BoosterTimerUp;
        boosterStarted = DateTime.Now;

        boosterTime.text = "x" + speed + " " + boosterTimeLeft.TotalSeconds.ToString();
        CheckCoroutine();
    }

    public void BuyBooster(int boost, TimeSpan leftFromLastGame)
    {
        speed = boost;

        premMoneyText.text = premMoney.ToString() + "G";

        premMoneyText.text = premMoney.ToString();


        boosterTimeLeft = new TimeSpan(leftFromLastGame.Ticks);
        boosterTimer = new Timer();
        boosterTimer.Interval = boosterTimeLeft.TotalMilliseconds;
        boosterStarted = DateTime.Now;

        boosterTime.text = "x" + speed + " " + boosterTimeLeft.TotalSeconds.ToString();
        CheckCoroutine();
    }

    public void BoosterTimerUp(object sender, ElapsedEventArgs e)
    {
        speed = 1;
        boosterTimeLeft = new TimeSpan(0);
        boosterStarted = new DateTime(0);
        boosterTimer.Stop();
        boosterTimer = null;

        StopAllCoroutines();

        boosterTime.gameObject.SetActive(false);
    }

    IEnumerator BoosterCoroutine()
    {
        while (true)
        {

            yield return new WaitForSeconds(1);
            boosterTimeLeft -= new TimeSpan(0, 0, 1);
            boosterTime.text = "x" + speed + " " + string.Format("{0:0}", boosterTimeLeft.TotalSeconds);
            //Debug.Log(boosterTimeLeft.TotalSeconds);
        }
    }

    public void CheckCoroutine()
    {
        if (speed > 1)
        {
            boosterTime.gameObject.SetActive(true);
            StartCoroutine("BoosterCoroutine");
        }
    }

    public void ActivateBoosterButtons()
    {
        if (speed == 1)
        {
            if (premMoney < 10)
                booster2xButton.interactable = false;
            else
                booster2xButton.interactable = true;

            if (premMoney < 40)
                booster3xButton.interactable = false;
            else
                booster3xButton.interactable = true;
        }
        else
        {
            booster2xButton.interactable = false;
            booster3xButton.interactable = false;
        }

    }

    public void SwitchSettingsMode(bool state)
    {
        settingsPanel.SetActive(state);
        isSettingsMode = state;
    }

    public void ChangeSoundVolume(float value)
    {
        //Debug.Log(value.ToString());
        audioSource.volume = value;
        PlayerPrefs.SetFloat(soundVolumePref, value);

        //userParams.soundVolume = value;
    }
    public void ChangeSoundVolume(float value, bool isLoading)
    {
        //Debug.Log(value.ToString());
        audioSource.volume = value;
        if (isLoading)
            soundSlider.value = value;
        //userParams.soundVolume = value;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartOver()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    public void setUser()
    {

    }

    /*private IEnumerator SendRequest() 
    {
        UnityWebRequest
    }*/

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR || UNITY_EDITOR_64
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        PlayerPrefs.SetFloat(quitTimePref, Time.time);
        PlayerPrefs.SetFloat(soundVolumePref, audioSource.volume);

        ReadyToSaveUserParams();
        string s = JsonUtility.ToJson(userParams);
        PlayerPrefs.SetString(userParamsPref, s);

        PlayerPrefs.SetString(elemsSavedPref, GrideController.instance.SaveGridToPrefs());

        PlayerPrefs.Save();
        //PlayerPrefs.DeleteAll();
    }

    public void ReadyToSaveUserParams()
    {
        userParams = new UserParams();
        userParams.userName = userName;
        userParams.score = scoreCounter;
        userParams.income = income;
        userParams.premMoney = premMoney;
        userParams.gameQuitTimeTicks = DateTime.Now.Ticks;
        userParams.speed = (int)speed;
        userParams.speedBoosterWorkingTimeLeftSeconds = boosterTimeLeft.TotalSeconds;
        userParams.gridLevel = gridLevel;

        //корутина сука не забудь
    }

    private void OnApplicationPause(bool pause)
    {
        Application.Quit();
    }
}