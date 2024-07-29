using ModelBoxGenerator;
using ModelBoxGenerator.Generator;

const string BlobFile = "blob.json";
const string ListfileUri = "https://github.com/wowdev/wow-listfile/releases/latest/download/community-listfile.csv";


if (File.Exists(BlobFile))
    File.Delete(BlobFile);

using var httpClient = new HttpClient();
using var listfileStream = await httpClient.GetStreamAsync(ListfileUri);

CascMgr.Instance
    .Initialize();

ListfileMgr.Instance
    .Initialize(listfileStream);



ModelGenerator generator = new();
await generator.Generate();

File.WriteAllText(BlobFile, generator.ToJson());


