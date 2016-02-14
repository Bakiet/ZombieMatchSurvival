using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

// Implementation of Soomla Store plugin
public class BerryStoreAssistant : MonoBehaviour {

    public List<ItemInfo> items = new List<ItemInfo>();
    public ItemInfo GetItemByID(string id) {
        return items.Find(x => x.id == id);
    }

    public List<IAP> iaps = new List<IAP>();
    public IAP GetIAPByID(string id) {
        return iaps.Find(x => x.id == id);
    }

	public static BerryStoreAssistant main;
	public Dictionary<string, string> marketItemPrices = new Dictionary<string, string>();

	void Awake () {
		main = this;
	}

	void Start () {
		SoomlaStore.Initialize(new BerryStoreAssets());
		StoreEvents.OnMarketPurchase += onMarketPurchase;
		StoreEvents.OnMarketItemsRefreshFinished += onMarketItemsRefreshFinished;
	}

	void onMarketItemsRefreshFinished (List<MarketItem> items) {
		foreach (MarketItem item in items)
			marketItemPrices.Add(item.ProductId, item.MarketPriceAndCurrency);
        foreach (BerryStorePurchaserIAP purchaser in FindObjectsOfType<BerryStorePurchaserIAP>())
            purchaser.Refresh();
	}

    // Function item purchase
    public void Purchase(int seedsCount, string goodId, int goodCount) {
        if (ProfileAssistant.main.local_profile["seed"] < seedsCount) {
            UIAssistant.main.ShowPage("Store");
            return;
        }

        ProfileAssistant.main.local_profile["seed"] -= seedsCount;
        ProfileAssistant.main.local_profile[goodId] += goodCount;
        ProfileAssistant.main.SaveUserInventory();
        ItemCounter.RefreshAll();
        AudioAssistant.Shot("Buy");

    }

	// Function item purchase
	public void PurchaseIAP (string id, string goodId, int goodCount)
	{
        IAP iap = GetIAPByID(id);
        if (iap != null)
            StoreInventory.BuyItem(id, goodId + ":" + goodCount.ToString());
	}

	// Successfully purchased
	void onMarketPurchase (PurchasableVirtualItem item, string payload, Dictionary<string, string> extra)
	{
        string[] info = payload.Split(':');
        if (info.Length != 2) return;

        ProfileAssistant.main.local_profile[info[0]] += int.Parse(info[1]);
        ProfileAssistant.main.SaveUserInventory();
        ItemCounter.RefreshAll();
		AudioAssistant.Shot ("Buy");
	}

    [System.Serializable]
    public class ItemInfo {
        public string name;
        public string id;
        public string description;
    }

    [System.Serializable]
    public class IAP {
        public string id;
        public string sku;
    }
}

public class BerryStoreAssets : IStoreAssets {

	#region IStoreAssets implementation
	public int GetVersion () {
		return 7;
	}
	
	// Descriptions of in-game currency
	public VirtualCurrency[] GetCurrencies () {
		return new VirtualCurrency[0];
	}
	
	// Descriptions of goods
	public VirtualGood[] GetGoods () {
        List<VirtualGood> goods = new List<VirtualGood>();
        foreach (BerryStoreAssistant.IAP iap in BerryStoreAssistant.main.iaps)
            goods.Add(new SingleUseVG(iap.id, "", iap.sku, new PurchaseWithMarket(iap.sku, 1)));
        return goods.ToArray();
	}
	
	// Descriptions of currency packs
	public VirtualCurrencyPack[] GetCurrencyPacks ()
	{
		return new VirtualCurrencyPack[0];
	}
	
	// Descriptions of categories
	public VirtualCategory[] GetCategories ()
	{
		return new VirtualCategory[0];
	}
	#endregion


}
