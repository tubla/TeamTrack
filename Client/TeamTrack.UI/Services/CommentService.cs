using System.Net.Http.Json;
using TeamTrack.UI.Models.Common;
using TeamTrack.UI.Models.Comments;
using TeamTrack.UI.Services.Contracts;

namespace TeamTrack.UI.Services;

public class CommentService : ICommentService
{
    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("comments", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<CommentDto>>()
                   ?? new ApiResponse<CommentDto> { Success = false };
        }
        catch (Exception ex)
        {
            return new ApiResponse<CommentDto> { Success = false, Errors = new() { ex.Message } };
        }
    }

    public async Task<ApiResponse<List<CommentDto>>> GetCommentsByTaskAsync(Guid taskId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CommentDto>>>($"comments?taskId={taskId}");
            return response ?? new ApiResponse<List<CommentDto>> { Success = false };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<CommentDto>> { Success = false, Errors = new() { ex.Message } };
        }
    }
}