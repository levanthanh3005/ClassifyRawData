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
    private String htmlValue = null;//its url
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
    public HTMLParser(String htmlLink, String urlId, String htmlValue)
    {
        this.htmlLink = htmlLink;
        this.urlId = urlId;
        this.htmlValue = htmlValue;
    }
    /// <summary>
    /// The function will remove all characters thats not number or alphabet characters
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private String normalize(String text)
    {
        return Regex.Replace(text, "[^0-9a-zA-Z]+", "").ToLower();
    }
    /// <summary>
    /// This function will compare 2 strings, for example, 11 and 111, result is 2/3 = 60%
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
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
    /// <summary>
    /// HTML Parser process, after that, image url, product's price, breadcrumb, keyword, product title will be detected
    /// </summary>
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
            HtmlDocument document = null;
            if (htmlValue == null)
            {
                document = htmlWeb.Load(this.htmlLink);
            }
            else
            {
                document.LoadHtml(htmlValue);
            }
            //Console.WriteLine(document.DocumentNode);
            //Console.ReadLine();
            //find Title
            psNodeLs = new List<HtmlNode>();
            findNode(document.DocumentNode, "", 0);
            String title = psNodeLs[0].InnerHtml.Replace("&quot;", "");
            Console.WriteLine("Title:" + title);
            product.setTitle(title.Replace('-', ' ').ToLower());

            //find keyword
            psNodeLs = new List<HtmlNode>();
            findNode(document.DocumentNode, "keyword", 7);
            String keyword = "";
            if (psNodeLs.Count > 0 && psNodeLs[0].Attributes.Contains("content"))
            {
                keyword = psNodeLs[0].Attributes["content"].Value;
                //node.Attributes.Contains("name") && (node.Attributes["name"].Value).StartsWith(text)
            }
            Console.WriteLine("keyword:" + keyword);
            product.setKeywords(keyword);

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
                //Console.WriteLine("In node to find price");
                //Console.WriteLine(item.InnerHtml);
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

            //find Img
            Console.WriteLine("find image");
            product.setImage("");
            psNodeLs = new List<HtmlNode>();
            foreach (var item in psNodeTitleLs)
            {
                //Console.WriteLine("In node to find image:>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                //Console.WriteLine(item.ParentNode.OuterHtml);
                if (item.Name == "title") continue;
                findImage(item);
            }
            //Console.WriteLine("result img node");
            HtmlNode imgNode = null;
            if (psNodeLs.Count == 1)
            {
                //product.setImage(psNodeLs[0].Attributes["src"].Value);
                imgNode = psNodeLs[0];
            }
            else
            {
                foreach (var item in psNodeLs)
                {
                    //Console.WriteLine(item.OuterHtml);
                    if (StringCompare(normalize(product.getFullName().Replace("&quot;", "")), normalize(item.Attributes["alt"].Value.Replace("&quot;", ""))) > 10)
                    {
                        imgNode = item;
                        break;
                    }
                    //Console.WriteLine(StringCompare(normalize(product.getFullName().Replace("&quot;", "")), normalize(item.Attributes["alt"].Value.Replace("&quot;", ""))));
                    //Console.WriteLine(item.Attributes["alt"].Value);
                    //Console.WriteLine(product.getFullName());
                    //Console.ReadLine();
                }
            }
            if (imgNode != null)
            {
                foreach (var att in imgNode.Attributes)
                {
                    if (!att.Value.ToLower().Contains("loading"))
                    {
                        product.setImage(att.Value);
                        break;
                    }
                }
            }
            //if (product.getImage() == null || product.getImage().Length == 0)
            //{
            //    product.setImage("Not found");
            //}
            //Console.ReadLine();
            Console.WriteLine("Done all");
        }
        catch(Exception e)
        {
            Accuracy.addWrongItemCount();
            Accuracy.addIssue(new Accuracy.Issue(this.urlId, this.htmlLink, e.ToString()));
            Console.WriteLine("Error:" + e.ToString());
        }
    }
    /// <summary>
    /// this function will find image links in node
    /// </summary>
    /// <param name="node"></param>
    public void findImage(HtmlNode node)
    {
        //Console.WriteLine("start to find image");
        //Console.WriteLine(node.OuterHtml);
        //Console.ReadLine();
        ////find node that id or class contain image, galery, preview
        //psNodeLs = new List<HtmlNode>();
        //findNode(rootNode, "", 8);
        //Console.WriteLine("findImage");
        //foreach(HtmlNode h in psNodeLs )
        //{
        //    Console.WriteLine(h.InnerHtml);
        //}
        //Console.ReadLine();
        if (node.Name == "body")
        {
            //Console.WriteLine("stop 1");
            //Console.ReadLine();
            return;
        }
        if (node.OuterHtml.ToLower().Contains("breadcrumb"))
        {
            //Console.WriteLine("stop 2");
            //Console.ReadLine();
            return;
        }
        if (node.OuterHtml.Contains("img"))
        {
            //Console.WriteLine("find image begin find node:"+node.ChildNodes.Count);
            //Console.WriteLine(node.InnerHtml.Contains("img"));
            //Console.ReadLine();
            findNode(node, "img", 9);
            //List<HtmlNode> lsn =  node.ChildNodes.Where(n => n.Name.Contains("img")).ToList();
            //foreach(var ni in lsn)
            //{
            //    Console.WriteLine(ni.InnerHtml);
            //    psNodeLs.Add(ni);
            //}
            //Console.ReadLine();
            return;
        }
        {
            //Console.WriteLine("begin to loop");
            //Console.ReadLine();
            findImage(node.ParentNode);
        }
    }
    /// <summary>
    /// This funcition will return the list of breadcrumb in bcNode (BreadCrumb Node)
    /// </summary>
    /// <param name="bcNode"></param>
    /// <returns></returns>
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
            String text = item.InnerHtml.Replace("&nbsp;", "").Replace("&gt;", "").Replace("&amp;", "&").Replace("&quot;", "").Trim();
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
    /// <summary>
    /// This function will find the price in prNode (Node contain price)
    /// </summary>
    /// <param name="prNode"></param>
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
        String newPrice = "0";
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
        else if (psNodeLs.Count == 1)
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
            if (psNodeLs.Count > 0)
            {
                currency = normalize(psNodeLs[0].InnerHtml).ToUpper();
            }
        }
        Console.WriteLine("oldPrice:" + oldPrice + " newPrice:" + newPrice + " currency:" + currency);
        productPricing.setOldPrice(oldPrice);
        productPricing.setNewPrice(newPrice);
        productPricing.setCurrency(currency);
        Console.WriteLine("Done in HTMLPaser");
    }
    /// <summary>
    /// This function will find node containing text from node to limitNode, limitNode contains node
    /// </summary>
    /// <param name="node"></param>
    /// <param name="limitNode"></param>
    /// <param name="text"></param>
    /// <returns></returns>
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
    /// <summary>
    /// This function will find the smallest node that contain text with status conditions from node to its childrens
    /// </summary>
    /// <param name="node"></param>
    /// <param name="text"></param>
    /// <param name="status"></param>
    public void findNode(HtmlNode node, String text, int status)
    {
        //status = 0 : find title
        //status = 1 : find by innerHTML
        //status = 2 : find by className
        //status = 3 : find price
        //status = 4 : find currency
        //status = 5 : find breadcrumb
        //status = 6 : list all node text in breadcrumb
        //status = 7 : find keyword
        //status = 8 : find Image
        //status = 9 : find by name of node
        if (status == 0)
        {
            if (node.Name.ToLower()=="title" && node.InnerHtml != node.OuterHtml)
            {
                psNodeLs.Add(node);
            }
        }
        if (status == 1)
        {
            //if (node.InnerHtml != node.OuterHtml && node.InnerHtml.StartsWith("Apple MacBook"))
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
        if (status == 7)
        {
            //if (node.Attributes.Contains("name") && (node.Attributes["name"].Value).StartsWith(text))
            //{
            //    Console.WriteLine("have meta:"+node.OuterHtml);
            //    Console.WriteLine("have meta:" + node.Attributes["content"]);
            //    Console.ReadLine();
            //}
            if (node.Attributes.Contains("name") && (node.Attributes["name"].Value).StartsWith(text))
            {
                psNodeLs.Add(node);
            }
        }
        //if (status == 8)
        //{
        //    if (node.InnerHtml.Contains("img"))
        //    {
        //        foreach (string s in new string[2] { "class", "id" }) {
        //            if (node.Attributes.Contains(s))
        //            {
        //                String sv = node.Attributes[s].Value.ToLower();
        //                if (sv.Contains("image") || sv.Contains("preview") || sv.Contains("gallery")) psNodeLs.Add(node);
        //            }
        //        }
        //    }
        //}
        if (status == 9)
        {
            if (node.OuterHtml.Contains(text))
            {
                //Console.WriteLine(node.OuterHtml);
                //Console.WriteLine("^^^>>" + node.Name + " " + (node.Name.Contains(text)) + " " + node.OuterHtml.Contains(text) + " " + node.ChildNodes.Count);
                //Console.ReadLine();
                if (node.Name.Contains(text))
                {
                    //Console.WriteLine("have node name");
                    psNodeLs.Add(node);
                }
            }
        }
        foreach (HtmlNode nodeS in node.ChildNodes)
        {
            findNode(nodeS, text, status);
        }
    }
    /// <summary>
    /// This function will find the node containing price information
    /// </summary>
    /// <param name="node"></param>
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
    /// <summary>
    /// This function will compute the diff between s and t, for example, 123 and 124, the result is 1 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    /// <returns></returns>
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
