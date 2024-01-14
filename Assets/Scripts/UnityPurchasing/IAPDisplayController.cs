using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;

public class IAPDisplayController : MonoBehaviour
{
    [Header("Objects")]
    public GameObject buttonObject;
    public GameObject boughtObject;

    [Header("IAP Type")]
    public PurchaseManager.IAPType type;

    public void OnEnable()
    {
        StartCoroutine(OnEnableCoroutine());
    }

    private IEnumerator OnEnableCoroutine()
    {
        yield return null;

        while (PurchaseManager.instance == null)
        {
            yield return null;
        }
        while (PurchaseManager.productCollection == null)
        {
            yield return null;
        }

        Display();
    }

    public void Display()
    {
        if (PurchaseManager.instance.GetProductFromType(type).hasReceipt == true)
        {
            buttonObject.SetActive(false);
            boughtObject.SetActive(true);
        }
        else
        {
            buttonObject.SetActive(true);
            boughtObject.SetActive(false);
        }
    }
}
