using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Events;

#if !DISABLE_IAP
using UnityEngine.Purchasing;
#endif

public class IAPButtonComplement : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text textTitle;
    public TMP_Text textPrice;
    public IAPDisplayController displayController;

    [Header("IAP Type")]
    public PurchaseManager.IAPType type;

    [Header("Events")]
    public UnityEvent OnPurchaseEnd;

    public void OnEnable()
    {
#if !DISABLE_IAP
        StartCoroutine(OnEnableCoroutine());
#endif
    }

    public void OnDisable()
    {
#if !DISABLE_IAP
        PurchaseManager.instance.ON_PRODUCT_FULFILLED -= PurchaseComplete;
        PurchaseManager.instance.ON_PURCHASE_FAILED -= PurchaseFailed;
#endif
    }

#if !DISABLE_IAP
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
        while (PurchaseManager.fetchedProducts == false)
        {
            yield return null;
        }

        PurchaseManager.instance.ON_PRODUCT_FULFILLED += PurchaseComplete;
        PurchaseManager.instance.ON_PURCHASE_FAILED += PurchaseFailed;

        ShowTexts();
    }
#endif

    public void ShowTexts()
    {
#if !DISABLE_IAP
        if (textTitle != null)
        {
            textTitle.text = PurchaseManager.instance.GetProductFromType(type).metadata.localizedTitle;
        }
        if (textPrice != null)
        {
            textPrice.text = PurchaseManager.instance.GetProductFromType(type).metadata.localizedPriceString;
        }
#endif
    }

    public void OnClick()
    {
        PurchaseManager.storeController.PurchaseProduct(PurchaseManager.instance.GetProductFromType(type));
    }

    private void PurchaseComplete(Product product)
    {
#if !DISABLE_IAP
        displayController.Display();
#endif

        OnPurchaseEnd?.Invoke();
    }

    private void PurchaseFailed()
    {
        PurchaseComplete(null);
    }
}
