#!/usr/bin/env dotnet-script

using System.Reflection;
using System.IO;
using System;

//Expected arguments: dotnet-script.dll get-assembly-version.dll ASSEMBLY_FILE
string[] args = Environment.GetCommandLineArgs();
if (args.Count() != 3)
{
    Console.Error.WriteLine($"Usage: {args[0]} ASSEMBLY_FILE");
    Environment.Exit(1);
}
string path = args[2];

try
{
    AssemblyName assembly = AssemblyName.GetAssemblyName(path);
    Console.WriteLine(assembly.Version);
}
catch(Exception e)
{
    Console.Error.WriteLine($"Error reading from {path}:\n{e}");
    Environment.Exit(2);
}
