# File Upload UI - Quick Integration Guide

## Add to Any View in 2 Steps

### Step 1: Set ViewBag Properties
```html
@{
    ViewBag.EntityType = "YourEntityName";
    ViewBag.EntityId = Model.Id;
}
```

### Step 2: Include the Partial
```html
@await Html.PartialAsync("_AttachmentUpload")
```

## Real Examples

### Player Edit View
```html
@model PlayerEditModel

<form method="post" action="/Player/Edit/@Model.Id">
    <!-- Form fields here -->
</form>

<!-- File Upload -->
@{
    ViewBag.EntityType = "Player";
    ViewBag.EntityId = Model.Id;
}
@await Html.PartialAsync("_AttachmentUpload")
```

### Club Details View
```html
@model Club

<div class="club-info">
    <!-- Club details here -->
</div>

<!-- File Upload -->
@{
    ViewBag.EntityType = "Club";
    ViewBag.EntityId = Model.Id;
}
@await Html.PartialAsync("_AttachmentUpload")
```

### Staff Member View
```html
@model Staff

<div class="staff-profile">
    <!-- Staff details here -->
</div>

<!-- File Upload -->
@{
    ViewBag.EntityType = "Staff";
    ViewBag.EntityId = Model.Id;
}
@await Html.PartialAsync("_AttachmentUpload")
```

## Configuration Options

### ViewBag Properties (Required)
- `EntityType` (string): The entity type (e.g., "Player", "Club", "Match", "Stadium")
- `EntityId` (int): The unique ID of the entity

### ViewBag Properties (Optional)
- `AllowMultiple` (bool): Allow multiple file selection (default: true)

### Example with Optional Props
```html
@{
    ViewBag.EntityType = "Match";
    ViewBag.EntityId = Model.Id;
    ViewBag.AllowMultiple = true; // Allow uploading multiple files at once
}
@await Html.PartialAsync("_AttachmentUpload")
```

## File Structure

```
Football Club/
├── wwwroot/
│   ├── css/
│   │   └── attachment-upload.css      ✅ Styling
│   └── js/
│       └── attachment-upload.js       ✅ JavaScript module
├── Views/
│   └── Shared/
│       └── _AttachmentUpload.cshtml  ✅ Partial view
├── Web/
│   └── Controllers/
│       └── Api/
│           └── AttachmentsApiController.cs  ✅ API endpoints
└── FILE_UPLOAD_UI.md                  ✅ Full documentation
```

## API Endpoints Used

All requests go to `/api/attachments`

### Upload File
```
POST /api/attachments
Content-Type: multipart/form-data

Form Fields:
- entityType: string
- entityId: int
- file: IFormFile
```

### List Files
```
GET /api/attachments?entityType={type}&entityId={id}
```

### Delete File
```
DELETE /api/attachments/{id}
```

## File Support

**Allowed Types:**
- 📄 PDF (application/pdf)
- 🖼️ Images (PNG, JPG, GIF, WebP)
- 📝 Text (text/plain)

**Max Size:** 10 MB per file

## Browser Compatibility

| Browser | Status |
|---------|--------|
| Chrome  | ✅ Full Support |
| Firefox | ✅ Full Support |
| Safari  | ✅ Full Support |
| Edge    | ✅ Full Support |
| IE 11   | ❌ Not Supported |

## Current Implementations

The component is already integrated into:

1. ✅ **Player Edit** (`/Player/Edit/{id}`)
2. ✅ **Player Details** (`/Player/Details/{id}`)
3. ✅ **Staff Edit** (`/Staff/Edit/{id}`)
4. ✅ **Club Details** (`/Club/Details/{id}`)

## Common Issues & Solutions

### Issue: Files won't upload
**Check:**
- User is authenticated (API requires login)
- User has Admin or User role
- File is in allowed format
- File size is under 10 MB

### Issue: Delete button not working
**Check:**
- User has Admin role (delete requires Admin)
- File still exists on server
- Check browser console for errors

### Issue: No files showing
**Check:**
- EntityType and EntityId are correct
- Files were actually uploaded
- API endpoint is responding
- Check network tab in DevTools

## Styling Customization

To customize colors, edit `attachment-upload.css`:

```css
.dropzone-wrapper {
    border: 2px dashed #0077cc;  /* Change primary color here */
    background-color: #f8f9fa;
}

.progress-bar {
    background-color: #1b6ec2;   /* Change progress bar color */
}

.btn-delete {
    background-color: #dc3545;   /* Change delete button color */
}
```

## JavaScript API

Access the component via the global `AttachmentUpload` object:

```javascript
// Initialize (done automatically, but can reinit if needed)
AttachmentUpload.init('dropzone', {
    entityType: 'Player',
    entityId: 1,
    maxFileSize: 10 * 1024 * 1024
});
```

## Security Notes

✅ **HTTPS** - Always use HTTPS in production
✅ **CSRF Protection** - API respects ASP.NET Core CSRF tokens
✅ **Authorization** - Upload requires "Admin, User" roles; Delete requires "Admin"
✅ **File Validation** - Server validates MIME types and file size
✅ **Path Security** - Files stored with GUID prefix to prevent collisions
✅ **XSS Prevention** - HTML is escaped to prevent script injection

## Next Steps

1. **Test the UI** - Navigate to a Player/Staff/Club edit page
2. **Upload a File** - Drag or click to upload
3. **Verify Storage** - Check `wwwroot/uploads/{entityType}/{entityId}/`
4. **Add to More Views** - Use the integration template above
5. **Customize Styling** - Adjust colors to match your theme

## Support

For detailed documentation, see: `FILE_UPLOAD_UI.md`

For troubleshooting, see: `FILE_UPLOAD_UI.md#Troubleshooting`
