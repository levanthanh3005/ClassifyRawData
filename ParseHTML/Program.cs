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
            string connetionString = null;
            SqlConnection cnn;
            connetionString = "Data Source=LEVANTHANH-PC;Initial Catalog=ProManagement;User ID=root;Password=12345678";
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Close();
                cnn.Open();
                Console.WriteLine("Done connection");
                String sql = "select * from SampleRawPage where id = 1;";
                SqlCommand command = new SqlCommand(sql, cnn);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    //Console.WriteLine(dataReader.GetValue(2));
                    //HTMLParser h = new HTMLParser(dataReader.GetValue(2).ToString());
                    //HTMLParser h = new HTMLParser("http://www.kaymu.pk/apple-iphone-6-128gb-silver-1901422.html");
                    //HTMLParser h = new HTMLParser("http://www.shophive.com/apple-iphone-6-128gb");
                    HTMLParser h = new HTMLParser("https://www.daraz.pk/iphone-6-128gb-without-face-time-silver-apple-mpg45716.html");
                    ////HTMLParser h = new HTMLParser("https://homeshopping.pk/products/Apple-iPhone-6-128GB-Space-Gray-Factory-Unlocked-Price-in-Pakistan.html");
                    h.doProcess();
                    Product product = h.getProduct();//just have title
                    ProductPricing productPricing = h.getProductPricing();
                    //Console.WriteLine(">>>"+product.getTitle());
                    CRFProcess crfp = new CRFProcess();
                    //crfp.doProcess();
                    crfp.setPosTag(product.getTitle());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connection" + ex.ToString());
            }
            Console.ReadLine();
        }
    }
}
