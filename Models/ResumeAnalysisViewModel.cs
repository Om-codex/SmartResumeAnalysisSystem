namespace SmartResumeAnalysisSystem.Models;

public class ResumeAnalysisViewModel
{
    public ResumeInputModel Resume { get; set; } = ResumeInputModel.CreateDefault();
    public ResumeAnalysisResult Analysis { get; set; } = new();
    public string StructuredResume { get; set; } = string.Empty;
}
