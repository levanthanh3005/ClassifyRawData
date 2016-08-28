using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class HTMLLoadData
{
    private String htmlLink;//its url
    private String urlId;//its url
    public HTMLLoadData(String htmlLink, String urlId)
    {
        this.htmlLink = htmlLink;
        this.urlId = urlId;
    }
    public void synWithConnnection(SqlConnection cnn)
    {
        HtmlWeb htmlWeb = new HtmlWeb()
        {
            AutoDetectEncoding = false,
            OverrideEncoding = Encoding.UTF8  //Set UTF8 để hiển thị tiếng Việt
        };

        //Load web
        HtmlDocument document = htmlWeb.Load(this.htmlLink);

        Console.WriteLine("synWithConnnection for :"+this.urlId);
        String sql = "Update dbo.SampleRawPage " +
            "set contents=@contents " +
            "where id=@id";
        SqlCommand command = new SqlCommand(sql, cnn);
        command.Parameters.AddWithValue("@id", this.urlId);
        command.Parameters.AddWithValue("@contents", document.DocumentNode.InnerHtml);
        int result = command.ExecuteNonQuery();
        Console.WriteLine("Done for:"+this.urlId);
    }
}
