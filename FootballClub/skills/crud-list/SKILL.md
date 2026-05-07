---
description: Generates an ASP.NET Core MVC List page for an Entity Framework model.
---

# ASP.NET MVC List Page Generator

Use this skill when the user asks to construct a "List" (Index) view for a specific Entity Framework model.

## Instructions

1. **Controller Configuration**: 
   - Check if a Controller for the model exists. If not, create one.
   - Inject `ApplicationDbContext` through the constructor.
   - Create an `Index()` action that fetches all records using `_context.ModelName.ToList()`.
   - Apply an SEO-friendly custom route using the `[Route]` attribute.

2. **View Generation**:
   - Create an `Index.cshtml` view inside `Views/<ControllerName>/`.
   - Set the page model to `@model IEnumerable<FootballClub.Models.YourModelName>`.
   - Set a descriptive `ViewData["Title"]`.

3. **HTML / UI Structure**:
   - Use an HTML `<table>` styled with standard Bootstrap classes (e.g., `table table-striped table-hover table-bordered`).
   - Create a `<thead>` with `<th>` headers for all the mapped, scalar properties of the target model.
   - Use a `@foreach (var item in Model)` loop to render `<tr>` tags in the `<tbody>`.
   - Include a final "Actions" column containing "Edit", "Details", and "Delete" links.

4. **Integrations**:
   - Ensure the newly created list plays nicely with the existing navigation and breadcrumbs if applicable.
