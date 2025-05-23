using UnityEngine;

#if !DISABLE_IAP
using UnityEngine.Purchasing;
#endif

public class PurchaseManager : MonoBehaviour
{
    [SerializeField] bool _testMode = true; // Use fake ads for tests. Only use real ads for the final build.

    public enum IAPType
    {
        RemoveAds
    }

#if !DISABLE_IAP
    public static ProductCollection productCollection;
#endif

    public static PurchaseManager instance;
    public static bool fetchedProducts = false;

    // Flags for non-consumables.
    public static bool removedAds = false;
    public static bool removedAdsOnce = false; // This one is saved locally and won't become false again.

#if !DISABLE_IAP
    // Individual products as variables.
    public static Product productRemoveAds;
#endif

    // Product IDs as strings.
    public const string PRODUCT_REMOVEADS_ID = "com.geometryspaceshooter.removeads";

    private void Awake()
    {
#if !DISABLE_IAP
        instance = this;

        StandardPurchasingModule.Instance().useFakeStoreAlways = _testMode;
#endif
    }

    public void RemoveAds()
    {
        removedAds = true;

        if (UsingFakeStore() == false)
        {
            removedAdsOnce = true;
        }

#if !DISABLE_ADS
        if (AdBannerManager.instance != null)
        {
            AdBannerManager.instance.HideBannerAd();
        }
#endif

        GameStats.SaveStats();
    }

    public void EnableAds()
    {
        removedAds = false;
    }

    public bool HasRemovedAds()
    {
#if !DISABLE_IAP
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
#else
        return false;
#endif
    }

    public bool UsingFakeStore()
    {
#if UNITY_EDITOR || DISABLE_IAP
        return true;
#else
        return StandardPurchasingModule.Instance().useFakeStoreAlways;
#endif
    }


    public void OnProductsFetched(UnityEngine.Purchasing.ProductCollection collection)
    {
#if !DISABLE_IAP
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
#endif
    }


#if !DISABLE_IAP
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
#endif
}