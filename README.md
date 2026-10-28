# Quran Memorization Planner (Child-Friendly)

Full-stack app that builds a personalized Quran reading plan for children.

Inputs: start Surah + Ayah, duration (weeks), unit (verses / lines / pages / half-pages / percent), amount per session, sessions per week.
Output: a week-by-week to-do list with kid-friendly colors, emojis, and a Download-as-PDF button.

## Structure

plan/
  backend/QuranPlanner.API/            .NET 9 Web API
    Controllers/PlanController.cs
    Models/PlanModels.cs
    Services/QuranDataService.cs
    Services/PlanGeneratorService.cs
    wwwroot/index.html                 frontend (served at /)
  ai-service/planner_ai.py             standalone Python planner
  README.md
  Setup-QuranPlanner.ps1

## Run

  cd E:\D_PART\Back_end\.Net_Projects\final_V\plan\backend\QuranPlanner.API
  dotnet restore
  dotnet run

Open http://localhost:5000 (or the URL the console prints).
Swagger UI: http://localhost:5000/swagger

## API

  GET  /api/plan/health         health check
  GET  /api/plan/surahs         list all 114 surahs
  POST /api/plan/generate       generate a personalized plan

Request body example:
  { "studentName":"Aisha", "age":8, "startSurah":"Al-Fatiha", "startAyah":1,
    "unit":"verses", "amountPerSession":5, "durationWeeks":4, "sessionsPerWeek":5,
    "preferredDays":["Sun","Mon","Tue","Wed","Thu"] }

## PDF export

After generating a plan, click the 'Download as PDF' button.
The PDF is produced in the browser by html2pdf.js (CDN) - no extra packages needed.

## Unit reference

  verses       number of ayahs per session
  lines        1 line ~ 6 ayahs
  half-page    1 half page ~ 7.5 ayahs
  pages        1 page ~ 15 ayahs (Madani Mushaf)
  percent      percent of the whole Quran (6,236 ayahs)

Assalamu Alaikum and happy learning.
