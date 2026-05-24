\# Student Management System - Agent Instructions



\## Architecture



\- Backend: Spring Boot REST API

\- Frontend: WPF Desktop (.NET 8)

\- Database: MySQL

\- Architecture Pattern: MVVM



\## Actors



1\. BANQUANLY

2\. GIAOVIEN



\## Important Rules



\- Teachers can only access assigned classes and assigned subjects.

\- QĐ1-QĐ6 are configurable regulations.

\- Do not modify unrelated files.

\- Prefer minimal blast radius changes.

\- Use existing APIs whenever possible.

\- Keep UI modern and clean using Material Design.

\- Avoid breaking existing backend endpoints.

\- All business logic should remain in backend when possible.



\## WPF Rules



\- Use MVVM strictly.

\- No business logic inside Views.

\- Use ObservableObject from CommunityToolkit.Mvvm.

\- Use async/await for API calls.

\- Use Services layer for HTTP communication.



\## Folder Responsibilities



Views/

UI only



ViewModels/

Presentation logic only



Services/

API communication and infrastructure



Models/

DTOs and data models



\## Workflow



1\. Explore first

2\. Plan

3\. Implement

4\. Verify build

5\. Verify no unrelated files changed



\## Done Criteria



\- Project builds successfully

\- No XAML compile errors

\- No unrelated files modified

\- UI functional

