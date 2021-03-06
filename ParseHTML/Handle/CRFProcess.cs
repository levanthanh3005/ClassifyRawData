﻿using java.io;
using java.util;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.tagger.maxent;
using Console = System.Console;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

public class CRFProcess
{
    public CRFProcess()
    {
        // crf_test -m model testdata.dat
    }
    /// <summary>
    /// This function will set lable for product fullname, after that, brand name and product name will be detected
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    public Product doProcess(Product product)
    {
        List<Word> lsW = setPosTag(product.getFullName());
        writeFile(lsW);
        lsW = setCRFTag(lsW);
        String keyword = "";
        String UId = "";
        List<String> lsCt = new List<string>();
        String brand = "";
        foreach (Word w in lsW)
        {
            if (!w.getRsTag().Contains("O"))
            {
                keyword = keyword + w.getContent() + ",";
            }
            if (w.getRsTag().Contains("PDN"))
            {
                //UId = UId + w.getContent() + "-";
                lsCt.Add(w.getContent());
            }
            if (w.getRsTag().Contains("BN"))
            {
                brand = brand + w.getContent();
            }
        }
        lsCt.Sort();
        foreach(String s in lsCt)
        {
            UId = UId + s + "-";
        }
        //Console.ReadLine();
        Console.WriteLine("keyword:" + keyword + " >UID " + UId + " brand:"+brand);
        product.setDescription(product.getTitle());
        product.setUid(UId.TrimEnd('-'));
        //product.setKeywords(keyword);
        product.setBrand(brand);
        return product;
    }
    /// <summary>
    /// Using CRF tool to set lable for each Word in lsW, this is the list of lable:
    /// PDN: product's name, PDT: product's type, BN: brand's name, O: outside
    /// </summary>
    /// <param name="lsW"></param>
    /// <returns></returns>
    public List<Word> setCRFTag(List<Word> lsW)
    {
        string output = string.Empty;
        string error = string.Empty;

        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c cd ../../CRF && crf_test -m model testdata.dat");
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
        processStartInfo.UseShellExecute = false;

        Process process = Process.Start(processStartInfo);

        using (StreamReader streamReader = process.StandardOutput)
        {
            output = streamReader.ReadToEnd();
        }

        using (StreamReader streamReader = process.StandardError)
        {
            error = streamReader.ReadToEnd();
        }
        String[] lsLine = output.Split('\n');
        String sf = "";
        for(int i = 0;i<lsW.Count;i++)
        {
            Console.WriteLine(">>>"+lsLine[i]+"<");
            //Console.WriteLine("$$>>>" + lsW[i].getContent() + "<");
            lsW[i].setRsTag(lsLine[i].Split('\t')[2]);
            sf = sf + lsLine[i] + "\n";
        }
        Accuracy.addIssue(new Accuracy.Issue(Accuracy.id,Accuracy.url, sf));
        Console.WriteLine("Done CRF");
        return lsW;
    }
    /// <summary>
    /// Using Parts of speech tool to detect the word form, more information in http://parts-of-speech.info/, and you can see in CRF/Explaination.dat
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public List<Word> setPosTag(String text)
    {
        String[] lst= Regex.Split(text, "([0-9]+)");
        text = "";
        foreach(String s in lst)
        {
            text = text + s + " ";
        }

        List<Word> lsW = new List<Word>();
        var jarRoot = @"..\..\CRF\stanford-postagger-full-2015-12-09";
        var modelsDirectory = jarRoot + @"\models";

        // Loading POS Tagger
        var tagger = new MaxentTagger(modelsDirectory + @"\wsj-0-18-bidirectional-nodistsim.tagger");

        var sentences = MaxentTagger.tokenizeText(new java.io.StringReader(text)).toArray();
        foreach (ArrayList sentence in sentences)
        {
            var taggedSentence = tagger.tagSentence(sentence);
            foreach(TaggedWord s in taggedSentence.toArray())
            {
                Word w = new Word();
                w.setContent(s.word());
                w.setPosTag(s.tag());
                //Console.WriteLine(">>" + s.word());
                //Console.WriteLine(">>" + s.tag());
                lsW.Add(w);
            }
        }
        Console.WriteLine("Done PosTag");
        //Console.ReadLine();
        return lsW;
    }
    /// <summary>
    /// Write file into CRF/testdata.dat as the input for CRF++
    /// </summary>
    /// <param name="lsW"></param>
    public void writeFile(List<Word> lsW)
    {
        Console.WriteLine("Start Writefile for CRF");
        StreamWriter file = new StreamWriter(@"../../CRF/testdata.dat");
        foreach (Word w in lsW)
        {
            // If the line doesn't contain the word 'Second', write the line to the file.
            String tag = w.getPosTag();
            if (tag.StartsWith("NN"))
            {
                tag = "NN";
            }
            //tag = Regex.Replace(tag, "[^0-9a-zA-Z]+", "");
            file.WriteLine(w.getContent()+" "+tag);
        }
        file.Close();
        Console.WriteLine("Done Writefile for CRF");
    }
    /*crf_learn template train.dat model
     * PDN: Product name
     * PDT: Produc type
     * O: outside
     * BN: Brand name
     */
}