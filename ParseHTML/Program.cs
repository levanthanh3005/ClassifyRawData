using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseHTML
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("apple iphone 6 128gb silver".("apple iphone 6 128gb silver 1901422"));
            Config.readConfig();
            string connetionString = null;
            SqlConnection cnn;
            SqlConnection cnn2;
            connetionString = "Data Source="+Config.dataSource+";Initial Catalog="+Config.dbName+";User ID="+Config.user+";Password="+Config.pass;
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn2 = new SqlConnection(connetionString);
                cnn.Close();cnn.Open();
                cnn2.Close(); cnn2.Open();
                Console.WriteLine("Done connection");
                String sql = "select * from dbo.RawPage where id<10";
                SqlCommand command = new SqlCommand(sql, cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    //HTMLLoadData h = new HTMLLoadData(dataReader.GetValue(2).ToString(), dataReader.GetValue(0).ToString());
                    //h.synWithConnnection(cnn2);
                    Console.WriteLine(dataReader.GetValue(0));
                    //HTMLParser h = new HTMLParser("http://www.kaymu.pk/apple-iphone-6-128gb-silver-1901422.html","1");
                    //HTMLParser h = new HTMLParser("http://www.shophive.com/apple-iphone-6-128gb","1");
                    //HTMLParser h = new HTMLParser("https://www.daraz.pk/iphone-6-128gb-without-face-time-silver-apple-mpg45716.html","1");
                    //HTMLParser h = new HTMLParser("https://homeshopping.pk/products/Apple-iPhone-6-128GB-Space-Gray-Factory-Unlocked-Price-in-Pakistan.html","1");
                    HTMLParser h = new HTMLParser(dataReader.GetValue(2).ToString(), dataReader.GetValue(0).ToString(), null);
                    Accuracy.id = dataReader.GetValue(0).ToString();
                    Accuracy.url = dataReader.GetValue(2).ToString();
                    h.doProcess();
                    Product product = h.getProduct();//just have title
                    ProductPricing productPricing = h.getProductPricing();
                    CRFProcess crfp = new CRFProcess();
                    //crfp.doProcess();
                    if (product.getFullName() == null)
                    {
                        continue;
                    }
                    product = crfp.doProcess(product);
                    product.synWithConnnection(cnn2);
                    productPricing.synWithConnnection(cnn2);
                    Category cat = new Category(h.getLsBreadCrumb());
                    cat.synWithConnnection(cnn2);
                    cat.synWithConnnectionWithProduct(cnn2, productPricing.getProductId());
                    Accuracy.addItemCount();
                    Console.WriteLine("Done with Accuracy:" + Accuracy.getAccuracy());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connection" + ex.ToString());
                Accuracy.addIssue(new Accuracy.Issue("#", "#", ex.ToString()));
            }
            Accuracy.saveFile();
            Console.ReadLine();
        }
    }
}
