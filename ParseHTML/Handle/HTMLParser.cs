using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class HTMLParser
{
    private String htmlLink;//its url
    private Product product = new Product();
    private ProductPricing productPricing = new ProductPricing();
    private List<HtmlNode> psNodeLs = new List<HtmlNode>();
    public Product getProduct()
    {
        return this.product;
    }
    public ProductPricing getProductPricing()
    {
        return this.productPricing;
    }
    public HTMLParser(String htmlLink)
    {
        this.htmlLink = htmlLink;
    }
    private String normalize(String text)
    {
        return Regex.Replace(text, "[^0-9a-zA-Z]+", "").ToLower();
    }
    private double StringCompare(string a, string b)
    {
        if (a == b) //Same string, no iteration needed.
            return 100;
        if ((a.Length == 0) || (b.Length == 0)) //One is empty, second is not
        {
            return 0;
        }
        double maxLen = a.Length > b.Length ? a.Length : b.Length;
        int minLen = a.Length < b.Length ? a.Length : b.Length;
        int sameCharAtIndex = 0;
        for (int i = 0; i < minLen; i++) //Compare char by char
        {
            if (a[i] == b[i])
            {
                sameCharAtIndex++;
            }
        }
        return sameCharAtIndex / maxLen * 100;
    }
    public void doProcess()
    {
        Console.WriteLine(StringCompare("apple iphone 6 128gb silver 1901422", "apple iphone 6 128gb silver"));
        HtmlWeb htmlWeb = new HtmlWeb()
        {
            AutoDetectEncoding = false,
            OverrideEncoding = Encoding.UTF8  //Set UTF8 để hiển thị tiếng Việt
        };

        //Load web
        HtmlDocument document = htmlWeb.Load(this.htmlLink);
        String title = this.htmlLink.Split('/')[3];
        if (title.Contains(".html"))
        {
            title = title.Substring(0, title.Length - 5);
        }
        product.setTitle(title.Replace('-', ' ').ToLower());
        title = normalize(title);
        productPricing.setStore(this.htmlLink.Split('/')[2]);
        Console.WriteLine("title:"+title);
        findNode(document.DocumentNode, title, 1);
        List<HtmlNode> psNodeTitleLs = psNodeLs;
        psNodeLs = new List<HtmlNode>();
        foreach (var item in psNodeTitleLs)
        {
            if (item.Name == "title") continue;
            findPrice(item);
        }
        HtmlNode prNode = psNodeLs[0];
        psNodeLs = new List<HtmlNode>();
        findNode(prNode, "", 3);
        //find price
        String oldPrice = "";
        String newPrice = "";
        String currency = "";
        if (psNodeLs.Count > 1)
        {
            //has old price
            foreach (var item in psNodeLs)
            {
                if (findFromNode(item, prNode, "old")) {
                    oldPrice = normalize(item.InnerHtml).ToUpper();
                } else
                {
                    newPrice = normalize(item.InnerHtml).ToUpper();
                }
            }
        } else
        {
            oldPrice = "";
            newPrice = normalize(psNodeLs[0].InnerHtml).ToUpper();
        }
        Boolean checkCurrency = (oldPrice!="" && !Regex.IsMatch(oldPrice, "^[0-9]")) || (newPrice != "" && !Regex.IsMatch(newPrice, "^[0-9]"));
        if (checkCurrency)
        {
            currency = Regex.Replace(newPrice, "[0-9]+", "").ToUpper();
            oldPrice = oldPrice.Replace(currency, "");
            newPrice = newPrice.Replace(currency, "");
        } else //find currency
        {
            psNodeLs = new List<HtmlNode>();
            findNode(prNode, "currency", 4);
            //foreach (HtmlNode nodeS in psNodeLs)
            //{
            //    Console.WriteLine(">>>HOPE >>>" + nodeS.InnerHtml);
            //}
            //foreach (HtmlNode nodeS in prNode.ChildNodes)
            //{
            //    Console.WriteLine(">>>" + nodeS.InnerHtml+" "+nodeS.ChildNodes.Count );
            //}
            currency = normalize(psNodeLs[0].InnerHtml).ToUpper();
        }
        Console.WriteLine("oldPrice:" + oldPrice + " newPrice:" + newPrice+" currency:"+currency);
        productPricing.setOldPrice(oldPrice);
        productPricing.setNewPrice(newPrice);
        productPricing.setCurrency(currency);
        productPricing.setTimestamp(getTimestampNow());
    }
    public Boolean findFromNode(HtmlNode node, HtmlNode limitNode, String text)
    {
        while(node.OuterHtml!=limitNode.OuterHtml && !node.OuterHtml.ToLower().Contains(text))
        {
            node = node.ParentNode;
        }
        if (node.OuterHtml.ToLower().Contains(text) && node.OuterHtml != limitNode.OuterHtml)
        {
            return true;
        }
        return false;
    }
    public void findNode(HtmlNode node, String text, int status)
    {
        //status = 1 : find by innerHTML
        //status = 2 : find by className
        //status = 3 : find price
        if (status == 1)
        {
            if (StringCompare(normalize(node.InnerHtml),text)>70 && node.InnerHtml != node.OuterHtml)
            {
                psNodeLs.Add(node);
            }
        }
        if (status == 2)
        {
            if (node.Attributes.Contains("class") && normalize(node.Attributes["class"].Value).StartsWith(text) && node.InnerHtml != node.OuterHtml)
            {
                psNodeLs.Add(node);
            }
        }
        if (status == 3)
        {
            if (!node.HasChildNodes && node.InnerHtml == node.OuterHtml && Regex.IsMatch(node.InnerHtml, ".*[0-9].*") && !node.InnerHtml.Contains('%'))
            {
                psNodeLs.Add(node);
            }
        }
        if (status == 4)
        {
            //Console.WriteLine("444:" + node.InnerHtml+" "+node.ChildNodes.Count);
            //Console.WriteLine("555:" + node.OuterHtml + " " + node.ChildNodes.Count);
            if (normalize(node.OuterHtml).Contains(text) && node.ChildNodes.Count<2)
            {
                //Console.WriteLine("66666666666666:"+node.InnerHtml);
                //foreach(HtmlNode hn1 in node.ChildNodes)
                //{
                //    Console.WriteLine(hn1.InnerHtml);
                //}
                psNodeLs.Add(node);
            }
        }
        foreach (HtmlNode nodeS in node.ChildNodes)
        {
            findNode(nodeS, text, status);
        }
    }
    public void findPrice(HtmlNode node)
    {
        if (node.ParentNode.Name == "body")
        {
            return;
        }
        if (node.ParentNode.InnerHtml.ToLower().Contains("breadcrumb"))
        {
            return;
        }
        if (normalize(node.InnerHtml).Contains("price"))
        {
            findNode(node, "price", 2);
            return;
        } 
        else
        {
            findPrice(node.ParentNode);
        }
    }
    public String getTimestampNow()
    {
        return new DateTime().ToString("yyyyMMddHHmmssffff");
    }
}
