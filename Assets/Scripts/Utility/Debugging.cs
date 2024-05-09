using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Windows;

public static class Debugging
{
    public static event EventHandler<string[]> SetIp;
    public static event EventHandler<string[]> SetPort; 
    public static event EventHandler<object>ShowIp;

    public struct CommandElement
    {
        public string[] components;

        public CommandElement(string[] str)
        {
            components = str;
        }

        public static CommandElement FromString(string s)
        {
            string[] str = Regex.Split(s, @"\.");
            for (int i = 0; i < str.Length; i++) 
            {
                str[i] = str[i].Replace("\u200B", "");
            }
            return new CommandElement(str);
            
        }
    }
    public struct Command
    {
        public CommandElement[] elements;

        public Command(CommandElement[] _elements)
        {
            elements = _elements;
        }

        public static Command FromString(string s)
        {
            string[] str_Elements = Regex.Split(s, @"\s+");
            CommandElement[] cmd_Elements = new CommandElement[str_Elements.Length];
            for (int i = 0; i < str_Elements.Length; i++)
            {
                cmd_Elements[i] = CommandElement.FromString(str_Elements[i]);
            }

            return new Command(cmd_Elements);
        }
    }



    public static bool inEditor { private get; set; } = true;

    public static int maxLines { private get; set; }
    public static int currentLine { private get; set; }
    public static TextMeshProUGUI debugTextTMP { private get; set; }
    public static GameObject debugPanel { private get; set; }

    public static void Initialize()
    {
        currentLine = maxLines;
    }

    public static void ToggleDeveloperConsole()
    {
        debugPanel.SetActive(!debugPanel.activeInHierarchy);
    }
    public static void ToggleDeveloperConsole(bool val)
    {
        debugPanel.SetActive(val);
    }

    public static void Log(string s)
    {
        if (inEditor)
        {
            Debug.Log(s);
        }
        else
        {
            AddToDevTerminal(s);
        }
    }

    public static void AddToDevTerminal(string s)
    {
        List<string> append_lines = GetLines(s);
        List<string> tmp_lines = GetLines(debugTextTMP.text);
        int total_count = tmp_lines.Count + append_lines.Count;
        bool appendable = total_count <= (maxLines - 1);
        int write_index = (maxLines) - total_count; //if there are 40 lines total the starting index is 59 (60th index) and will wrtie 40 rows from 59-99
        if (appendable)
        {
            tmp_lines.AddRange(append_lines);
            debugTextTMP.text = WriteFrom(tmp_lines, write_index);
        }
        else
        {
            int overflow_count = -write_index; //if the text isnt apprendable the -write_index will tell us how many lines over we are
            for (int i = 0; i < overflow_count; i++)
            {
                tmp_lines.RemoveAt(i);
            }

            tmp_lines.AddRange(append_lines);
            debugTextTMP.text = WriteFrom(tmp_lines, 0);
        }
    }   

    public static void ParseCommand(string s)
    {
        //Log(s);
        Command cmd = Command.FromString(s);
        //Log(cmd.elements[0].components[0]);
       
        switch (cmd.elements[0].components[0]) 
        {
            case "set-ip":
                SetIp?.Invoke(null, cmd.elements[1].components); //Handled in the network manager class
                break;
            case "set-port":
                SetPort?.Invoke(null, cmd.elements[1].components); //Handled in the network manager class
                break;
            case "show-ip":                
                ShowIp?.Invoke(null, null); //Handled in the network manager class
                break;
        }
    }

    public static string WriteFrom(List<string> lines, int index)
    {
        string r_string = "";

        for (int i = 0; i < index; i++)
        {
            r_string += "\n";
        }

        foreach (string line in lines)
        {
            r_string += line + "\n";
        }


        return r_string;
    }

    public static List<string> GetLines(string s)
    {
        List<string> lines = new List<string>();
        using (StringReader reader = new StringReader(s))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
        }
        return lines;
    }
} 


