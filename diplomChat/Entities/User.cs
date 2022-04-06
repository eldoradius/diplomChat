using System.Text.Json.Serialization;

namespace diplomChat.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public Role Role { get; set; }
    
    [JsonIgnore]
    public string PasswordHash { get; set; }
}