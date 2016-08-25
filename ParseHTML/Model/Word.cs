using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Word
{
    private String content;
    private String posTag;
    private String rsTag;

    public Word()
    {
    }

    public Word(String content, String posTag, String rsTag)
    {
        this.content = content;
        this.posTag = posTag;
        this.rsTag = rsTag;
    }

    public String getContent()
    {
        return content;
    }

    public void setContent(String content)
    {
        this.content = content;
    }

    public String getPosTag()
    {
        return posTag;
    }

    public void setPosTag(String posTag)
    {
        this.posTag = posTag;
    }

    public String getRsTag()
    {
        return rsTag;
    }

    public void setRsTag(String rsTag)
    {
        this.rsTag = rsTag;
    }

}