using diplomChat.Entities;

namespace diplomChat.Models.Users;

public class AuthenticateResponse : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public Role Role { get; set; }
    
    public string Token { get; set; }

    public AuthenticateResponse(User user, string token)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        UserName = user.UserName;
        Role = user.Role;
        Token = token;
    }
}