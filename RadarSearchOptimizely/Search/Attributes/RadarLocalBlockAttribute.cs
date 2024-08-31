namespace RadarSearchOptimizely.Search.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RadarLocalBlockAttribute : Attribute
    {
        public bool Searchable { get; set; }

        public RadarLocalBlockAttribute()
        {
            Searchable = true;
        }
    }
}
