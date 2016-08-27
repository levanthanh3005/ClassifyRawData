using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class HTMLParser
{
    private String htmlLink;//its url
    private String urlId;//its url
    private Product product = new Product();
    private ProductPricing productPricing = new ProductPricing();
    private List<HtmlNode> psNodeLs = new List<HtmlNode>();
    private List<String> lsBreadcrumb = new List<string>();
    public Product getProduct()
    {
        return this.product;
    }
    public ProductPricing getProductPricing()
    {
        return this.productPricing;
    }
    public List<String> getLsBreadCrumb()
    {
        return lsBreadcrumb;
    }
    public HTMLParser(String htmlLink, String urlId)
    {
        this.htmlLink = htmlLink;
        this.urlId = urlId;
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
        try
        {
            Console.WriteLine("doProcess in HTMLPaser:" + Compute("applemacbookair11mjvp2", "applemacbookair11mjvp2"));
            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8  //Set UTF8 để hiển thị tiếng Việt
            };

            //Load web
            HtmlDocument document = htmlWeb.Load(this.htmlLink);
            //find Title
            psNodeLs = new List<HtmlNode>();
            findNode(document.DocumentNode, "", 0);
            String title = psNodeLs[0].InnerHtml.Replace("&quot;", "");
            Console.WriteLine("Title:" + title);
            psNodeLs = new List<HtmlNode>();
            product.setTitle(title.Replace('-', ' ').ToLower());
          
            //find breadcrumbs
            psNodeLs = new List<HtmlNode>();
            findNode(document.DocumentNode, "breadcrumb", 5);
            Console.WriteLine("find breadcrumbs:" + psNodeLs.Count);
            if (psNodeLs.Count == 0)
            {
                Accuracy.addWrongItemCount();
                Accuracy.addIssue(new Accuracy.Issue(this.urlId, this.htmlLink, "Can not find breadcrumbs"));
                product.setFullName(product.getTitle());
            }
            else
            {
                HtmlNode bcNode = psNodeLs[0];
                lsBreadcrumb = seperateBreadcrumb(bcNode);
                product.setFullName(lsBreadcrumb[lsBreadcrumb.Count - 1]);
                Console.WriteLine("product fullname:" + product.getFullName());
            }

            //find price
            psNodeLs = new List<HtmlNode>();
            findNode(document.DocumentNode, normalize(product.getFullName()), 1);
            List<HtmlNode> psNodeTitleLs = psNodeLs;
            Console.WriteLine("title:" + title + " >psNodeTitleLs>" + psNodeTitleLs.Count);
            psNodeLs = new List<HtmlNode>();
            foreach (var item in psNodeTitleLs)
            {
                Console.WriteLine("In node to find price");
                Console.WriteLine(item.InnerHtml);
                if (item.Name == "title") continue;
                findNodeContainPrice(item);
            }
            if (psNodeLs.Count == 0)
            {
                findPrice(null);
            }
            else
            {
                findPrice(psNodeLs[0]);
            }
            productPricing.setUrl(this.htmlLink);
            productPricing.setStore(this.htmlLink.Split('/')[2]);
        }catch(Exception e)
        {
            Accuracy.addWrongItemCount();
            Accuracy.addIssue(new Accuracy.Issue(this.urlId, this.htmlLink, e.ToString()));
        }
    }
    public List<String> seperateBreadcrumb(HtmlNode bcNode)
    {
        Console.WriteLine("seperateBreadcrumb");
        //String[] lsBC = System.Text.RegularExpressions.Regex.Split(text, @"\s{2,}");
        //foreach (String s in lsBC)
        //{
        //    Console.WriteLine(">>"+s);
        //}
        //List<String> lsBC = new List<string>();
        //foreach (var item in bcNode.ChildNodes)
        //{
        //    //if (item.ChildNodes.Count==1)
        //    //{
        //    Console.WriteLine("1>" + item.InnerHtml);
        //    Console.WriteLine("11>" + item.OuterHtml);
        //    Console.WriteLine("2>" + item.InnerText);
        //    Console.WriteLine("3>" + item.ChildNodes.Count);
        //    //}
        //}
        List<String> lsBC = new List<string>();
        psNodeLs = new List<HtmlNode>();
        findNode(bcNode, "", 6);
        foreach (HtmlNode item in psNodeLs)
        {
            //Console.WriteLine("1>" + item.InnerHtml);
            String text = item.InnerHtml.Replace("&nbsp;", "").Replace("&gt;", "").Replace("&amp;", "&").Trim();
            text = Regex.Replace(text, "[^0-9a-zA-Z& ]+", "").ToLower();
            if (text.Replace(" ","").Replace("&","").Length>0 && !text.Equals("home") && !text.Equals("category"))
            {
                //Console.WriteLine("1>" + text+">");
                lsBC.Add(text);
            }
        }
        //Console.ReadLine();
        return lsBC;
    }
    public void findPrice(HtmlNode prNode)
    {
        if (prNode==null)
        {
            productPricing.setOldPrice("0");
            productPricing.setNewPrice("0");
            productPricing.setCurrency("");
            Accuracy.addWrongItemCount();
            Accuracy.addIssue(new Accuracy.Issue(this.urlId, this.htmlLink, "Can not find price"));
            return;
        }
        String oldPrice = "0";
        String newPrice = "";
        String currency = "";
        //HtmlNode prNode = psNodeLs[0];
        psNodeLs = new List<HtmlNode>();
        findNode(prNode, "", 3);
        if (psNodeLs.Count > 1)
        {
            //has old price
            foreach (var item in psNodeLs)
            {
                if (findFromNode(item, prNode, "old"))
                {
                    oldPrice = normalize(item.InnerHtml).ToUpper();
                }
                else
                {
                    newPrice = normalize(item.InnerHtml).ToUpper();
                }
            }
        }
        else
        {
            oldPrice = "0";
            newPrice = normalize(psNodeLs[0].InnerHtml).ToUpper();
        }
        Boolean checkCurrency = (oldPrice != "" && !Regex.IsMatch(oldPrice, "^[0-9]")) || (newPrice != "" && !Regex.IsMatch(newPrice, "^[0-9]"));
        if (checkCurrency)
        {
            currency = Regex.Replace(newPrice, "[0-9]+", "").ToUpper();
            oldPrice = oldPrice.Replace(currency, "");
            newPrice = newPrice.Replace(currency, "");
        }
        else //find currency
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
        Console.WriteLine("oldPrice:" + oldPrice + " newPrice:" + newPrice + " currency:" + currency);
        productPricing.setOldPrice(oldPrice);
        productPricing.setNewPrice(newPrice);
        productPricing.setCurrency(currency);
        Console.WriteLine("Done in HTMLPaser");
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
        //status = 0 : find title
        //status = 1 : find by innerHTML
        //status = 2 : find by className
        //status = 3 : find price
        //status = 4 : find currency
        //status = 5 : find breadcrumb
        //status = 6 : list all node text in breadcrumb
        if (status == 0)
        {
            if (node.Name.ToLower()=="title" && node.InnerHtml != node.OuterHtml)
            {
                psNodeLs.Add(node);
            }
        }
        if (status == 1)
        {
            //if (node.InnerHtml != node.OuterHtml && node.InnerHtml.StartsWith("Apple iPhone"))
            //{
            //    Console.WriteLine(node.InnerHtml);
            //    Console.WriteLine(normalize(node.InnerHtml.Replace("&quot;", "")));
            //    Console.WriteLine(text);
            //    Console.WriteLine(Compute(normalize(node.InnerHtml.Replace("&quot;", "")), text));
            //    Console.WriteLine(StringCompare(normalize(node.InnerHtml.Replace("&quot;", "")), text));
            //    Console.ReadLine();
            //}
            if ((Compute(normalize(node.InnerHtml.Replace("&quot;","")),text)<2 || StringCompare(normalize(node.InnerHtml.Replace("&quot;", "")), text) >49) && node.InnerHtml != node.OuterHtml)
            {
                //Console.WriteLine("Have");
                //Console.WriteLine(node.InnerHtml);
                //Console.ReadLine();
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
        if (status == 5)
        {
            if (node.Attributes.Contains("class") && normalize(node.Attributes["class"].Value).Contains(text) && node.HasChildNodes)
            {
                psNodeLs.Add(node);
            }
        }
        if (status == 6)
        {
            if (!node.HasChildNodes && node.InnerHtml == node.OuterHtml)
            {
                psNodeLs.Add(node);
            }
        }
        foreach (HtmlNode nodeS in node.ChildNodes)
        {
            findNode(nodeS, text, status);
        }
    }
    public void findNodeContainPrice(HtmlNode node)
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
            Console.WriteLine("hasPrice");
            findNode(node, "price", 2);
            return;
        } 
        else
        {
            findNodeContainPrice(node.ParentNode);
        }
    }
    public int Compute(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
        {
            if (string.IsNullOrEmpty(t))
                return 0;
            return t.Length;
        }

        if (string.IsNullOrEmpty(t))
        {
            return s.Length;
        }

        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        // initialize the top and right of the table to 0, 1, 2, ...
        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 1; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                int min1 = d[i - 1, j] + 1;
                int min2 = d[i, j - 1] + 1;
                int min3 = d[i - 1, j - 1] + cost;
                d[i, j] = Math.Min(Math.Min(min1, min2), min3);
            }
        }
        return d[n, m];
    }
}
