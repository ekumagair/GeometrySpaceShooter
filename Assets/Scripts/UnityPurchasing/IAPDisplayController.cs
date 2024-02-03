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
    public GameObject placeholderObject;

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
        placeholderObject.SetActive(true);

        yield return null;

        while (PurchaseManager.instance == null)
        {
            yield return null;
        }

        Display();
    }

    public void Display()
    {
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
