using System;
using System.IO;
using System.Linq;
using System.Text;

public static class Program
{
    private static readonly string srcFile = "methods.txt";
    private static readonly string dstFile = "MathF.cs";

    public static void Main()
    {
        using (var writer = new StreamWriter(dstFile))
        {
            writer.WriteLine("namespace System");
            writer.WriteLine("{");
            writer.WriteLine("    public static class MathF");
            writer.WriteLine("    {");
            foreach (var line in File.ReadLines(srcFile))
            {
                if (IsDoubleConst(line))
                {
                    writer.WriteLine("        " + GetFloatConst(line));
                }

                if (IsDoubleMethod(line))
                {
                    writer.WriteLine("        " + GetFloatMethod(line));
                }
            }
            writer.WriteLine("    }");
            writer.WriteLine("}");
        }
    }

    private static bool IsDoubleConst(string line)
    {
        line = line.Trim();

        if (!line.StartsWith("public"))
        {
            return false;
        }

        if (line.Contains("const"))
        {
            return true;
        }

        return false;
    }

    private static bool IsDoubleMethod(string line)
    {
        line = line.Trim();

        if (!line.StartsWith("public"))
        {
            return false;
        }

        if (line.Contains("const"))
        {
            return false;
        }

        if (line.Contains("double"))
        {
            return true;
        }

        return false;
    }

    private static string GetFloatConst(string line)
    {
        return line.Trim().Replace("double", "float").Replace(";", "F;");
    }

    private static string GetMethodName(string line)
    {
        var end = line.IndexOf('(');
        var tmp = line.Substring(0, end).Split(' ');
        return tmp[tmp.Length - 1];
    }

    private static string GetReturnType(string line)
    {
        var end = line.IndexOf('(');
        var tmp = line.Substring(0, end).Split(' ');
        return tmp[tmp.Length - 2];
    }

    private static Tuple<string, string>[] GetMethodArgs(string line)
    {
        var start = line.IndexOf('(') + 1;
        var end = line.IndexOf(')');
        var length = end - start;

        var args = line.Substring(start, length).Split(new string[] { ", " }, StringSplitOptions.None);

        return args.Select(x =>
        {
            var split = x.Split(' ');
            return Tuple.Create(split[0], split[1]);
        }).ToArray();
    }

    private static string GetFloatMethod(string line)
    {
        var name = GetMethodName(line);
        var type = GetReturnType(line);
        var args = GetMethodArgs(line);

        var sb = new StringBuilder();
        sb.Append("public static ");
        sb.Append(type == "double" ? "float" : type);
        sb.Append(" ");
        sb.Append(name);
        sb.Append("(");

        var last = args.Last();

        foreach (var arg in args)
        {
            if (arg.Item1 == "double")
            {
                sb.Append("float ");
                sb.Append(arg.Item2);
            }
            else
            {
                sb.Append(arg.Item1);
                sb.Append(" ");
                sb.Append(arg.Item2);
            }

            if (arg != last)
            {
                sb.Append(", ");
            }
        }

        sb.Append(") => ");

        if (type == "double")
        {
            sb.Append("(float)");
        }

        sb.Append("Math.");
        sb.Append(name);
        sb.Append("(");

        foreach (var arg in args)
        {
            sb.Append(arg.Item2);

            if (arg != last)
            {
                sb.Append(", ");
            }
        }

        sb.Append(");");

        return sb.ToString();
    }
}
