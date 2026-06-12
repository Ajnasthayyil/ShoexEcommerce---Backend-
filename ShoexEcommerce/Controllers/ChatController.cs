using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.DTOs.Chat;
using ShoexEcommerce.Application.Interfaces;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IShoppingAssistantService _service;

    public ChatController(
        IShoppingAssistantService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Ask(
        ChatRequestDto dto)
    {
        int userId = 1;

        var answer =
            await _service.AskAsync(userId, dto.Message);

        return Ok(new ChatResponseDto
        {
            Response = answer
        });
    }
}