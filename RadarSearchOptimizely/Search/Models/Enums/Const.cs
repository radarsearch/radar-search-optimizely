namespace RadarSearchOptimizely.Search.Models.Enums
{
    public static class Const
    {
        public const string Languagefield = "Language";//
        public const string ContentIdField = "ContentId";//
        public const string AccessControlField = "Acl"; //
        public const string TypeField = "Type";//
        public const string IdField = "Id";//
        public const string GuidField = "Guid";//
        public const string TitleField = "Title"; //
        public const string DescriptionField = "Description"; //
        public const string TextField = "Text"; //
        public const string ImageField = "Image"; //
        public const string UrlField = "Url"; //
        public const string RelativeUrlField = "RelativeUrlField"; //
        public const string CreatedField = "Created"; //
        public const string UpdatedField = "Updated"; //
        public const string DeletedField = "Deleted"; //
        public const string HideField = "Hide"; //
        public const string UniqueField = "Unique"; //
        public const string Category = "Category"; //
        public const string Keywords = "Keywords"; //

        public static readonly string[] MultiField = { IdField, GuidField, TypeField, DescriptionField, TextField, UrlField, RelativeUrlField, CreatedField, UpdatedField, DeletedField, HideField, UniqueField, TitleField, ImageField, Category, Keywords, ContentIdField };
        public static readonly string[] FieldBoostNames = { "BoostA", "BoostB", "BoostC", "BoostD", "BoostE", "BoostF" };

        public const string LuceneFolder = "radar";
        public const string QueueFolder = "queue";
        public const string RadarSettingsFolder = "radarsettings";
        public const string GlobalLanguage = "global";
        public const string RadarExtension = ".bin";
        public const int NumberOfFilesInCollection = 100;

        public const string RadarwordOr = "radaror";
        public const string RadarwordAnd = "radarand";
        public const char AclSpecial = 'x';

        public static readonly string[] LuceneStopwords =
        {
            "a", "an", "and", "are", "as", "at", "be", "but", "by", "for",
            "if", "in", "into", "is", "it", "no", "not", "of", "on", "or", "such", "that", "the", "their", "then",
            "there", "these", "they", "this", "to", "was", "will", "with"
        };
    }
}
