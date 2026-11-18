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
    [SerializeField] GameObject labelButtonsParent;

    [SerializeField] Button controlsButton;
    [SerializeField] Button controlsExitButton;
    [SerializeField] Button showAllButton;
    [SerializeField] Button hideAllButton;
    [SerializeField] Button resetButton;
    [SerializeField] Button labelsButton;

    [SerializeField] Image controlsPanel;
    [SerializeField] Image labelsPanel;

    List<GameObject> models = new List<GameObject>();
    Button[] modelButtons;
    Button[] labelButtons;

    Quaternion startRotation;

    Vector3 startPosition;
    Vector3 startCameraPosition;
    float startOrthographicSize;

    Color32 activecolour = new Color32 (50, 50, 50, 255);
    Color32 inactivecolour = new Color32 (250, 250, 250, 255);

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

        labelButtons = labelButtonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in labelButtons)
        {
            button.onClick.AddListener(() => { ToggleLabelButton(button); });
        }

        controlsExitButton.GetComponent<Button>().onClick.AddListener(() =>
            { ActivateModels(true); });
        showAllButton.GetComponent<Button>().onClick.AddListener(() =>
            { ActivateModels(true); });
        hideAllButton.GetComponent<Button>().onClick.AddListener(() =>
            { ActivateModels(false); });

        controlsExitButton.GetComponent<Button>().onClick.AddListener(ResetModels);
        resetButton.GetComponent<Button>().onClick.AddListener(ResetModels);

        startPosition = transform.position;
        startRotation = transform.rotation;

        startCameraPosition = Camera.main.transform.position;
        startOrthographicSize = Camera.main.orthographicSize;
    }

    void ToggleModel(Button clicked)
    {
        foreach (GameObject model in models)
        {
            if (clicked.name.Split()[0] == model.name)
            {
                model.SetActive(!model.activeSelf);

                clicked.GetComponentInChildren<TextMeshProUGUI>().color = model.activeSelf ? 
                    activecolour : inactivecolour;

                foreach (Button button in labelButtons)
                {
                    if (button.name == clicked.name)
                    {
                        button.gameObject.SetActive(model.activeSelf);
                    }
                }

                break;
            }
        }
    }

    void ToggleLabelButton(Button clicked)
    {
        Color32 textcolour = clicked.GetComponentInChildren<TextMeshProUGUI>().color;

                clicked.GetComponentInChildren<TextMeshProUGUI>().color =
                    textcolour.r == activecolour.r ? inactivecolour : activecolour;
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

        foreach (Button button in labelButtons)
        {
            button.gameObject.SetActive(active);
        }
    }

    public void ClosePanels(bool active)
    {
        controlsPanel.gameObject.SetActive(active);
        controlsButton.gameObject.SetActive(!active);

        labelsPanel.gameObject.SetActive(false);

        ResetLabelsPanel(true);
    }

    public void ResetLabelsPanel(bool active)
    {
        foreach (Button button in labelButtonsParent.GetComponentsInChildren<Button>())
        {
            foreach (GameObject model in models)
            {
                if (button.name.Split()[0] == model.name)
                {
                    button.gameObject.SetActive(model.activeSelf);
                }
            }
        }
    }

    public void ResetModels()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        Camera.main.transform.position = startCameraPosition;
        Camera.main.orthographicSize = startOrthographicSize;
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
