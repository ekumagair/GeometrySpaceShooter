using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class IAPButtonComplement : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text textTitle;
    public TMP_Text textPrice;
    public IAPDisplayController displayController;

    [Header("IAP Type")]
    public PurchaseManager.IAPType type;

    public void OnEnable()
    {
#if !DISABLE_IAP
        StartCoroutine(OnEnableCoroutine());
#endif
    }

    private IEnumerator OnEnableCoroutine()
    {
        if (textPrice != null)
        {
            LocalizedString placeholder = new LocalizedString("Main", "buy");
            textPrice.text = LocalizationSettings.StringDatabase.GetLocalizedString(placeholder.TableReference, placeholder.TableEntryReference);
        }

        yield return null;

        while (PurchaseManager.instance == null)
        {
            yield return null;
        }
        while (PurchaseManager.productCollection == null)
        {
            yield return null;
        }

        ShowTexts();
    }

    public void ShowTexts()
    {
        if (textTitle != null)
        {
            textTitle.text = PurchaseManager.instance.GetProductFromType(type).metadata.localizedTitle;
        }
        if (textPrice != null)
        {
            textPrice.text = PurchaseManager.instance.GetProductFromType(type).metadata.localizedPriceString;
        }
    }

    public void PurchaseComplete()
    {
        switch (type)
        {
            case PurchaseManager.IAPType.RemoveAds:
                PurchaseManager.instance.RemoveAds();
                break;

            default:
                if (Debug.isDebugBuild) { Debug.LogError("IAP button error: invalid IAP type."); }
                break;
        }

        displayController.Display();
    }
}
