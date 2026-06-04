# File Upload UI Component

## Overview

The File Upload UI is a reusable, drag-and-drop file upload component integrated with the Football Club API. It provides a modern interface for uploading, viewing, and deleting attachments associated with entities (Players, Clubs, Stadiums, etc.).

## Features

✅ **Drag & Drop Support** - Intuitive drag-and-drop file upload interface
✅ **Click to Browse** - Traditional file browser fallback
✅ **Real-time Progress** - Visual progress bar during uploads
✅ **File Validation** - Client and server-side validation
✅ **AJAX Integration** - Seamless upload/delete without page refresh
✅ **File Management** - View, download, and delete uploaded files
✅ **Responsive Design** - Works on desktop and mobile devices
✅ **Error Handling** - User-friendly error messages and alerts

## Technical Stack

- **Frontend**: HTML, CSS, Vanilla JavaScript (no dependencies)
- **Backend**: ASP.NET Core API (`/api/attachments`)
- **Files Storage**: `wwwroot/uploads/{entityType}/{entityId}/`
- **Database**: Entity Framework Core (Attachment model)

## Supported File Types

- **Documents**: PDF (.pdf)
- **Images**: PNG (.png), JPG (.jpg, .jpeg), GIF (.gif), WebP (.webp)
- **Text**: Plain Text (.txt)

**Max File Size**: 10 MB per file

## Usage

### 1. Partial View Integration

Add the upload component to any view by including the partial:

```html
@{
    ViewBag.EntityType = "Player";
    ViewBag.EntityId = Model.Id;
}
@await Html.PartialAsync("_AttachmentUpload")
```

**Required ViewBag properties:**
- `EntityType` (string): The type of entity (e.g., "Player", "Club", "Stadium")
- `EntityId` (int): The unique identifier of the entity

### 2. JavaScript Initialization

The component automatically initializes when the DOM is ready. The partial view includes the necessary script tags and initialization code.

### 3. Current Implementations

The component is already integrated into:

- **Player Edit** (`Views/Player/Edit.cshtml`)
- **Player Details** (`Views/Player/Details.cshtml`)

## API Endpoints

### GET - List Attachments
```
GET /api/attachments?entityType={type}&entityId={id}
```

**Response:**
```json
[
  {
    "id": 1,
    "entityType": "Player",
    "entityId": 1,
    "fileName": "passport.pdf",
    "filePath": "uploads/Player/1/abc123_passport.pdf",
    "contentType": "application/pdf",
    "fileSize": 102400,
    "createdAt": "2026-05-31T10:30:00Z"
  }
]
```

### POST - Upload File
```
POST /api/attachments
Content-Type: multipart/form-data

Form Data:
- entityType (string): The entity type
- entityId (int): The entity ID
- file (IFormFile): The file to upload
```

**Response (201 Created):**
```json
{
  "id": 1,
  "entityType": "Player",
  "entityId": 1,
  "fileName": "passport.pdf",
  "filePath": "uploads/Player/1/abc123_passport.pdf",
  "contentType": "application/pdf",
  "fileSize": 102400,
  "createdAt": "2026-05-31T10:30:00Z"
}
```

### DELETE - Remove Attachment
```
DELETE /api/attachments/{id}
```

**Response (204 No Content)**

## Component Structure

### Files

1. **Views/Shared/_AttachmentUpload.cshtml**
   - Razor partial view with HTML structure
   - Dropzone element, file list, and alert containers
   - ViewBag-based configuration

2. **wwwroot/css/attachment-upload.css**
   - Comprehensive styling for all component elements
   - Responsive design with mobile support
   - Animations and hover effects

3. **wwwroot/js/attachment-upload.js**
   - IIFE-based module for encapsulation
   - AJAX file upload/delete operations
   - DOM manipulation and event handling
   - File validation and error handling

## Component UI Elements

### Dropzone
- Drag-and-drop area with visual feedback
- Click to open file browser
- Helpful hints for supported file types

### Progress Bar
- Shows upload progress during file transfers
- Displays count of files being uploaded
- Auto-hides when upload completes

### File List
- Displays all uploaded files for the entity
- Shows file name, size, and upload date
- Download button (direct file access)
- Delete button with confirmation (Admin only)

### Alerts
- Success: File uploaded successfully
- Warning: Partial success with some failures
- Error: Upload or delete failed
- Auto-dismiss after 5 seconds

