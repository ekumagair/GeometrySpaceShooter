using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
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

    public static string storeName = null;
    public static string environment = "editor";

#if !DISABLE_IAP
    public static List<Product> productCollection;
    public static StoreController storeController;
#endif

    public static PurchaseManager instance;
    public static bool fetchedProducts = false;
    public static bool fetchedPurchases = false;

    // Flags for non-consumables.
    public static bool removedAds = false;
    public static bool removedAdsOnce = false; // This one is saved locally and won't become false again.

#if !DISABLE_IAP
    // Individual products as variables.
    public static Product productRemoveAds;
    public static Entitlement entitlementRemoveAds;
#endif

    // Product IDs as strings.
    public const string PRODUCT_REMOVEADS_ID = "com.geometryspaceshooter.removeads";

    // Actions.
#if !DISABLE_IAP
    public Action<Product> ON_PRODUCT_FULFILLED;
    public Action ON_PURCHASE_CONFIRMED;
    public Action ON_PURCHASE_FAILED;
#endif

#if !DISABLE_IAP
    void Awake()
    {
        instance = this;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        _testMode = true;
#endif

        storeName = _testMode ? "fake" : null;
        environment = _testMode ? "editor" : "production";

        InitializeIAP();
    }
#endif

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
#if DISABLE_IAP || UNITY_EDITOR
        return true;
#else
        return storeName == "fake";
#endif
    }

#if !DISABLE_IAP
    private async void InitializeIAP()
    {
        if (Debug.isDebugBuild) { Debug.Log("Initialized IAP [start]"); }

        await Task.Delay(1000);

        var options = new InitializationOptions().SetEnvironmentName(environment);
        await UnityServices.InitializeAsync(options);

        if (Debug.isDebugBuild) { Debug.Log("Unity services initialized!"); }

        storeController = UnityIAPServices.StoreController(storeName);

        storeController.OnStoreDisconnected += OnStoreDisconnected;
        storeController.OnProductsFetchFailed += OnProductsFetchFailed;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        storeController.OnPurchaseFailed += OnPurchaseFailed;
        storeController.OnPurchaseDeferred += OnPurchaseDeferred;

        storeController.OnPurchasePending += OnPurchasePending;

        await storeController.Connect();

        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnPurchasesFetched += OnPurchasesFetched;

        var initialProductsToFetch = new List<ProductDefinition>
        {
            new(PRODUCT_REMOVEADS_ID, ProductType.NonConsumable),
        };

        storeController.FetchProducts(initialProductsToFetch);
        storeController.FetchPurchases();

        while (!fetchedProducts || !fetchedPurchases)
        {
            await Task.Yield();
        }

        storeController.OnCheckEntitlement += OnCheckEntitlement;
        CheckAllEntitlements();

        if (Debug.isDebugBuild) { Debug.Log("Initialized IAP [end]"); }
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription desc)
    {
        if (Debug.isDebugBuild) { Debug.Log("Store disconnected. " + desc.message); }
    }

    private void OnPurchasePending(PendingOrder order)
    {
        foreach (CartItem cItem in order.CartOrdered.Items())
        {
            FulfillProduct(cItem.Product, true);
        }

        storeController.ConfirmPurchase(order);
    }

    private void OnProductsFetched(List<Product> collection)
    {
        foreach (Product product in collection)
        {
            if (product != null)
            {
                if (product.definition.id == PRODUCT_REMOVEADS_ID)
                {
                    productRemoveAds = product;
                }
            }
        }

        productCollection = collection;
        fetchedProducts = true;
    }

    private void OnProductsFetchFailed(ProductFetchFailed fail)
    {
        if (Debug.isDebugBuild) { Debug.Log("Purchases fetch failed. " + fail.FailureReason); }

        fetchedProducts = true;
    }

    private void OnPurchasesFetched(Orders orders)
    {
        if (Debug.isDebugBuild) { Debug.Log("Purchases fetch success! Confirmed orders: " + orders.ConfirmedOrders.Count.ToString()); }

        fetchedPurchases = true;
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription desc)
    {
        if (Debug.isDebugBuild) { Debug.Log("Purchases fetch failed. " + desc.message); }

        fetchedPurchases = true;
    }

    private void CheckAllEntitlements()
    {
        storeController.CheckEntitlement(productRemoveAds);
    }

    private void OnCheckEntitlement(Entitlement entitlement)
    {
        string id = entitlement.Product.definition.id;

        switch (id)
        {
            case PRODUCT_REMOVEADS_ID:
                entitlementRemoveAds = entitlement;
                break;

            default:
                if (Debug.isDebugBuild) { Debug.LogError("Checked entitlement of an invalid product."); }
                break;
        }

        FulfillProduct(entitlement.Product, entitlement.Status != EntitlementStatus.NotEntitled);
    }

    public void FulfillProduct(Product product, bool entitled)
    {
        string id = product.definition.id;

        switch (id)
        {
            case PRODUCT_REMOVEADS_ID:
                if (entitled)
                {
                    RemoveAds();
                }
                else
                {
                    EnableAds();
                }
                break;

            default:
                if (Debug.isDebugBuild) { Debug.LogError("Tried to fulfill invalid product."); }
                break;
        }

        ON_PRODUCT_FULFILLED?.Invoke(product);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        if (Debug.isDebugBuild) { Debug.Log("Purchase confirmed!"); }

        CheckAllEntitlements();
        ON_PURCHASE_CONFIRMED?.Invoke();

        ReportEntitlements();
    }

    private void OnPurchaseFailed(FailedOrder failedOrder)
    {
        if (Debug.isDebugBuild) { Debug.Log("Purchase failed."); }

        ON_PURCHASE_FAILED?.Invoke();

        ReportEntitlements();
    }

    private void OnPurchaseDeferred(DeferredOrder deferredOrder)
    {
        if (Debug.isDebugBuild) { Debug.Log("Purchase deferred."); }
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
#endif

    public void ReportEntitlements()
    {
#if !DISABLE_IAP && (DEVELOPMENT_BUILD || UNITY_EDITOR)
        Debug.Log("RemoveAds entitlement: " + entitlementRemoveAds.Status.ToString());
#endif
    }
}