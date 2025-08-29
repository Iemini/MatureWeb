using System.ComponentModel.DataAnnotations;

namespace Northwind.Mvc.Models;

public record class Thing
(
    [Range(1, 10)] int? Id,
    [Required] string? Color,
    [EmailAddress] string? Email
);

