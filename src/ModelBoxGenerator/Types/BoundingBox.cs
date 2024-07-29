using System.Numerics;

namespace ModelBoxGenerator.Types
{
    /// <summary>
    /// A structure representing an axis-aligned bounding box, comprised of two <see cref="Vector3"/> objects
    /// defining the bottom and top corners of the box.
    /// </summary>
    internal readonly record struct CAaBox(Vector3 BottomCorner, Vector3 TopCorner);
}    


