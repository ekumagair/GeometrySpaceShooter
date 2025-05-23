using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if !DISABLE_IAP
using UnityEngine.Purchasing;
#endif

public class IAPDisplayController : MonoBehaviour
{
    [Header("Objects")]
    public GameObject availableObject;
    public GameObject boughtObject;
    public GameObject placeholderObject;

    [Header("IAP Type")]
    public PurchaseManager.IAPType type;

    public void OnEnable()
    {
#if !DISABLE_IAP
        StartCoroutine(OnEnableCoroutine());
#else
        gameObject.SetActive(false);
#endif
    }

#if !DISABLE_IAP
    private IEnumerator OnEnableCoroutine()
    {
        availableObject.SetActive(false);
        boughtObject.SetActive(false);
        placeholderObject.SetActive(true);

        yield return null;

        while (PurchaseManager.instance == null)
        {
            yield return null;
        }

        Display();
    }
#endif

    public void Display()
    {
#if !DISABLE_IAP
        if (PurchaseManager.productCollection != null && PurchaseManager.fetchedProducts != false && PurchaseManager.instance != null)
        {
            if (PurchaseManager.instance.GetProductFromType(type).hasReceipt == true)
            {
                PurchaseAlreadyMade();
            }
            else
            {
                PurchaseAvailable();
            }
        }
        else
        {
            if (type == PurchaseManager.IAPType.RemoveAds && PurchaseManager.removedAdsOnce == true)
            {
                PurchaseAlreadyMade();
            }
            else
            {
                PurchaseAvailable();
            }
        }
#else
        gameObject.SetActive(false);
#endif

        placeholderObject.SetActive(false);
    }

    private void PurchaseAvailable()
    {
        availableObject.SetActive(true);
        boughtObject.SetActive(false);
    }

    private void PurchaseAlreadyMade()
    {
        availableObject.SetActive(false);
        boughtObject.SetActive(true);
    }
}
