using EPiServer.Core;

namespace RadarSearchOptimizely.Search.Models.PropertyModels
{
    public class DatePropertyModel : PropertyModel
    {
        private readonly string _date;

        public DatePropertyModel(PropertyData property, IContent content, bool firstLevel, bool isBlockChild) : base(property, content, firstLevel, isBlockChild)
        {
            _date = string.Empty;
        }

        public override string SearchValue()
        {
            return _date;
        }
    }
}
