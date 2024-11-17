using JsonApiClient.Attributes;

namespace JsonApiClient.Exceptions;

/// <summary>
/// Exception thrown when an attribute that is required for the proper functioning of the library is missing. This is
/// usually fired when a resource model is not decorated with the required <see cref="JResAttribute"/> attribute.
/// </summary>
public class MissingAttributeException: InvalidOperationException
{
    /// <summary>
    /// Constructor of the exception, taking a message to be displayed.
    /// </summary>
    /// <param name="attributeType">The type of the attribute that is missing.</param>
    /// <param name="modelType">The type of the class which should be decorated with the attribute that is missing.</param>
    public MissingAttributeException(Type attributeType, Type modelType) : base(
        $"Model {modelType} is not decorated with the {attributeType}, hence it can not be used as a json:api resource.")
    { }
}