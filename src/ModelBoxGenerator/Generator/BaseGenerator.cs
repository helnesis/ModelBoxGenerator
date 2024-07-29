namespace ModelBoxGenerator.Generator
{
    internal interface IGenerator 
    {
        Task Generate();
        string ToJson();
    }

}
