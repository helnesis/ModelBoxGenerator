namespace ModelBoxGenerator.Types
{
    internal readonly record struct ModelBox(CAaBox BoundingBox, float BoundingBoxRadius, CAaBox CollisionBox, float CollisionBoxRadius);
}
