using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour, IDetailedStoreListener
{
    private List<CatalogItem> Catalog = new List<CatalogItem>();

    private static IStoreController m_StoreController;

    [SerializeField]
    private GameObject shopButtonPrefab;
    
    [SerializeField]
    private GameObject shopButtonParent;

    private void Awake()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName("production");

            UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            // An error occurred during initialization.
        }
    }

    private void Start()
    {
        //Check if logged in
        RefreshIAPItems();
    }

    private void OnGUI()
    {
        // if we are not initialized, only draw a message
        if (!IsInitialized)
        {
            GUILayout.Label("Initializing IAP and logging in...");
            return;
        }
    }

    private void RefreshIAPItems()
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest()
        {
            CatalogVersion = "ThrowableItems"
        };

        PlayFabClientAPI.GetCatalogItems(request, result => {
            Catalog = result.Catalog;

            // Make UnityIAP initialize
            InitializePurchasing();
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void InitializePurchasing()
    {
        // If IAP is already initialized, return gently
        if (IsInitialized) return;

        // Create a builder for IAP service
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));

        // Register each item from the catalog
        foreach (var item in Catalog)
        {
            builder.AddProduct(item.ItemId, ProductType.NonConsumable);
        }

        // Trigger IAP service initialization
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log(string.Format($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureDescription}"));

    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"OnInitializeFailed InitializationFailureReason: {error}. Message: {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (!IsInitialized)
        {
            return PurchaseProcessingResult.Complete;
        }

        // Test edge case where product is unknown
        if (purchaseEvent.purchasedProduct == null)
        {
            Debug.LogWarning("Attempted to process purchase with unknown product. Ignoring");
            return PurchaseProcessingResult.Complete;
        }

        // Test edge case where purchase has no receipt
        if (string.IsNullOrEmpty(purchaseEvent.purchasedProduct.receipt))
        {
            Debug.LogWarning("Attempted to process purchase with no receipt: ignoring");
            return PurchaseProcessingResult.Complete;
        }

        Debug.Log("Processing transaction: " + purchaseEvent.purchasedProduct.transactionID);

        
        // Deserialize receipt
        var googleReceipt = GooglePurchase.FromJson(purchaseEvent.purchasedProduct.receipt);

        // Invoke receipt validation
        // This will not only validate a receipt, but will also grant player corresponding items
        // only if receipt is valid.
        print("TEST INFO " + purchaseEvent.purchasedProduct.metadata.isoCurrencyCode);
        print("TEST INFO " + (uint)(purchaseEvent.purchasedProduct.metadata.localizedPrice * 100));
        print("TEST INFO " + googleReceipt.PayloadData.json);
        print("TEST INFO " + googleReceipt.PayloadData.signature);
        PlayFabClientAPI.ValidateGooglePlayPurchase(new ValidateGooglePlayPurchaseRequest()
        {
            // Pass in currency code in ISO format
            CurrencyCode = purchaseEvent.purchasedProduct.metadata.isoCurrencyCode,
            // Convert and set Purchase price
            PurchasePrice = (uint)(purchaseEvent.purchasedProduct.metadata.localizedPrice * 100),
            // Pass in the receipt
            ReceiptJson = googleReceipt.PayloadData.json,
            // Pass in the signature
            Signature = googleReceipt.PayloadData.signature
        }, result => Debug.Log("Validation successful!"),
           error => Debug.Log("Validation failed: " + error.GenerateErrorReport())
        );

        return PurchaseProcessingResult.Complete;
    }

    void BuyProductID(string productId)
    {
        print(productId);
        // If IAP service has not been initialized, fail hard
        if (!IsInitialized) throw new Exception("IAP Service is not initialized!");

        // Pass in the product id to initiate purchase
        m_StoreController.InitiatePurchase(productId);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}"));
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;

        foreach (var item in Catalog)
        {
            var button = Instantiate(shopButtonPrefab, shopButtonParent.transform);
            button.GetComponentInChildren<Button>().onClick.AddListener(() => BuyProductID(item.ItemId));
        }
    }

    public bool IsInitialized
    {
        get
        {
            return m_StoreController != null && Catalog != null;
        }
    }
}
