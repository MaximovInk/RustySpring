using System;

namespace MaximovInk
{
    public class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Func<string[], string> Execute { get; set; }
    }
}