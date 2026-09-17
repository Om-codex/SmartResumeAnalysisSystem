using System.Text;
using SmartResumeAnalysisSystem.Models;

namespace SmartResumeAnalysisSystem.Services;

public class ResumeBuilderService
{
    public string BuildStructuredResume(ResumeInputModel model)
    {
        var builder = new StringBuilder();

        builder.AppendLine(model.FullName.ToUpperInvariant());
        builder.AppendLine($"Email: {model.Email} | Phone: {model.Phone}");
        AppendIfNotEmpty(builder, "Address", model.Address);
        AppendIfNotEmpty(builder, "LinkedIn", model.LinkedIn);
        AppendIfNotEmpty(builder, "GitHub", model.GitHub);

        AppendSection(builder, "Career Objective", model.CareerObjective);
        AppendEducation(builder, model.Education);
        AppendSkills(builder, model.Skills);
        AppendProjects(builder, model.Projects);
        AppendExperience(builder, model.Experience);

        builder.AppendLine();
        builder.AppendLine("Target Job Role");
        builder.AppendLine(model.TargetJobRole);
        AppendSection(builder, "Job Description", model.JobDescription);

        return builder.ToString().Trim();
    }

    private static void AppendIfNotEmpty(StringBuilder builder, string label, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            builder.AppendLine($"{label}: {value}");
        }
    }

    private static void AppendSection(StringBuilder builder, string title, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine(title);
        builder.AppendLine(value);
    }

    private static void AppendEducation(StringBuilder builder, List<EducationEntry> education)
    {
        if (education.Count == 0) return;

        builder.AppendLine();
        builder.AppendLine("Education");
        foreach (var item in education)
        {
            builder.AppendLine($"- {item.Degree}, {item.Institution} ({item.PassingYear}) - {item.Score}".TrimEnd(' ', '-', ','));
        }
    }

    private static void AppendSkills(StringBuilder builder, List<SkillEntry> skills)
    {
        if (skills.Count == 0) return;

        builder.AppendLine();
        builder.AppendLine("Skills");
        foreach (var group in skills.GroupBy(x => string.IsNullOrWhiteSpace(x.SkillType) ? "Other" : x.SkillType))
        {
            var names = string.Join(", ", group.Select(x => x.SkillName).Where(x => !string.IsNullOrWhiteSpace(x)));
            builder.AppendLine($"- {group.Key}: {names}");
        }
    }

    private static void AppendProjects(StringBuilder builder, List<ProjectEntry> projects)
    {
        if (projects.Count == 0) return;

        builder.AppendLine();
        builder.AppendLine("Projects");
        foreach (var project in projects)
        {
            builder.AppendLine($"- {project.Title}");
            AppendIfNotEmpty(builder, "  Description", project.Description);
            AppendIfNotEmpty(builder, "  Technologies", project.TechnologiesUsed);
        }
    }

    private static void AppendExperience(StringBuilder builder, List<ExperienceEntry> experience)
    {
        if (experience.Count == 0) return;

        builder.AppendLine();
        builder.AppendLine("Experience / Internship");
        foreach (var item in experience)
        {
            builder.AppendLine($"- {item.Role} at {item.CompanyName} ({item.Duration})".TrimEnd(' ', '-', '('));
            AppendIfNotEmpty(builder, "  Responsibilities", item.Responsibilities);
        }
    }
}
