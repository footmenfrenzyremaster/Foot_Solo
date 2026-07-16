using System.Text;
using War3Net.CodeAnalysis.Jass;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: JassParseCheck <script.j> [more scripts...]");
    return 2;
}

var failed = false;
foreach (var arg in args)
{
    var path = Path.GetFullPath(arg);
    var text = File.ReadAllText(path, Encoding.Latin1);
    try
    {
        var unit = JassSyntaxFactory.ParseCompilationUnit(text);
        Console.WriteLine($"OK   {path} ({unit.Declarations.Length:n0} top-level declarations)");
    }
    catch (Exception ex)
    {
        failed = true;
        Console.WriteLine($"FAIL {path}: {ex.Message}");
    }
}

return failed ? 1 : 0;
