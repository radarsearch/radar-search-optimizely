namespace RadarSearchOptimizely.Search.Contracts
{
    public interface IPropertyModel
    {
        string Name { get; set; }
        bool Searchable { get; set; }
        string SearchValue();
    }
}
