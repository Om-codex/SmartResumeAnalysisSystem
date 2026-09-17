# Smart Resume Analysis System

ASP.NET Core MVC college mini project using C#, classes, properties, methods, string handling, Razor views, clean HTML/CSS UI, dark mode, and Gemini AI resume analysis.

## Features

- Enter personal, education, skills, projects, and experience details.
- Enter target job role and a small job description.
- Generate a structured resume preview.
- Analyze resume score, strengths, weaknesses, missing skills, keywords, and improvements.
- Download the result as a PDF using the browser print/save dialog.
- Works without login and without database.
- Uses local demo scoring if Gemini API key is not configured.

## Gemini API Key

Do not hard-code the API key in source code. Use one of these options:

```powershell
$env:GEMINI_API_KEY="your_google_ai_studio_key"
```

Or set `Gemini:ApiKey` through user secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "your_google_ai_studio_key"
```

## Run

```powershell
dotnet restore
dotnet run
```

Then open the URL shown in the terminal, usually `https://localhost:5001` or `http://localhost:5000`.

## Project Report Points

- Problem: Students often do not know whether their resume matches a job role.
- Objective: Create resume, analyze it using AI, suggest improvements, and generate a PDF report.
- Technologies: ASP.NET Core MVC, C#, HTML, CSS, JavaScript, Gemini API.
- C# Concepts: Classes, properties, methods, string handling, dependency injection, async API call.