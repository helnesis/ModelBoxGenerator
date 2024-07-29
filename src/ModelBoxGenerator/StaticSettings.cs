using System.Text.Json;

namespace ModelBoxGenerator
{
    internal sealed class StaticSettings
    {
        public static JsonSerializerOptions JsonDefault =>
            new()
            {
                WriteIndented = true,
                IncludeFields = true,
            };


        public static ParallelOptions ParallelDefault =>
         new()
          {
             MaxDegreeOfParallelism = Environment.ProcessorCount
          };
    }
}
