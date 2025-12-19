namespace Shell.Core;

public class TypeWriter
{
    private readonly int _baseDelay;

    public TypeWriter(int baseDelay = 250)
    {
        _baseDelay = baseDelay;
    }

    public async Task WriteAsync(string text)
    {
        int i = 0;

        while (i < text.Length)
        {
            char c = text[i];
            
            if (char.IsWhiteSpace(c))
            {
                Console.Write(c);
                i++;
                await Task.Delay(_baseDelay);
                continue;
            }
            if (c == '.' || c == ',' || c == '\n')
            {
                Console.Write(c);
                i++;
                await Task.Delay(_baseDelay * 3);
                continue;
            }

            if (c == '-')
            {
                int dashCount = 0;

                // zähle aufeinanderfolgende '-'
                while (i < text.Length && text[i] == '-')
                {
                    dashCount++;
                    i++;
                }

                await Task.Delay(_baseDelay * dashCount);
                continue;
            }

            Console.Write(c);
            i++;
        }
    }
}
