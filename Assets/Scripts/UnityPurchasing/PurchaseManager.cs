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
    public static PurchaseManager instance;
    public static bool fetchedProducts = false;

    // Flags for non-consumables.
    public static bool removedAds = false;
    public static bool removedAdsOnce = false; // This one is saved locally and won't become false again.

    // Individual products as variables.
    public static Product productRemoveAds;

    // Product IDs as strings.
    public const string PRODUCT_REMOVEADS_ID = "com.geometryspaceshooter.removeads";

    private void Awake()
    {
        instance = this;
        StandardPurchasingModule.Instance().useFakeStoreAlways = _testMode;
    }

    public void RemoveAds()
    {
        removedAds = true;

        if (UsingFakeStore() == false)
        {
            removedAdsOnce = true;
        }

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
        if (fetchedProducts)
        {
            // If has connection to the IAP service, check for product receipt.
            return removedAds;
        }
        else
        {
            // If the connection to the IAP service has failed, check for local save.
            return removedAdsOnce;
        }
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
        fetchedProducts = true;
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
