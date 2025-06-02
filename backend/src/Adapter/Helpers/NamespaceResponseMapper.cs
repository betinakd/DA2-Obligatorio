using System.Diagnostics.CodeAnalysis;
using Domain;
using Models.Response;

namespace Adapter.Helpers;
[ExcludeFromCodeCoverage]
public static class NamespaceResponseMapper
{
    public static NamespaceResponse MapToNamespaceResponse(SimNamespace domainNamespace)
    {
        if(domainNamespace == null)
        {
            return null;
        }

        return new NamespaceResponse
        {
            Id = domainNamespace.Id,
            Name = domainNamespace.Name,
            BaseNamespaceId = domainNamespace.BaseNamespaceId,
            Elements = domainNamespace.Elements?
                .Select(SimClassResponseMapper.MapToSimClassResponse)
                .ToList() ?? []
        };
    }
}
