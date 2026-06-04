---
description: Generates an ASP.NET Core MVC Edit/Create form page for an Entity Framework model.
---

# ASP.NET MVC Edit Form Generator

Use this skill when the user asks to construct an "Edit" or "Create" form view for a specific Entity Framework model.

## Instructions

1. **Controller Configuration**: 
   - Ensure the Controller has a `[HttpGet]` action for `Edit(int id)` that retrieves the database entity and returns the specific View.
   - Ensure there is a `[HttpPost]` action for `Edit(int id, Model model)` that validates `ModelState`, maps values, updates the `_context`, and saves changes async.
   
2. **View Generation**:
   - Create an `Edit.cshtml` view inside `Views/<ControllerName>/`.
   - Set `@model FootballClub.Models.YourModelName`.

3. **HTML / UI Structure**:
   - Wrap the form in a Bootstrap `<div class="row"><div class="col-md-8">` structure.
   - Begin the form using `<form asp-action="Edit">`.
   - Include a hidden input for the Primary Key (`<input type="hidden" asp-for="Id" />`).
   - For every scalar attribute, create a `<div class="form-group mb-3">`:
     - Provide a `<label asp-for="Property" class="control-label"></label>`.
     - Provide an `<input asp-for="Property" class="form-control" />`.
     - Add a `<span asp-validation-for="Property" class="text-danger"></span>`.
   - Add a submit button `<button type="submit" class="btn btn-primary">Save Changes</button>`.

4. **Validation Scripts**:
   - At the bottom of the View, render the `_ValidationScriptsPartial` to enable client-side validation:
     ```html
     @section Scripts {
         @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
     }
     ```
