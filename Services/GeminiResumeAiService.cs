using System.Net.Http.Json;
using System.Text.Json;
using SmartResumeAnalysisSystem.Models;

namespace SmartResumeAnalysisSystem.Services;

public class GeminiResumeAiService : IResumeAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiResumeAiService> _logger;

    public GeminiResumeAiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiResumeAiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResumeAnalysisResult> AnalyzeAsync(ResumeInputModel model, string structuredResume, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return BuildDemoAnalysis(model, structuredResume, "Gemini API key is not configured in user-secrets or the GEMINI_API_KEY environment variable.");
        }

        try
        {
            var geminiModel = _configuration["Gemini:Model"] ?? "gemini-2.0-flash";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiModel}:generateContent?key={Uri.EscapeDataString(apiKey)}";

            var request = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = BuildPrompt(model, structuredResume) }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.35
                }
            };

            using var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var text = ExtractGeminiText(responseJson);
            var result = DeserializeAnalysis(text);
            result.UsedAi = true;
            return NormalizeAnalysisResult(result, model);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Gemini analysis failed. Falling back to local demo analysis.");
            return BuildDemoAnalysis(model, structuredResume, $"Gemini API error: {ex.Message}");
        }
    }

    private static string BuildPrompt(ResumeInputModel model, string structuredResume)
    {
        return $$"""
        You are a resume evaluator for college students and entry-level candidates.
        Analyze this resume for the target role and job description.

        Return valid JSON only. Do not include markdown.

        JSON format:
        {
          "resumeScore": 75,
          "overallSummary": "short summary",
          "strengths": ["strength 1"],
          "weaknesses": ["weakness 1"],
          "missingSkills": ["skill 1"],
          "improvements": ["improvement 1"],
          "projectSuggestions": ["project suggestion 1"],
          "objectiveSuggestion": "improved career objective",
          "keywordSuggestions": ["keyword 1"],
          "jobRoleFit": "short role fit explanation"
        }

        Target role: {{model.TargetJobRole}}
        Job description: {{model.JobDescription}}

        Resume:
        {{structuredResume}}
        """;
    }

    private static string ExtractGeminiText(string responseJson)
    {
        using var document = JsonDocument.Parse(responseJson);
        return document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";
    }

    private static ResumeAnalysisResult DeserializeAnalysis(string text)
    {
        var cleaned = text.Trim();
        if (cleaned.StartsWith("```", StringComparison.Ordinal))
        {
            cleaned = cleaned.Replace("```json", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("```", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();
        }

        return JsonSerializer.Deserialize<ResumeAnalysisResult>(cleaned, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ResumeAnalysisResult();
    }

    private static ResumeAnalysisResult BuildDemoAnalysis(ResumeInputModel model, string structuredResume, string reasonMessage)
    {
        var text = structuredResume.ToLowerInvariant();
        var skillCount = model.Skills.Count(x => !string.IsNullOrWhiteSpace(x.SkillName));
        var projectCount = model.Projects.Count(x => !string.IsNullOrWhiteSpace(x.Title));
        var experienceCount = model.Experience.Count(x => !string.IsNullOrWhiteSpace(x.Role));
        var hasObjective = !string.IsNullOrWhiteSpace(model.CareerObjective);
        var score = 45 + Math.Min(skillCount * 4, 20) + Math.Min(projectCount * 7, 20) + Math.Min(experienceCount * 5, 10) + (hasObjective ? 5 : 0);

        var commonKeywords = GetSuggestedKeywords(model.TargetJobRole, text);

        return new ResumeAnalysisResult
        {
            ResumeScore = Math.Clamp(score, 0, 92),
            UsedAi = false,
            OverallSummary = $"Demo analysis generated locally. {reasonMessage}",
            Strengths = BuildStrengths(model),
            Weaknesses = BuildWeaknesses(model),
            MissingSkills = commonKeywords,
            Improvements =
            [
                "Use action verbs such as developed, designed, implemented, analyzed, automated, and optimized.",
                "Add measurable outcomes where possible, for example percentage improvement, number of users, or time saved.",
                "Keep the resume focused on the target job role instead of listing unrelated details.",
                "Mention tools, frameworks, and databases used in each project."
            ],
            ProjectSuggestions =
            [
                "Add one role-specific project that directly matches the job description.",
                "For every project, mention the problem solved, technologies used, and your individual contribution.",
                "Include deployment, GitHub, or demo links if available."
            ],
            ObjectiveSuggestion = $"Motivated candidate seeking a {model.TargetJobRole} role, with practical knowledge of relevant technologies, problem-solving ability, and project experience aligned with business requirements.",
            KeywordSuggestions = commonKeywords,
            JobRoleFit = $"The resume has a basic fit for {model.TargetJobRole}. It can be improved by adding stronger role-specific keywords, measurable project outcomes, and clearer technical responsibilities."
        };
    }

    private static ResumeAnalysisResult NormalizeAnalysisResult(ResumeAnalysisResult result, ResumeInputModel model)
    {
        result.ResumeScore = Math.Clamp(result.ResumeScore, 0, 100);
        result.OverallSummary = string.IsNullOrWhiteSpace(result.OverallSummary)
            ? "The resume was analyzed successfully, but the AI returned a short response. Review the section-wise suggestions below."
            : result.OverallSummary;
        result.Strengths ??= [];
        result.Weaknesses ??= [];
        result.MissingSkills ??= [];
        result.Improvements ??= [];
        result.ProjectSuggestions ??= [];
        result.KeywordSuggestions ??= [];
        result.ObjectiveSuggestion = string.IsNullOrWhiteSpace(result.ObjectiveSuggestion)
            ? $"Motivated candidate seeking a {model.TargetJobRole} role with practical project experience and willingness to learn."
            : result.ObjectiveSuggestion;
        result.JobRoleFit = string.IsNullOrWhiteSpace(result.JobRoleFit)
            ? $"The resume was reviewed for the {model.TargetJobRole} role."
            : result.JobRoleFit;

        if (result.Strengths.Count == 0) result.Strengths.Add("Resume information is organized into clear sections.");
        if (result.Weaknesses.Count == 0) result.Weaknesses.Add("No major weakness was returned by the AI response.");
        if (result.Improvements.Count == 0) result.Improvements.Add("Add measurable achievements and job-role keywords to improve impact.");
        if (result.ProjectSuggestions.Count == 0) result.ProjectSuggestions.Add("Add project outcomes, technologies used, and your exact contribution.");

        return result;
    }

    private static List<string> BuildStrengths(ResumeInputModel model)
    {
        var strengths = new List<string>();
        if (model.Skills.Count > 0) strengths.Add("Includes a dedicated skills section.");
        if (model.Projects.Count > 0) strengths.Add("Includes project work, which is useful for student and fresher resumes.");
        if (model.Education.Count > 0) strengths.Add("Education details are clearly provided.");
        if (model.Experience.Count > 0) strengths.Add("Experience or internship information improves credibility.");
        return strengths.Count == 0 ? ["Basic personal details are provided."] : strengths;
    }

    private static List<string> BuildWeaknesses(ResumeInputModel model)
    {
        var weaknesses = new List<string>();
        if (model.Skills.Count < 5) weaknesses.Add("Skills section looks short for the selected job role.");
        if (model.Projects.Count < 2) weaknesses.Add("Adding one or two more relevant projects would improve the resume.");
        if (string.IsNullOrWhiteSpace(model.CareerObjective)) weaknesses.Add("Career objective is missing.");
        if (string.IsNullOrWhiteSpace(model.JobDescription)) weaknesses.Add("A more detailed job description would help produce better recommendations.");
        return weaknesses.Count == 0 ? ["No major structural weakness found in the entered data."] : weaknesses;
    }

    private static List<string> GetSuggestedKeywords(string role, string resumeText)
    {
        var roleText = role.ToLowerInvariant();
        var keywords = roleText switch
        {
            var r when r.Contains("data") => new[] { "SQL", "Excel", "Power BI", "Python", "Data Cleaning", "Dashboard", "Statistics" },
            var r when r.Contains("web") || r.Contains("front") => new[] { "HTML", "CSS", "JavaScript", "Bootstrap", "Responsive Design", "API Integration", "Git" },
            var r when r.Contains(".net") || r.Contains("c#") => new[] { "C#", "ASP.NET Core", "MVC", "SQL Server", "Entity Framework", "REST API", "OOP" },
            var r when r.Contains("java") => new[] { "Java", "OOP", "Spring Boot", "SQL", "REST API", "Collections", "Git" },
            _ => new[] { "Problem Solving", "Communication", "Git", "SQL", "Project Management", "Teamwork" }
        };

        return keywords.Where(keyword => !resumeText.Contains(keyword.ToLowerInvariant())).Take(6).ToList();
    }
}
