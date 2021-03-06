﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Product
{

    private String id;
    private String uid;
    private String Title;
    private String Keywords;
    private String Description;
    private String Brand;
    private String CreatedDate;
    private String ModifiedDate;
    private int IsReviewed;
    private int IsReported;
    private int IsDeleted;
    private int IsFeatured;
    private int ClickCount;
    private String Priority;
    private String ExtraCol1;
    private String ExtraCol2;
    private String Thumbnail;
    private String Image;
    private String fullName;

    public Product()
    {
        setCreatedDate(getDateNow());
        setModifiedDate(getDateNow());
    }
    public String getDateNow()
    {
        DateTime myDateTime = DateTime.Now;
        return myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }
    public Product(String id, String uid, String Title, String Keywords, String Description, String Brand, String CreatedDate, String ModifiedDate, int IsReviewed, int IsReported, int IsDeleted, int IsFeatured, int ClickCount, String Priority, String ExtraCol1, String ExtraCol2, String Thumbnail, String Image)
    {
        this.id = id;
        this.uid = uid;
        this.Title = Title;
        this.Keywords = Keywords;
        this.Description = Description;
        this.Brand = Brand;
        this.CreatedDate = CreatedDate;
        this.ModifiedDate = ModifiedDate;
        this.IsReviewed = IsReviewed;
        this.IsReported = IsReported;
        this.IsDeleted = IsDeleted;
        this.IsFeatured = IsFeatured;
        this.ClickCount = ClickCount;
        this.Priority = Priority;
        this.ExtraCol1 = ExtraCol1;
        this.ExtraCol2 = ExtraCol2;
        this.Thumbnail = Thumbnail;
        this.Image = Image;
    }

    public String getId()
    {
        return id;
    }

    public void setId(String id)
    {
        this.id = id;
    }

    public String getUid()
    {
        return uid;
    }

    public void setUid(String uid)
    {
        this.uid = uid;
        if (uid.Length==0)
        {
            Accuracy.addIssue(new Accuracy.Issue(Accuracy.id, Accuracy.url, "No Uid"));
        }
    }

    public String getTitle()
    {
        return Title;
    }

    public void setTitle(String Title)
    {
        this.Title = Title;
    }
    public String getFullName()
    {
        return fullName;
    }

    public void setFullName(String fullName)
    {
        this.fullName = fullName;
    }
    public String getKeywords()
    {
        return Keywords;
    }

    public void setKeywords(String Keywords)
    {
        this.Keywords = Keywords;
    }

    public String getDescription()
    {
        return Description;
    }

    public void setDescription(String Description)
    {
        this.Description = Description;
    }

    public String getBrand()
    {
        return Brand;
    }

    public void setBrand(String Brand)
    {
        this.Brand = Brand;
        if (Brand.Length == 0)
        {
            Accuracy.addIssue(new Accuracy.Issue(Accuracy.id, Accuracy.url, "No Brand"));
        }
    }

    public String getCreatedDate()
    {
        return CreatedDate;
    }

    public void setCreatedDate(String CreatedDate)
    {
        this.CreatedDate = CreatedDate;
    }

    public String getModifiedDate()
    {
        return ModifiedDate;
    }

    public void setModifiedDate(String ModifiedDate)
    {
        this.ModifiedDate = ModifiedDate;
    }

    public int getIsReviewed()
    {
        return IsReviewed;
    }

    public void setIsReviewed(int IsReviewed)
    {
        this.IsReviewed = IsReviewed;
    }

    public int getIsReported()
    {
        return IsReported;
    }

    public void setIsReported(int IsReported)
    {
        this.IsReported = IsReported;
    }

    public int getIsDeleted()
    {
        return IsDeleted;
    }

    public void setIsDeleted(int IsDeleted)
    {
        this.IsDeleted = IsDeleted;
    }

    public int getIsFeatured()
    {
        return IsFeatured;
    }

    public void setIsFeatured(int IsFeatured)
    {
        this.IsFeatured = IsFeatured;
    }

    public int getClickCount()
    {
        return ClickCount;
    }

    public void setClickCount(int ClickCount)
    {
        this.ClickCount = ClickCount;
    }

    public String getPriority()
    {
        return Priority;
    }

    public void setPriority(String Priority)
    {
        this.Priority = Priority;
    }

    public String getExtraCol1()
    {
        return ExtraCol1;
    }

    public void setExtraCol1(String ExtraCol1)
    {
        this.ExtraCol1 = ExtraCol1;
    }

    public String getExtraCol2()
    {
        return ExtraCol2;
    }

    public void setExtraCol2(String ExtraCol2)
    {
        this.ExtraCol2 = ExtraCol2;
    }

    public String getThumbnail()
    {
        return Thumbnail;
    }

    public void setThumbnail(String Thumbnail)
    {
        this.Thumbnail = Thumbnail;
    }

    public String getImage()
    {
        return Image;
    }

    public void setImage(String Image)
    {
        this.Image = Image;
    }
    public void setAutoUniqueId()
    {
        setUid(Guid.NewGuid().ToString());
    }
    /// <summary>
    /// Syn data with sql server
    /// </summary>
    /// <param name="cnn"></param>
    public void synWithConnnection(SqlConnection cnn)
    {
        try
        {
            //cnn.Close();
            //cnn.Open();
            Console.WriteLine("synWithConnnection:>" + this.getImage() + "<");
            //Console.WriteLine(this.getUid()+" "+ this.getUid().Length);
            //Console.WriteLine(this.getTitle() + " " + this.getTitle().Length);
            //Console.WriteLine(this.getKeywords() + " " + this.getKeywords().Length);
            //Console.WriteLine(this.getDescription() + " " + this.getDescription().Length);
            //Console.WriteLine(this.getBrand() + " " + this.getBrand().Length);
            //Console.WriteLine(this.getModifiedDate() + " " + this.getModifiedDate().Length);
            //Console.WriteLine(this.getCreatedDate() + " " + this.getCreatedDate().Length);
            //Console.WriteLine(this.getImage() + " " + this.getImage().Length);
            String sql = "Insert into dbo.Product " +
                "(UId ,Title ,Keywords ,Description ,Brand ,CreatedDate ,ModifiedDate ,IsReviewed ,IsReported ,IsDeleted ,IsFeatured ,ClickCount ,Priority ,ExtraCol1 ,ExtraCol2 ,Thumbnail ,Image) values " +
                "(@UId,@Title,@Keywords,@Description,@Brand,@CreatedDate,@ModifiedDate,0,0,0,0,0,null,null,null,null,@Image);";
            SqlCommand command = new SqlCommand(sql, cnn);
            command.Parameters.AddWithValue("@UId", this.getUid());
            command.Parameters.AddWithValue("@Title", this.getTitle());
            command.Parameters.AddWithValue("@Keywords", this.getKeywords());
            command.Parameters.AddWithValue("@Description", this.getDescription());
            command.Parameters.AddWithValue("@Brand", this.getBrand());
            command.Parameters.AddWithValue("@ModifiedDate", this.getModifiedDate());
            command.Parameters.AddWithValue("@CreatedDate", this.getCreatedDate());
            command.Parameters.AddWithValue("@Image", this.getImage());
            int result = command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Accuracy.addWrongItemCount();
            Accuracy.addIssue(new Accuracy.Issue(Accuracy.id, Accuracy.url, "synWithConnnection in product " + e.ToString()));
            Console.WriteLine("Error:" + e.ToString());
        }
    }
}
