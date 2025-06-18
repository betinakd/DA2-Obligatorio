using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class NamespaceRequest
{
    [Required(ErrorMessage = "Name is required.")]
    public required string Name { get; set; }

    public Guid? BaseNamespaceId { get; set; }
}
