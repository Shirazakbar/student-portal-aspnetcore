# Student Portal

`Student Portal` is a student portal workspace containing a static frontend and an ASP.NET Core backend.

## Structure

- `frontend/` — static HTML, CSS, and JavaScript pages.
- `StudentPortalApi/` — ASP.NET Core Web API project.

## Setup

1. Open the solution in Visual Studio or your preferred .NET development environment.
2. Configure your local database connection string in `StudentPortalApi/appsettings.json`.
3. Run `dotnet restore` and `dotnet run` from the `StudentPortalApi` folder.

## Notes

- `bin/` and `obj/` directories are generated build outputs and should not be committed.
- Local development configuration files are ignored by `.gitignore`.
- Keep database credentials and local secrets out of source control.


## 🚀 Live Demo
[Click here to view the Live Site](https://student-portal-aspnetcore.onrender.com/)

> ⚠️ **Note on First Load:** This application is hosted on Render's free tier. 
> If the page takes 60–90 seconds to load initially, the server is just waking 
> up from sleep mode! Once awake, it will be lightning fast.
