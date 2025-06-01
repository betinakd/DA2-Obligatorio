using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class NamespaceRequest
{
    [Required(ErrorMessage = "Name is required.")]
    public string? Name { get; set; }

    public Guid? BaseNamespaceId { get; set; }
}
