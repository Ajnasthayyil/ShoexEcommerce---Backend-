namespace ShoexEcommerce.Application.Interfaces;

public interface IShoppingAssistantService
{
    Task<string> AskAsync(int userId, string question);
}