## Styling & Customization

### CSS Variables
The component uses Bootstrap-compatible colors:
- Primary blue: `#0077cc` / `#1b6ec2`
- Danger red: `#dc3545` / `#c82333`
- Light backgrounds: `#f8f9fa`, `#e7f0f8`

### Responsive Breakpoints
- Desktop: Full layout with side-by-side file items
- Mobile (<768px): Stacked layout for better touch interaction

### Theme Integration
The component works with the existing Bootstrap theme and can be customized by:
1. Modifying CSS variable values in `attachment-upload.css`
2. Overriding styles in your own CSS file
3. Adjusting the color scheme in the partial view

## Security Considerations

### Client-Side Validation
- File type checking (MIME type)
- File size validation (10 MB limit)
- XSS prevention (HTML escaping)

### Server-Side Validation
- MIME type whitelist enforcement
- File size validation
- Path traversal prevention (sanitization)
- Authorization checks (Admin role for delete)

### File Storage
- Files stored outside webroot for security
- GUID-prefixed filenames to avoid collisions
- Organized by entity type and ID

## Adding to Other Entities

To add the upload component to another entity (e.g., Club, Stadium):

1. **Update the Controller/View:**
   ```html
   @{
       ViewBag.EntityType = "Club";
       ViewBag.EntityId = Model.Id;
   }
   @await Html.PartialAsync("_AttachmentUpload")
   ```

2. **Ensure Authorization:**
   - The API requires `[Authorize(Roles = "Admin, User")]` for POST
   - Delete requires `[Authorize(Roles = "Admin")]`

3. **No Additional Code Needed** - The JavaScript handles everything!

## Troubleshooting

### Issue: Uploads fail with 401
**Solution:** Ensure the user is authenticated and has the appropriate role (Admin or User)

### Issue: Files not appearing in list
**Solution:** 
- Check browser console for errors
- Verify entityType and entityId are correct
- Ensure API endpoint is responding with 200 OK

### Issue: Delete button doesn't work
**Solution:** 
- Only Admin role can delete
- Check browser console for specific error messages
- Verify file hasn't already been deleted

### Issue: Upload progress bar doesn't appear
**Solution:**
- Check that `#uploadProgress` element exists in DOM
- Verify CSS is loaded correctly
- Check browser console for JavaScript errors

## Testing

### Manual Testing Steps

1. **Navigate to Player Edit page**
2. **Upload a file:**
   - Drag and drop a PDF onto the dropzone
   - Or click and browse
   - Verify progress bar appears
   - Verify success message appears
3. **Verify file appears in list:**
   - Check file name displays correctly
   - Verify file size shows correctly
   - Check creation date displays
4. **Download file:**
   - Click download button
   - File should download to your device
5. **Delete file:**
   - Click delete button
   - Confirm deletion when prompted
   - File should disappear from list
   - Verify success message

### API Testing (Postman/cURL)

**Upload a file:**
```bash
curl -X POST http://localhost:5000/api/attachments \
  -F "entityType=Player" \
  -F "entityId=1" \
  -F "file=@path/to/file.pdf"
```

**List attachments:**
```bash
curl http://localhost:5000/api/attachments?entityType=Player&entityId=1
```

**Delete attachment:**
```bash
curl -X DELETE http://localhost:5000/api/attachments/1
```

## Browser Compatibility

- Chrome/Edge: ✅ Full support
- Firefox: ✅ Full support
- Safari: ✅ Full support (iOS 13+)
- IE 11: ❌ Not supported (uses modern JavaScript)

## Future Enhancements

Potential improvements for future releases:

1. **Image Preview** - Display thumbnail previews for images
2. **Batch Upload** - Show multiple files being uploaded simultaneously
3. **Search & Filter** - Find files by name or date
4. **File Categories** - Tag files as documents, photos, certificates, etc.
5. **Edit Metadata** - Add descriptions or tags to files
6. **Compression** - Automatic image compression before upload
7. **Cloud Storage** - Integration with AWS S3, Azure Blob, etc.
8. **Virus Scanning** - Server-side malware detection
9. **Version Control** - Track file revisions and history
10. **Sharing** - Generate shareable links for files

## Support

For issues or questions about the File Upload UI component:
1. Check the troubleshooting section above
2. Review browser console for error messages
3. Verify API responses using developer tools
4. Check server logs for API errors
