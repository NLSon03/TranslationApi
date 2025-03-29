
namespace TranslationWeb.Infrastructure.Interfaces;

public interface IErrorHandlingService
{
    /// <summary>
    /// Xử lý các exception chung
    /// </summary>
    /// <param name="exception">Exception cần xử lý</param>
    Task HandleErrorAsync(Exception exception);

    /// <summary>
    /// Xử lý các lỗi HTTP
    /// </summary>
    /// <param name="response">HTTP response chứa thông tin lỗi</param>
    Task HandleHttpErrorAsync(HttpResponseMessage response);

    /// <summary>
    /// Xử lý các lỗi từ API
    /// </summary>
    /// <param name="errorMessage">Thông báo lỗi từ API</param>
    Task HandleApiErrorAsync(string errorMessage);
}