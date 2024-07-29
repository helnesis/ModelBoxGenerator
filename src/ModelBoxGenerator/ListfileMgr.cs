using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Collections.Frozen;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ModelBoxGenerator
{
    internal readonly record struct ListfileRecord([property: Index(0)] uint FileDataId, 
                                                   [property: Index(1)] string FilePath);
    internal sealed partial class ListfileMgr
    {
        private static readonly Lazy<ListfileMgr> _instance = new(() => new ListfileMgr());
        public static ListfileMgr Instance => _instance.Value;

        const string WmoGroup = "^.*(_[0-9]{3}(lod[0-9])?([0-9]+)?)|_lod[0-9].wmo$";

        private readonly Dictionary<uint, string> _listfileContent = [];

        private bool _isLoaded = false;
        public void Initialize(Stream listfileStream)
        {
            if (_isLoaded)
                return;

            using var reader = new StreamReader(listfileStream);
            using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) 
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            });


            while(csvReader.Read())
            {
                var record = csvReader.GetRecord<ListfileRecord>();

                // Keep only M2 and WMO files thar are not group WMOs.
                if (record.FilePath.EndsWith(".m2") || (record.FilePath.EndsWith(".wmo") && !GroupWmoPattern().IsMatch(record.FilePath)))
                {
                    _listfileContent.TryAdd(record.FileDataId, record.FilePath);
                }
            }
        }

        public FrozenDictionary<uint, string> Assets => _listfileContent.ToFrozenDictionary();


        [GeneratedRegex(WmoGroup)]
        private static partial Regex GroupWmoPattern();
    }
}
