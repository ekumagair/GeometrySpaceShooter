using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class PopUp : MonoBehaviour
{
    public bool generic = true;

    [Header("Components")]
    public RectTransform root;
    public CanvasGroup canvasGroup;
    public TMP_Text textTitle;
    public TMP_Text textDescription;
    public Button[] buttons;
    public TMP_Text[] buttonTexts;

    public static PopUp instance;

    private System.Action action1 = null;
    private System.Action action2 = null;
    private System.Action action3 = null;
    private System.Action action4 = null;

    void Awake()
    {
        if (generic)
        {
            instance = this;
        }
    }

    void Start()
    {
        gameObject.SetActive(true);
        ResetActions();
        root.gameObject.SetActive(false);
    }

    public void SetButtonsTexts(LocalizedString btn1, LocalizedString btn2, LocalizedString btn3, LocalizedString btn4)
    {
        if (btn1 != null)
            buttonTexts[0].text = LocalizationSettings.StringDatabase.GetLocalizedString(btn1.TableReference, btn1.TableEntryReference);

        if (btn2 != null)
            buttonTexts[1].text = LocalizationSettings.StringDatabase.GetLocalizedString(btn2.TableReference, btn2.TableEntryReference);

        if (btn3 != null)
            buttonTexts[2].text = LocalizationSettings.StringDatabase.GetLocalizedString(btn3.TableReference, btn3.TableEntryReference);

        if (btn4 != null)
            buttonTexts[3].text = LocalizationSettings.StringDatabase.GetLocalizedString(btn4.TableReference, btn4.TableEntryReference);
    }

    public void SetActions(System.Action btn1, System.Action btn2, System.Action btn3, System.Action btn4)
    {
        action1 = btn1 != null ? btn1 : ClosePopUp;
        action2 = btn2 != null ? btn2 : ClosePopUp;
        action3 = btn3 != null ? btn3 : ClosePopUp;
        action4 = btn4 != null ? btn4 : ClosePopUp;
    }

    public void ResetActions()
    {
        SetActions(ClosePopUp, ClosePopUp, ClosePopUp, ClosePopUp);
    }

    public void OpenPopUp(LocalizedString title, LocalizedString desc, int btnAmount)
    {
        gameObject.SetActive(true);
        root.gameObject.SetActive(true);
        canvasGroup.interactable = true;

        textTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString(title.TableReference, title.TableEntryReference);
        textDescription.text = LocalizationSettings.StringDatabase.GetLocalizedString(desc.TableReference, desc.TableEntryReference);

        if (btnAmount > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(i < btnAmount);
            }
        }
    }

    public void OverrideDescriptionText(string text)
    {
        StartCoroutine(OverrideDescriptionTextDelay(text));
    }

    private IEnumerator OverrideDescriptionTextDelay(string text)
    {
        yield return new WaitForSeconds(0.1f);

        textDescription.text = text;
    }

    public void AppendDescriptionText(string text)
    {
        StartCoroutine (AppendDescriptionTextDelay(text));
    }

    private IEnumerator AppendDescriptionTextDelay(string text)
    {
        yield return new WaitForSeconds(0.1f);

        textDescription.text += text;
    }

    public void PressedButton(int btnId)
    {
        canvasGroup.interactable = false;
        StartCoroutine(PressedButtonCoroutine(btnId));
    }

    private IEnumerator PressedButtonCoroutine(int btnId)
    {
        yield return null;

        switch (btnId)
        {
            case 0:
                action1?.Invoke();
                break;

            case 1:
                action2?.Invoke();
                break;

            case 2:
                action3?.Invoke();
                break;

            case 3:
                action4?.Invoke();
                break;

            default:
                break;
        }

        if (PersistentCanvas.reference != null)
        {
            PersistentCanvas.reference.CreateButtonSound(0);
        }
    }

    public void ClosePopUp()
    {
        ResetActions();
        root.gameObject.SetActive(false);
    }
}
