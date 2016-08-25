using java.io;
using java.util;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.tagger.maxent;
using Console = System.Console;
using System.Collections.Generic;
using System;

public class CRFProcess
{
    public CRFProcess()
    {
        // crf_test -m model testdata.dat
    }
    public List<Word> doProcess(List<Word> lsWord)
    {
        //string output = string.Empty;
        //string error = string.Empty;

        //ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c cd ../../CRF && crf_test -m model testdata.dat");
        //processStartInfo.RedirectStandardOutput = true;
        //processStartInfo.RedirectStandardError = true;
        //processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
        //processStartInfo.UseShellExecute = false;

        //Process process = Process.Start(processStartInfo);

        //using (StreamReader streamReader = process.StandardOutput)
        //{
        //    output = streamReader.ReadToEnd();
        //}

        //using (StreamReader streamReader = process.StandardError)
        //{
        //    error = streamReader.ReadToEnd();
        //}

        //Console.WriteLine("The following output was detected:");
        //Console.WriteLine(output);
        //String[] lsLine = output.Split('\n');
        //foreach(String s in lsLine)
        //{
        //    Console.WriteLine("=>"+s);
        //}
        return lsWord;
    }
    public List<Word> setPosTag(String text)
    {
        List<Word> lsW = new List<Word>();
        var jarRoot = @"..\..\CRF\stanford-postagger-full-2015-12-09";
        var modelsDirectory = jarRoot + @"\models";

        // Loading POS Tagger
        var tagger = new MaxentTagger(modelsDirectory + @"\wsj-0-18-bidirectional-nodistsim.tagger");

        // Text for tagging
        //text = "A Part-Of-Speech Tagger (POS Tagger) is a piece of software that reads text"
        //           + "in some language and assigns parts of speech to each word (and other token),"
        //           + " such as noun, verb, adjective, etc., although generally computational "
        //           + "applications use more fine-grained POS tags like 'noun-plural'.";

        var sentences = MaxentTagger.tokenizeText(new java.io.StringReader(text)).toArray();
        Console.WriteLine("Set Pos Tag");
        foreach (ArrayList sentence in sentences)
        {
            var taggedSentence = tagger.tagSentence(sentence);
            Console.WriteLine(Sentence.listToString(taggedSentence, false));
        }
        return lsW;
    }
}