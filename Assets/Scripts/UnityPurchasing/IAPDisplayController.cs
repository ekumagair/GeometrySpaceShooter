using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;

public class IAPDisplayController : MonoBehaviour
{
    [Header("Objects")]
    public GameObject availableObject;
    public GameObject boughtObject;

    [Header("IAP Type")]
    public PurchaseManager.IAPType type;

    public void OnEnable()
    {
        StartCoroutine(OnEnableCoroutine());
    }

    private IEnumerator OnEnableCoroutine()
    {
        availableObject.SetActive(false);
        boughtObject.SetActive(false);

        yield return null;

        while (PurchaseManager.instance == null)
        {
            yield return null;
        }

        Display();
    }

    public void Display()
    {
        if (PurchaseManager.productCollection != null && PurchaseManager.fetchedProducts != false)
        {
            if (PurchaseManager.instance.GetProductFromType(type).hasReceipt == true)
            {
                PurchaseUnavailable();
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
                PurchaseUnavailable();
            }
            else
            {
                PurchaseAvailable();
            }
        }
    }

    private void PurchaseAvailable()
    {
        availableObject.SetActive(true);
        boughtObject.SetActive(false);
    }

    private void PurchaseUnavailable()
    {
        availableObject.SetActive(false);
        boughtObject.SetActive(true);
    }
}
