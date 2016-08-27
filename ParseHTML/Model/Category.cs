using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Category
{
    private List<String> lsBC;
    private String parentCatId = null;
    public Category(List<String> lsBC)
    {
        this.lsBC = lsBC;
    }
    public void synWithConnnection(SqlConnection cnn)
    {
        if (lsBC.Count<2)
        {
            //no category
            return;
        }
        Console.WriteLine("synWithConnnection in Category");
        int i = 0;
        SqlDataReader dataReader = null;
        while (i < lsBC.Count - 1 && parentCatId == null)
        {
            String sql = "select * from dbo.Category where CatName=@CatName";
            SqlCommand command = new SqlCommand(sql, cnn);
            command.Parameters.AddWithValue("@CatName", lsBC[i]);
            dataReader = command.ExecuteReader();
            if (!dataReader.Read())
            {
                dataReader.Close();
                break;
            }
            dataReader.Close();
            i++;
        }
        if (dataReader!=null) dataReader.Close();
        Console.WriteLine("pass p1");
        while (i < lsBC.Count - 1)
        {
            String sql = "Insert into dbo.Category " +
                "(CatName,ParentCatId) values " +
                "(@CatName,@ParentCatId);";
            SqlCommand command = new SqlCommand(sql, cnn);
            command.Parameters.AddWithValue("@CatName", lsBC[i]);
            if (parentCatId==null)
            {
                parentCatId = "-1";
            }
            command.Parameters.AddWithValue("@ParentCatId", parentCatId);
            int result = command.ExecuteNonQuery();

            sql = "select MAX(id) from dbo.Category;";
            command = new SqlCommand(sql, cnn);
            dataReader = command.ExecuteReader();
            dataReader.Read();
            Console.WriteLine("ID:" + dataReader.GetValue(0));
            parentCatId = dataReader.GetValue(0).ToString();
            dataReader.Close();
            i++;
        }
    }
    public void synWithConnnectionWithProduct(SqlConnection cnn, int productId)
    {
        if (parentCatId == null)
        {
            parentCatId = "-1";
        }
        String sql = "Insert into dbo.CategoryProduct " +
    "(ProductId,CategoryId,IsReviewed,IsDeleted,Priority) values " +
    "(@ProductId,@CategoryId,0,0,0);";
        SqlCommand command = new SqlCommand(sql, cnn);
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@CategoryId", parentCatId);
        int result = command.ExecuteNonQuery();
    }
}

