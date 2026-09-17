using SmartResumeAnalysisSystem.Models;

namespace SmartResumeAnalysisSystem.Services;

public interface IResumeAiService
{
    Task<ResumeAnalysisResult> AnalyzeAsync(ResumeInputModel model, string structuredResume, CancellationToken cancellationToken);
}
