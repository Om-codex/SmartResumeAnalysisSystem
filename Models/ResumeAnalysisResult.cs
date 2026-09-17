namespace SmartResumeAnalysisSystem.Models;

public class ResumeAnalysisResult
{
    public int ResumeScore { get; set; }
    public string OverallSummary { get; set; } = string.Empty;
    public List<string> Strengths { get; set; } = [];
    public List<string> Weaknesses { get; set; } = [];
    public List<string> MissingSkills { get; set; } = [];
    public List<string> Improvements { get; set; } = [];
    public List<string> ProjectSuggestions { get; set; } = [];
    public string ObjectiveSuggestion { get; set; } = string.Empty;
    public List<string> KeywordSuggestions { get; set; } = [];
    public string JobRoleFit { get; set; } = string.Empty;
    public bool UsedAi { get; set; }
}
