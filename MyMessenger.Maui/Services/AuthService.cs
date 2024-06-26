using System.Text.Json;
using Blazored.LocalStorage;
using MyMessenger.Maui.Library.Interface;
using MyMessenger.Application.DTO.AuthDTOs;
using System.Security.Claims;
using MyMessenger.Application.DTO;

namespace MyMessenger.Maui.Services;

public class AuthService
{
    private readonly IHttpWrapper httpWrapper;
    private readonly ILocalStorageService storage;
    private readonly CustomAuthService authService;
    public AuthService(IHttpWrapper httpWrapper, ILocalStorageService storage, CustomAuthService authService)
	{
        this.httpWrapper = httpWrapper;
        this.storage = storage;
        this.authService = authService;
    }
    public async Task<bool> Login(string email, string password)
    {
        var data = new LoginDTO { Email = email, Password = password };
        var json = JsonSerializer.Serialize(data);
        try
        {
            var responseData = await httpWrapper.PostAsync("Auth/", json);
            var response = await responseData.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(response))
            {
                var responseObject = JsonSerializer.Deserialize<TokensDTO>(response);
                var accessToken = responseObject.accessToken;
                var refreshToken = responseObject.refreshToken;
                await storage.SetItemAsync("accessToken", accessToken);
                await storage.SetItemAsync("refreshToken", refreshToken);

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email), 
            };
                var identity = new ClaimsIdentity(claims, "jwt");
                authService.CurrentUser = new ClaimsPrincipal(identity);
            }
            else
            {
                Console.WriteLine("Invalid login credentials.");
                return false;
            }
        } catch (HttpRequestException ex) 
        {
            Console.WriteLine($"HTTP request failed: {ex.Message}");
            return false;
        }
        return true;
    }
    public async Task<bool> Refresh()
    {
        var accessToken = await storage.GetItemAsStringAsync("accessToken");
        var refreshToken = await storage.GetItemAsStringAsync("refreshToken");
        var data = new TokensDTO { refreshToken = refreshToken, accessToken = accessToken };
        var json = JsonSerializer.Serialize(data);
        try
        {
            var responseData = await httpWrapper.PostAsync("Auth/refresh", json);
            var response = await responseData.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<TokensDTO>(response);
            var newAccessToken = responseObject.accessToken;
            var newRefreshToken = responseObject.refreshToken;
            if (newAccessToken != null)
            {
                await storage.SetItemAsync("accessToken", newAccessToken);
                await storage.SetItemAsync("refreshToken", newRefreshToken);
            }
            else
            {
                Console.WriteLine("Tokens are expired!");
                await Logout();
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request failed: {ex.Message}");
            return false;
        }
        return true;
    }
    public async Task Logout()
    {
        authService.CurrentUser = new ClaimsPrincipal();
        await storage.RemoveItemAsync("accessToken");
        await storage.RemoveItemAsync("refreshToken");
    }
    public async Task<bool> SignUp(string name, string username, string email, string password)
    {
        var data = new SignUpDTO { Email = email, Password = password, Name = name, UserName = username };
        var json = JsonSerializer.Serialize(data);
        try
        {
            var responseData = await httpWrapper.PostAsync("Auth/sign", json);
            var response = await responseData.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(response))
            {
                var responseObject = JsonSerializer.Deserialize<ResponseDTO>(response);
                return responseObject.isSuccessful;
            }
            else
            {
                Console.WriteLine("Invalid data.");
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request failed: {ex.Message}");
            return false;
        }
    }
}