using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject modelButtonsParent;
    [SerializeField] GameObject areaButtonsParent;

    [SerializeField] Button controlsButton;
    [SerializeField] Button controlsExitButton;
    [SerializeField] Button showAllButton;
    [SerializeField] Button hideAllButton;
    [SerializeField] Button resetButton;
    [SerializeField] Button areasButton;
    [SerializeField] Button backButton;
    [SerializeField] Button showButton;
    [SerializeField] Button findButton;

    [SerializeField] Image controlsPanel;
    [SerializeField] Image areasPanel;

    [SerializeField] TextMeshProUGUI panelTitleText;
    [SerializeField] TextMeshProUGUI panelListText;
    [SerializeField] TextMeshProUGUI areaTitleText;
    [SerializeField] TextMeshProUGUI areaDescriptionText;

    List<GameObject> models = new List<GameObject>();
    Button[] modelButtons;

    Quaternion startRotation;

    Vector3 startPosition;
    Vector3 startCameraPosition;
    float startOrthographicSize;

    Color32 activecolour = new Color32 (50, 50, 50, 255);
    Color32 inactivecolour = new Color32 (200, 200, 200, 255);

    void Start()
    {
        foreach (Transform child in transform)
        {
            models.Add(child.gameObject);
        }

        modelButtons = modelButtonsParent.GetComponentsInChildren<Button>();

        foreach (Button button in modelButtons)
        {
            button.onClick.AddListener(() => { ToggleModel(button); });
        }

        controlsExitButton.GetComponent<Button>().onClick.AddListener(() =>
            { ActivateModels(true); });
        showAllButton.GetComponent<Button>().onClick.AddListener(() =>
            { ActivateModels(true); });
        hideAllButton.GetComponent<Button>().onClick.AddListener(() =>
            { ActivateModels(false); });

        backButton.GetComponent<Button>().onClick.AddListener(() =>
            { ResetAreasPanel(true); });

        controlsExitButton.GetComponent<Button>().onClick.AddListener(ResetModels);
        resetButton.GetComponent<Button>().onClick.AddListener(ResetModels);

        showButton.onClick.AddListener(() => { ActivateShowPanel(true); });
        findButton.onClick.AddListener(() => { ActivateShowPanel(false); });

        startPosition = transform.position;
        startRotation = transform.rotation;

        startCameraPosition = Camera.main.transform.position;
        startOrthographicSize = Camera.main.orthographicSize;
    }

    void ToggleModel(Button clicked)
    {
        foreach (GameObject model in models)
        {
            if (model.name == clicked.name.Split()[0])
            {
                model.SetActive(!model.activeSelf);

                clicked.GetComponentInChildren<TextMeshProUGUI>().color = model.activeSelf ? 
                    activecolour : inactivecolour;
            }
        }
    }

    void ActivateModels(bool active)
    {
        foreach (GameObject model in models)
        {
            model.SetActive(model.name.Contains("Cut") ? false : active);
        }

        foreach (Button button in modelButtons)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().color =
                !active || button.name.Contains("Cut") ? inactivecolour : activecolour;
        }
    }

    public void ClosePanels(bool active)
    {
        controlsPanel.gameObject.SetActive(active);
        controlsButton.gameObject.SetActive(!active);

        areasPanel.gameObject.SetActive(false);

        ResetAreasPanel(true);
    }

    public void ResetAreasPanel(bool active)
    {
        panelTitleText.gameObject.SetActive(active);
        panelListText.gameObject.SetActive(active);

        areaTitleText.gameObject.SetActive(!active);
        areaDescriptionText.gameObject.SetActive(!active);

        backButton.gameObject.SetActive(!active);
        showButton.gameObject.SetActive(active);
        findButton.gameObject.SetActive(false);
        foreach (Button button in areaButtonsParent.GetComponentsInChildren<Button>())
        {
            button.gameObject.SetActive(false);
            button.interactable = true;
         }
    }

    public void ResetModels()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        Camera.main.transform.position = startCameraPosition;
        Camera.main.orthographicSize = startOrthographicSize;
    }

    void ActivateShowPanel(bool show)
    {
        panelTitleText.gameObject.SetActive(!show);
        panelListText.gameObject.SetActive(!show);

        areaTitleText.gameObject.SetActive(false);
        areaDescriptionText.gameObject.SetActive(false);

        backButton.gameObject.SetActive(!show);
        showButton.gameObject.SetActive(!show);
        findButton.gameObject.SetActive(show);
        areaButtonsParent.gameObject.SetActive(show);

        foreach (Button button in areaButtonsParent.GetComponentsInChildren<Button>())
        {
            button.interactable = true;
        }
    }
	
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
