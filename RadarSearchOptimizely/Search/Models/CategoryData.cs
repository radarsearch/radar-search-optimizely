using System.Runtime.Serialization;

namespace RadarSearchOptimizely.Search.Models
{
    [Serializable]
    [DataContract]
    public class CategoryData
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }

        public CategoryData()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        public override string ToString()
        {
            return Id + " # " + Name + " # " + Description;
        }

        public static CategoryData GetCategoryDataFromString(string categorystring)
        {
            var members = categorystring.Split('#');
            if (members.Length != 3)
                return null;

            int id;
            var result = int.TryParse(members[0].Trim(' '), out id);
            if (!result)
                return null;

            return new CategoryData
            {
                Id = id,
                Name = members[1].Trim(' '),
                Description = members[2].Trim(' ')
            };
        }
    }
}
