using Microsoft.AspNetCore.Mvc;
using SmartResumeAnalysisSystem.Models;
using SmartResumeAnalysisSystem.Services;

namespace SmartResumeAnalysisSystem.Controllers;

public class ResumeController : Controller
{
    private readonly ResumeBuilderService _resumeBuilderService;
    private readonly IResumeAiService _resumeAiService;

    public ResumeController(ResumeBuilderService resumeBuilderService, IResumeAiService resumeAiService)
    {
        _resumeBuilderService = resumeBuilderService;
        _resumeAiService = resumeAiService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(ResumeInputModel.CreateDefault());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Analyze(ResumeInputModel model, CancellationToken cancellationToken)
    {
        model.Normalize();

        if (!ModelState.IsValid)
        {
            model.EnsureRows();
            return View("Create", model);
        }

        var resumeText = _resumeBuilderService.BuildStructuredResume(model);
        var analysis = await _resumeAiService.AnalyzeAsync(model, resumeText, cancellationToken);

        return View("Result", new ResumeAnalysisViewModel
        {
            Resume = model,
            StructuredResume = resumeText,
            Analysis = analysis
        });
    }
}