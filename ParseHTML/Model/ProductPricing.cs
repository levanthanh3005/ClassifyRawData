using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ProductPricing
{
    private int id;
    private int productId;
    private String oldPrice;
    private String newPrice;
    private String currency;
    private String store;
    private String url;
    private String timestamp;
    public ProductPricing()
    {
        setTimestamp(getDateNow());
    }

    public ProductPricing(int id, int productId, String oldPrice, String newPrice, String currency, String store, String url, String timestamp)
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

    public int getId()
    {
        return id;
    }

    public void setId(int id)
    {
        this.id = id;
    }

    public int getProductId()
    {
        return productId;
    }

    public void setProductId(int productId)
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
    public String getDateNow()
    {
        DateTime myDateTime = DateTime.Now;
        return myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }
    public void synWithConnnection(SqlConnection cnn)
    {
        //cnn.Close();
        //cnn.Open();
        Console.WriteLine("synWithConnnection");
        String sql = "select MAX(id) from dbo.Product;";
        SqlCommand command = new SqlCommand(sql, cnn);
        SqlDataReader dataReader = command.ExecuteReader();
        dataReader.Read();
        Console.WriteLine("ID:"+dataReader.GetValue(0)+">"+this.getOldPrice()+">"+this.getNewPrice());
        setProductId(int.Parse(dataReader.GetValue(0).ToString()));
        dataReader.Close();
        sql = "Insert into dbo.ProductPricing "+
            "( ProductId ,OldPrice ,NewPrice ,Currency ,Store ,Url ,Timestamp) values "+
            "(@ProductId,@OldPrice,@NewPrice,@Currency,@Store,@Url,@Timestamp);";
        command = new SqlCommand(sql, cnn);
        command.Parameters.AddWithValue("@ProductId", this.getProductId());
        command.Parameters.AddWithValue("@OldPrice", this.getOldPrice());
        command.Parameters.AddWithValue("@NewPrice", this.getNewPrice());
        command.Parameters.AddWithValue("@Currency", this.getCurrency());
        command.Parameters.AddWithValue("@Store", this.getStore());
        command.Parameters.AddWithValue("@Url", this.getUrl());
        command.Parameters.AddWithValue("@Timestamp", this.getTimestamp());
        int result = command.ExecuteNonQuery();
    }
}
