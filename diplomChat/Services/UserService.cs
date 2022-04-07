using AutoMapper;
using diplomChat.Authorization;
using diplomChat.Entities;
using diplomChat.Helpers;
using diplomChat.Models.Users;
using Microsoft.Extensions.Options;

namespace diplomChat.Services;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    void Register(RegisterRequest model);
    void Update(int id, UpdateRequest model);
    void Delete(int id);
    IEnumerable<User> GetAll();
    User GetById(int id);
}

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;
    
    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }
    
    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.UserName == model.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateJwtToken(user);

        return response;
    }
    
    public void Update(int id, UpdateRequest model)
    {
        var user = GetUser(id);

        if (model.Username != user.UserName && _context.Users.Any(x => x.UserName == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    
    public void Register(RegisterRequest model)
    {
        if (_context.Users.Any(x => x.UserName == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        var user = _mapper.Map<User>(model);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
        
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    
    public void Delete(int id)
    {
        var user = GetUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }
    
    public User GetById(int id)
    {
        return GetUser(id);
    }

    private User GetUser(int id) 
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}