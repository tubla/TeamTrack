using System.Net.Http.Json;
using TeamTrack.UI.Models.Common;
using TeamTrack.UI.Models.Attachments;
using TeamTrack.UI.Services.Contracts;

namespace TeamTrack.UI.Services;

public class AttachmentService : IAttachmentService
{
    private readonly HttpClient _httpClient;

    public AttachmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<AttachmentDto>> UploadAsync(MultipartFormDataContent content, Guid? taskId)
    {
        try
        {
            var url = taskId.HasValue ? $"attachments?taskId={taskId}" : "attachments";
            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadFromJsonAsync<ApiResponse<AttachmentDto>>()
                   ?? new ApiResponse<AttachmentDto> { Success = false };
        }
        catch (Exception ex)
        {
            return new ApiResponse<AttachmentDto> { Success = false, Errors = new() { ex.Message } };
        }
    }

    public async Task<ApiResponse<List<AttachmentDto>>> GetByTaskAsync(Guid taskId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AttachmentDto>>>($"attachments?taskId={taskId}");
            return response ?? new ApiResponse<List<AttachmentDto>> { Success = false };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<AttachmentDto>> { Success = false, Errors = new() { ex.Message } };
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"attachments/{id}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>()
                   ?? new ApiResponse<bool> { Success = false };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Errors = new() { ex.Message } };
        }
    }
}