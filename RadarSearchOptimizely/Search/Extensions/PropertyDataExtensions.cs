using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using RadarSearchOptimizely.Search.Contracts;
using RadarSearchOptimizely.Search.Models.PropertyModels;

namespace RadarSearchOptimizely.Search.Extensions
{
    public static class PropertyDataExtensions
    {
        public static bool IsSearchable(this PropertyData propertyData, IContent content)
        {
            const string debugNamespace = "EPi.Common.Extensions.PropertyDataExtensions.IsSearchable";
            var propertyDefinitionRepository = ServiceLocator.Current.GetInstance<IPropertyDefinitionRepository>();
            if (propertyDefinitionRepository == null)
                throw new NullReferenceException(debugNamespace + " PropertyDefinitionRepository is null");
            var propertyDefinitions = propertyDefinitionRepository.List(content.ContentTypeID);
            if (propertyDefinitions == null)
                throw new NullReferenceException(debugNamespace + " Propertydefinitions is null");
            return propertyDefinitions.Any(propertyDefinition => propertyDefinition.ID.Equals(propertyData.PropertyDefinitionID) && propertyDefinition.Searchable);
        }

        public static IPropertyModel CreatePropertyModel(this PropertyData propertyData, IContent content, bool firstLevel, bool isBlockChild)
        {
            var propertyType = propertyData.PropertyValueType;

            switch (propertyData.Type)
            {
                case PropertyDataType.Category:
                    return new CategoryPropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.Block:
                    return new BlockPropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.Boolean:
                    return new BooleanPropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.ContentReference:
                    return new ContentReferencePropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.Date:
                    return new DatePropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.FloatNumber:
                    return new NumberPropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.LinkCollection:
                    return new LinkCollectionPropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.Number:
                    return new NumberPropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.PageReference:
                    return new PageReferencePropertyModel(propertyData, content, firstLevel, isBlockChild);
                case PropertyDataType.PageType:
                    return new PageTypePropertyModel(propertyData, content, firstLevel, isBlockChild);
                default:
                    if (propertyType == typeof(String[]))
                        return new StringArrayPropertyModel(propertyData, content, firstLevel, isBlockChild);
                    if (propertyType == typeof(ContentArea))
                        return new ContentAreaPropertyModel(propertyData, content, firstLevel, isBlockChild);
                    if (propertyType == typeof(XhtmlString))
                        return new XhtmlStringPropertyModel(propertyData, content, firstLevel, isBlockChild);
                    if (propertyType == typeof(string))
                        return new StringPropertyModel(propertyData, content, firstLevel, isBlockChild);
                    if (propertyType == typeof(Blob))
                        return new BlobPropertyModel(propertyData, content, firstLevel, isBlockChild);
                    if (IsGenericList(propertyType))
                        return new CollectionPropertyModel(propertyData, content, firstLevel, isBlockChild);
                    return new EmptyPropertyModel(propertyData, content, firstLevel, isBlockChild);
            }
        }

        public static bool IsGenericList(Type type)
        {
            return type != null && type.GetInterfaces().Where(@interface => @interface.IsGenericType).Any(@interface => @interface.GetGenericTypeDefinition() == typeof(ICollection<>));
        }
    }
}