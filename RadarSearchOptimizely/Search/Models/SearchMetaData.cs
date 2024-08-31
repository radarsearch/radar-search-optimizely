using System.Runtime.Serialization;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Models.Enums;
using Index = RadarSearchOptimizely.Search.Models.Enums.Index;

namespace RadarSearchOptimizely.Search.Models
{
    [DataContract]
    [Serializable]
    public class SearchMetaData : ISearchMetaData
    {
        [DataMember]
        public string FieldName { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public float Boost { get; set; }

        [DataMember]
        public Index FieldIndex { get; set; }

        [DataMember]
        public Store FieldStore { get; set; }

        [DataMember]
        public int ExtremeBoost
        {
            get
            {
                return _extremeBoost;
            }
            set
            {
                if (value >= 6)
                    _extremeBoost = 6;
                else if (value < 1)
                    _extremeBoost = 1;
                else
                    _extremeBoost = value;
            }
        }

        private int _extremeBoost;

        public SearchMetaData()
        {
            FieldName = string.Empty;
            Value = string.Empty;
            Boost = 0.0f;
            FieldIndex = Index.ANALYZED;
            FieldStore = Store.YES;
            ExtremeBoost = 1;
        }
    }
}
