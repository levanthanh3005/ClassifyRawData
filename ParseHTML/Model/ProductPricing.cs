using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ProductPricing
{
    private String id;
    private String productId;
    private String oldPrice;
    private String newPrice;
    private String currency;
    private String store;
    private String url;
    private String timestamp;

    public ProductPricing()
    {
    }

    public ProductPricing(String id, String productId, String oldPrice, String newPrice, String currency, String store, String url, String timestamp)
    {
        this.id = id;
        this.productId = productId;
        this.oldPrice = oldPrice;
        this.newPrice = newPrice;
        this.currency = currency;
        this.store = store;
        this.url = url;
        this.timestamp = timestamp;
    }

    public String getId()
    {
        return id;
    }

    public void setId(String id)
    {
        this.id = id;
    }

    public String getProductId()
    {
        return productId;
    }

    public void setProductId(String productId)
    {
        this.productId = productId;
    }

    public String getOldPrice()
    {
        return oldPrice;
    }

    public void setOldPrice(String oldPrice)
    {
        this.oldPrice = oldPrice;
    }

    public String getNewPrice()
    {
        return newPrice;
    }

    public void setNewPrice(String newPrice)
    {
        this.newPrice = newPrice;
    }

    public String getCurrency()
    {
        return currency;
    }

    public void setCurrency(String currency)
    {
        this.currency = currency;
    }

    public String getStore()
    {
        return store;
    }

    public void setStore(String store)
    {
        this.store = store;
    }

    public String getUrl()
    {
        return url;
    }

    public void setUrl(String url)
    {
        this.url = url;
    }

    public String getTimestamp()
    {
        return timestamp;
    }

    public void setTimestamp(String timestamp)
    {
        this.timestamp = timestamp;
    }

}
