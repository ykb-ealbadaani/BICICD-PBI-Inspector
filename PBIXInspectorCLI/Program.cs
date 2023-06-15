﻿using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Utils;
using System.Reflection;

internal partial class Program
{
    private static void Main(string[] args)
    {
        const string SamplePBIXFilePath = @"Files\Inventory sample.pbix";
        const string SampleRulesFilePath = @"Files\Inventory rules sample.json";
        const bool Verbose = true;
        

        Inspector? insp = null;

        Welcome();

        try
        {
            var parsedArgs = CLIArgsUtils.ParseArgs(args);
            insp = RunInspector(parsedArgs.PBIXFilePath, parsedArgs.RulesFilePath, parsedArgs.Verbose);
        }
        catch (ArgumentNullException)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nRunning with sample files for demo purposes.\nSample .pbix file is at \"{0}\".\nSample inspection rules json file is at \"{1}\".\n", Path.Combine(AppContext.BaseDirectory, SamplePBIXFilePath), Path.Combine(AppContext.BaseDirectory, SampleRulesFilePath));
            Console.ResetColor();
            insp = RunInspector(SamplePBIXFilePath, SampleRulesFilePath, Verbose);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            if (insp != null)
            {
                insp.MessageIssued -= Insp_MessageIssued;
            }
            Console.WriteLine("\nPress any key to quit application.");
            Console.ReadLine();
        }
    }

    private static void Welcome()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("PBIX Inspector v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        Console.ResetColor();
    }

    private static Inspector RunInspector(string PBIXFilePath, string RulesFilePath, bool Verbose)
    {
        Inspector insp = new Inspector(PBIXFilePath, RulesFilePath);
        insp.MessageIssued += Insp_MessageIssued;

        try
        {
            var testResults = insp.Inspect();

            if (!Verbose) { Console.WriteLine("Verbose param is set to false so only list test failures."); }
            foreach (var result in testResults)
            {
                if (result.Result && !Verbose) continue; //skip if verbose is false
                Console.WriteLine();
                Console.ForegroundColor = result.Result ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(result.ResultMessage);
            }
        }
        catch (PBIXInspectorException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            Console.ResetColor();
        }

        return insp;
    }

    private static void Insp_MessageIssued(object? sender, MessageIssuedEventArgs e)
    {
        Console.WriteLine("{0}: {1}", e.MessageType.ToString(), e.Message);
    }
}