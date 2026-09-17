using System.ComponentModel.DataAnnotations;

namespace SmartResumeAnalysisSystem.Models;

public class ResumeInputModel
{
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    [Display(Name = "LinkedIn URL")]
    public string LinkedIn { get; set; } = string.Empty;

    [Display(Name = "GitHub URL")]
    public string GitHub { get; set; } = string.Empty;

    [Display(Name = "Career Objective")]
    public string CareerObjective { get; set; } = string.Empty;

    [Required(ErrorMessage = "Target job role is required.")]
    [Display(Name = "Target Job Role")]
    public string TargetJobRole { get; set; } = string.Empty;

    [Display(Name = "Small Job Description")]
    public string JobDescription { get; set; } = string.Empty;

    public List<EducationEntry> Education { get; set; } = [];
    public List<SkillEntry> Skills { get; set; } = [];
    public List<ProjectEntry> Projects { get; set; } = [];
    public List<ExperienceEntry> Experience { get; set; } = [];

    public static ResumeInputModel CreateDefault()
    {
        return new ResumeInputModel
        {
            Education = [new EducationEntry()],
            Skills = [new SkillEntry()],
            Projects = [new ProjectEntry()],
            Experience = [new ExperienceEntry()]
        };
    }

    public void Normalize()
    {
        Education ??= [];
        Skills ??= [];
        Projects ??= [];
        Experience ??= [];

        FullName = Clean(FullName);
        Email = Clean(Email);
        Phone = Clean(Phone);
        Address = Clean(Address);
        LinkedIn = Clean(LinkedIn);
        GitHub = Clean(GitHub);
        CareerObjective = Clean(CareerObjective);
        TargetJobRole = Clean(TargetJobRole);
        JobDescription = Clean(JobDescription);

        Education = Education.Where(x => !x.IsEmpty()).ToList();
        Skills = Skills.Where(x => !x.IsEmpty()).ToList();
        Projects = Projects.Where(x => !x.IsEmpty()).ToList();
        Experience = Experience.Where(x => !x.IsEmpty()).ToList();
    }

    public void EnsureRows()
    {
        Education ??= [];
        Skills ??= [];
        Projects ??= [];
        Experience ??= [];

        if (Education.Count == 0) Education.Add(new EducationEntry());
        if (Skills.Count == 0) Skills.Add(new SkillEntry());
        if (Projects.Count == 0) Projects.Add(new ProjectEntry());
        if (Experience.Count == 0) Experience.Add(new ExperienceEntry());
    }

    private static string Clean(string? value)
    {
        return string.Join(' ', (value ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}

public class EducationEntry
{
    public string Degree { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public string PassingYear { get; set; } = string.Empty;
    public string Score { get; set; } = string.Empty;

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Degree)
            && string.IsNullOrWhiteSpace(Institution)
            && string.IsNullOrWhiteSpace(PassingYear)
            && string.IsNullOrWhiteSpace(Score);
    }
}

public class SkillEntry
{
    public string SkillName { get; set; } = string.Empty;
    public string SkillType { get; set; } = "Technical";

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(SkillName);
    }
}

public class ProjectEntry
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TechnologiesUsed { get; set; } = string.Empty;

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Title)
            && string.IsNullOrWhiteSpace(Description)
            && string.IsNullOrWhiteSpace(TechnologiesUsed);
    }
}

public class ExperienceEntry
{
    public string CompanyName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Responsibilities { get; set; } = string.Empty;

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(CompanyName)
            && string.IsNullOrWhiteSpace(Role)
            && string.IsNullOrWhiteSpace(Duration)
            && string.IsNullOrWhiteSpace(Responsibilities);
    }
}
