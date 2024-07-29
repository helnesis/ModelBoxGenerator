using ModelBoxGenerator.Types;
using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;

namespace ModelBoxGenerator.Generator
{
    /// <summary>
    /// Read M2 file, parse bounding boxes and vertices box.
    /// </summary>
    internal sealed class ModelGenerator : IGenerator
    {
        const int M2BoundingBoxOffset = 0xA8;
        const int WmoBoundingBoxOffset = 0x58;
        const int WmoSig = 0x4D564552;

        private readonly ConcurrentDictionary<uint, CAaBox> _modelBoundingBoxes = [];

        private static bool IsWmoFile(uint sig)
            => sig.Equals(WmoSig);

        public async Task Generate()
        {
            var files = ListfileMgr.Instance.Assets;

            await Parallel.ForEachAsync(files, StaticSettings.ParallelDefault, async (kvp, cancellationToken) =>
            {
                await using var stream = CascMgr.Instance.OpenFile(kvp.Key);
                CAaBox bbox = new();

                if (stream is not null)
                {
                    using var reader = new BinaryReader(stream);

                    if (IsWmoFile(reader.ReadUInt32()))
                    {
                        reader.BaseStream.Position = WmoBoundingBoxOffset;

                        bbox = new(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                                      new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                    }
                    else
                    {
                        reader.BaseStream.Position = M2BoundingBoxOffset;

                        ModelBox modelBox = new()
                        {
                            BoundingBox = new CAaBox(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                                     new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())),

                            BoundingBoxRadius = reader.ReadSingle(),

                            CollisionBox = new CAaBox(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                          new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())),

                            CollisionBoxRadius = reader.ReadSingle()
                        };


                        bbox =
                        modelBox.BoundingBox.BottomCorner == Vector3.Zero && modelBox.BoundingBox.TopCorner == Vector3.Zero
                            ? modelBox.CollisionBox
                            : modelBox.BoundingBox;
                    }

                    _modelBoundingBoxes.TryAdd(kvp.Key, bbox);
                    reader.BaseStream.Position = 0;

                }
            });
        }

        public string ToJson()
            => JsonSerializer.Serialize(_modelBoundingBoxes, StaticSettings.JsonDefault);
    }
}
