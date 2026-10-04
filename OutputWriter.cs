using System.Text;

namespace CSharpLearningProject;

/// <summary>
/// 输出重定向器: 拦截 Console.WriteLine 输出, 转发到 GUI
/// 同时保留控制台输出 (如果有的话)
/// </summary>
public class OutputWriter : StringWriter
{
    private readonly StringBuilder _buffer = new();
    private readonly Action<string>? _onOutput;

    public OutputWriter(Action<string>? onOutput = null)
    {
        _onOutput = onOutput;
    }

    public override void Write(string? value)
    {
        _buffer.Append(value);
        _onOutput?.Invoke(value ?? "");
    }

    public override void WriteLine(string? value)
    {
        _buffer.AppendLine(value);
        _onOutput?.Invoke((value ?? "") + "\n");
    }

    public override void WriteLine()
    {
        _buffer.AppendLine();
        _onOutput?.Invoke("\n");
    }

    public string GetText() => _buffer.ToString();

    public void Clear() => _buffer.Clear();
}
