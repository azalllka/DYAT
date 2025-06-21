using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DYAT.Domain.Entities;

public class User : IdentityUser
{
    public virtual Wallet Wallet { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
} 