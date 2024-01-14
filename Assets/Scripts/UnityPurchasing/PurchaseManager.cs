using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaseManager : MonoBehaviour
{
    [SerializeField] bool _testMode = true;

    public enum IAPType
    {
        RemoveAds
    }

    public static ProductCollection productCollection;
    public static Product productRemoveAds;
    public static PurchaseManager instance;

    // Non-consumables.
    public static bool removedAds = false;
    public static bool removedAdsOnce = false; // This one won't become false again.

    public const string PRODUCT_REMOVEADS_ID = "com.geometryspaceshooter.removeads";

    private void Awake()
    {
        instance = this;
        StandardPurchasingModule.Instance().useFakeStoreAlways = _testMode;
    }

    public void RemoveAds()
    {
        removedAds = true;
        removedAdsOnce = true;

        if (AdBannerManager.instance != null)
        {
            AdBannerManager.instance.HideBannerAd();
        }

        GameStats.SaveStats();
    }

    public void EnableAds()
    {
        removedAds = false;
    }

    public bool HasRemovedAds()
    {
        return removedAds;
    }

    public bool UsingFakeStore()
    {
#if UNITY_EDITOR
        return true;
#else
        return StandardPurchasingModule.Instance().useFakeStoreAlways;
#endif
    }

    public void OnProductsFetched(ProductCollection collection)
    {
        foreach (Product product in collection.all)
        {
            if (product != null)
            {
                if (product.definition.id == PRODUCT_REMOVEADS_ID)
                {
                    if (product.hasReceipt == true)
                    {
                        RemoveAds();
                    }
                    else
                    {
                        EnableAds();
                    }

                    productRemoveAds = product;
                }
            }
        }

        productCollection = collection;
    }

    public Product GetProductFromType(IAPType type)
    {
        switch (type)
        {
            case IAPType.RemoveAds:
                return productRemoveAds;

            default:
                return null;
        }
    }
}
