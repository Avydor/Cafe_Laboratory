using System;
using System.IO;

class Generator
{
    static void Main()
    {
        using (StreamWriter writer = new StreamWriter("CrazyCalculator.cs"))
        {
            writer.WriteLine("using System;");
            writer.WriteLine("class Program {");
            writer.WriteLine("  static void Main() {");
            writer.WriteLine("    Console.Write(\"Число 1: \"); int a = int.Parse(Console.ReadLine());");
            writer.WriteLine("    Console.Write(\"Число 2: \"); int b = int.Parse(Console.ReadLine());");

            bool first = true;
            for (int i = 1; i <= 30; i++)
            {
                for (int j = 1; j <= 30; j++)
                {
                    string keyword = first ? "if" : "else if";
                    writer.WriteLine($"    {keyword} (a == {i} && b == {j}) Console.WriteLine(\"Результат: {i + j}\");");
                    first = false;
                }
            }

            writer.WriteLine("    else Console.WriteLine(\"Числа должны быть от 1 до 30!\");");
            writer.WriteLine("  }");
            writer.WriteLine("}");
        }
        Console.WriteLine("Файл CrazyCalculator.cs успешно создан!");
    }
}