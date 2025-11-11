using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Models;

public abstract class Person: IdentityUser<int>
{
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
}