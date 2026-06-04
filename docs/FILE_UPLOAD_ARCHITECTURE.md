# File Upload UI - Architecture & Components Reference

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    USER INTERFACE LAYER                      │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Views/Player/Edit.cshtml                                    │
│  ├── Player Form                                             │
│  └── @await Html.PartialAsync("_AttachmentUpload") ────┐   │
│                                                        │    │
│  Views/Shared/_AttachmentUpload.cshtml                 │    │
│  ├── Dropzone Area                                     │    │
│  ├── Progress Bar                                      │    │
│  ├── Alert Container                                   │    │
│  └── File List                                         │    │
│      └── Connected to:                                 │    │
│          ├── CSS: attachment-upload.css  ◄──────┐    │    │
│          └── JS:  attachment-upload.js   ◄──┐   │    │    │
│                                           │   │    │    │
└───────────────────────────────────────────┼───┼────┼────┘
                                            │   │    │
                      ┌─────────────────────┘   │    │
                      │                         │    │
┌─────────────────────v─────────────────────────v────┴─────┐
│                 FRONTEND LAYER                            │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  wwwroot/js/attachment-upload.js                          │
│  ├── AttachmentUpload.init()  ◄────────────┐             │
│  ├── handleFiles()            │ Event       │             │
│  ├── uploadFiles()            │ Handlers    │             │
│  ├── deleteFile()             │             │             │
│  └── DOM Manipulation         │             │             │
│                               │             │             │
│  wwwroot/css/attachment-upload.css         │             │
│  ├── .dropzone-wrapper        │ Styling    │             │
│  ├── .file-item               │ for        │             │
│  ├── .progress-bar            │ Components │             │
│  └── Responsive Rules         │             │             │
│                               │             │             │
│  User Interactions            └─────────────┘             │
│  ├── Drag & Drop                                          │
│  ├── Click Upload                                         │
│  ├── Delete Action                                        │
│  └── Download Click                                       │
│                                                            │
└─────────────────────┬──────────────────────────────────────┘
                      │
        ┌─────────────v──────────────┐
        │   AJAX Requests (Fetch)    │
        └─────────────┬──────────────┘
                      │
    ┌─────────────────┼─────────────────┐
    │                 │                 │
    v                 v                 v
┌──────────┐    ┌──────────┐    ┌──────────────┐
│ POST     │    │ GET      │    │ DELETE       │
│ Upload   │    │ ListFiles│    │ Remove       │
└─────┬────┘    └────┬─────┘    └─────┬────────┘
      │              │                │
      └──────────────┼────────────────┘
                     │
┌────────────────────v────────────────────────────────┐
│            API LAYER (ASP.NET Core)                 │
├─────────────────────────────────────────────────────┤
│                                                      │
│  /api/attachments                                   │
│  │                                                  │
│  ├─ POST   → AttachmentsApiController.Upload()      │
│  │          ├─ Validate MIME type                   │
│  │          ├─ Check file size (max 10MB)           │
│  │          ├─ Sanitize filename                    │
│  │          ├─ Save to wwwroot/uploads/             │
│  │          └─ Insert Attachment record             │
│  │                                                  │
│  ├─ GET    → AttachmentsApiController.GetAll()      │
│  │          ├─ Filter by entityType                 │
│  │          ├─ Filter by entityId                   │
│  │          └─ Return AttachmentDto list            │
│  │                                                  │
│  └─ DELETE → AttachmentsApiController.Delete()      │
│             ├─ Check Authorization (Admin role)     │
│             ├─ Delete file from disk                │
│             └─ Remove Attachment record             │
│                                                     │
└────────────────────┬────────────────────────────────┘
                     │
        ┌────────────v───────────┐
        │   DATA LAYER           │
        └────────────┬───────────┘
                     │
        ┌────────────┴───────────┐
        │                        │
        v                        v
┌─────────────────┐      ┌─────────────────┐
│   File System   │      │   Database      │
│                 │      │                 │
│ wwwroot/uploads/│      │ DbSet<Attachment>
│ ├─ Player/      │      │ ├─ Id            │
│ │  ├─ 1/        │      │ ├─ EntityType    │
│ │  │ ├─ abc...pdf      │ ├─ EntityId      │
│ │  │ └─ def...jpg      │ ├─ FileName      │
│ │  └─ 2/        │      │ ├─ FilePath      │
│ ├─ Club/        │      │ ├─ ContentType   │
│ │  └─ 1/        │      │ ├─ FileSize      │
│ ├─ Staff/       │      │ └─ CreatedAt     │
│ │  └─ 1/        │      │                 │
│ └─ Match/       │      │ MockRepository  │
│    └─ 1/        │      │ or DbContext    │
│                 │      │                 │
└─────────────────┘      └─────────────────┘
```

## Component Communication Flow

```
USER                FRONTEND              API                 STORAGE
│                      │                  │                      │
│ 1. Drag file         │                  │                      │
├─────────────────────>│                  │                      │
│                      │ 2. Validate      │                      │
│                      │    (MIME, size)  │                      │
│                      │ 3. POST file     │                      │
│                      ├─────────────────>│                      │
│                      │                  │ 4. Validate          │
│                      │                  │    (secure)          │
│                      │                  │ 5. Save to disk      │
│                      │                  ├─────────────────────>│
│                      │                  │                      │ 6. File written
│                      │                  │                      │    with GUID name
│                      │                  │ 7. Save to DB        │
│                      │                  ├─────────────────────>│
│                      │                  │                      │ 8. Record inserted
│                      │                  │ 9. 201 Created       │
│                      │<─────────────────┤                      │
│ 10. Show success     │<─────────────────┤                      │
│<────────────────────┤                  │                      │
│                      │ 11. GET files    │                      │
│                      ├─────────────────>│                      │
│                      │                  │ 12. Query DB         │
│                      │                  ├─────────────────────>│
│                      │                  │                      │ 13. Return records
│                      │                  │<─────────────────────┤
│                      │ 14. JSON list    │                      │
│                      │<─────────────────┤                      │
│ 15. Render list      │<─────────────────┤                      │
│<────────────────────┤                  │                      │
│                      │                  │                      │
│ 16. Click delete     │                  │                      │
├─────────────────────>│                  │                      │
│                      │ 17. DELETE {id}  │                      │
│                      ├─────────────────>│                      │
│                      │                  │ 18. Check auth       │
│                      │                  │ 19. Get file path    │
│                      │                  ├─────────────────────>│
│                      │                  │                      │ 20. Delete file
│                      │                  │ 21. Delete from DB   │
│                      │                  ├─────────────────────>│
│                      │                  │                      │ 22. Record removed
│                      │                  │ 23. 204 No Content   │
│                      │<─────────────────┤                      │
│ 24. Remove from UI   │<─────────────────┤                      │
│<────────────────────┤                  │                      │
│                      │                  │                      │
```

## File Organization Diagram

```
Football Club Project Root
│
├── FootballClub/
│   │
│   ├── Views/
│   │   ├── Player/
│   │   │   ├── Index.cshtml
│   │   │   ├── Create.cshtml
│   │   │   ├── Edit.cshtml          ← Component integrated
│   │   │   ├── Details.cshtml        ← Component integrated
│   │   │   └── Schedule.cshtml
│   │   │
│   │   ├── Staff/
│   │   │   ├── Create.cshtml
│   │   │   ├── Edit.cshtml          ← Component integrated
│   │   │   ├── Details.cshtml
│   │   │   └── Index.cshtml
│   │   │
│   │   ├── Club/
│   │   │   ├── Index.cshtml
│   │   │   └── Details.cshtml       ← Component integrated
│   │   │
│   │   └── Shared/
│   │       ├── _Layout.cshtml
│   │       ├── _DateTimePicker.cshtml
│   │       ├── _AutocompleteDropdown.cshtml
│   │       └── _AttachmentUpload.cshtml  ← NEW! Component
│   │
│   ├── Web/
│   │   ├── Controllers/Api/
│   │   │   └── AttachmentsApiController.cs  ← API endpoints
│   │   │
│   │   ├── Dto/
│   │   │   └── AttachmentUploadDto.cs
│   │   │
│   │   └── Program.cs
│   │
│   ├── Models/
│   │   └── Attachment.cs            ← Entity model
│   │
│   └── wwwroot/
│       ├── css/
│       │   ├── site.css
│       │   └── attachment-upload.css     ← NEW! Styling
│       │
│       ├── js/
│       │   ├── site.js
│       │   └── attachment-upload.js      ← NEW! JavaScript
│       │
│       ├── uploads/                 ← File storage root
│       │   ├── Player/
│       │   │   ├── 1/
│       │   │   │   ├── a1b2c3d4_passport.pdf
│       │   │   │   └── e5f6g7h8_photo.jpg
│       │   │   └── 2/
│       │   │       └── ...
│       │   │
│       │   ├── Club/
│       │   │   └── 1/
│       │   │       └── badge_abc123.png
│       │   │
│       │   └── Staff/
│       │       └── ...
│       │
│       └── images/, lib/, favicon.ico
│
├── FootballClub.Tests/
│   └── Api/
│       └── AttachmentsApiTests.cs    ← API tests
│
├── FILE_UPLOAD_UI.md                ← NEW! Full documentation
├── QUICK_FILE_UPLOAD_INTEGRATION.md ← NEW! Quick start
├── FILE_UPLOAD_IMPLEMENTATION_SUMMARY.md  ← NEW! Summary
└── README.md
```

## Component Dependencies

```
_AttachmentUpload.cshtml
├── Requires: wwwroot/css/attachment-upload.css
├── Requires: wwwroot/js/attachment-upload.js
├── Requires: /api/attachments endpoint
├── Requires: ViewBag.EntityType
├── Requires: ViewBag.EntityId
└── Optional: ViewBag.AllowMultiple (default: true)

attachment-upload.js
├── Requires: Fetch API (modern browsers)
├── Requires: DOM with id="fileItems", "uploadProgress", etc.
├── Requires: API endpoint /api/attachments
└── No external dependencies (vanilla JS)

attachment-upload.css
├── Requires: Bootstrap compatible HTML
├── Requires: SVG for dropzone icon
└── No external dependencies
```

## Data Flow Diagram

```
┌────────────────┐
│  User Action   │
│ (Drag/Click)   │
└────────┬───────┘
         │
         v
┌────────────────────────────┐
│ JavaScript Event Handler   │
│ (dragover/drop/click)      │
└────────┬───────────────────┘
         │
         v
┌────────────────────────────┐
│ File Validation            │
│ • MIME type check          │
│ • Size validation          │
│ • Error handling           │
└────────┬───────────────────┘
         │
    ┌────┴────┐
    │          │
    v          v
  Valid      Invalid
    │          │
    │          v
    │      ┌──────────────┐
    │      │ Show Alert   │
    │      │ (Error)      │
    │      └──────────────┘
    │
    v
┌────────────────────────────┐
│ Create FormData            │
│ • entityType               │
│ • entityId                 │
│ • file blob                │
└────────┬───────────────────┘
         │
         v
┌────────────────────────────┐
│ Show Progress Bar          │
│ Display: "Uploading..."    │
└────────┬───────────────────┘
         │
         v
┌────────────────────────────┐
│ POST /api/attachments      │
│ (AJAX Request)             │
└────────┬───────────────────┘
         │
    ┌────┴────────┐
    │             │
    v             v
 Success       Error
    │             │
    │             v
    │         ┌──────────────┐
    │         │ Show Alert   │
    │         │ (Error msg)  │
    │         └──────────────┘
    │
    v
┌────────────────────────────┐
│ Parse Response JSON        │
│ Get attachment ID          │
└────────┬───────────────────┘
         │
         v
┌────────────────────────────┐
│ Add to File List           │
│ • Create DOM element       │
│ • Insert into list         │
│ • Attach event listeners   │
└────────┬───────────────────┘
         │
         v
┌────────────────────────────┐
│ Show Success Alert         │
│ Auto-dismiss after 5 sec   │
└────────────────────────────┘
```

## Security Architecture

```
                    User Input
                        │
                        v
┌──────────────────────────────────────┐
│   CLIENT-SIDE VALIDATION             │
├──────────────────────────────────────┤
│ ✓ MIME type check                    │
│ ✓ File size validation (10MB)        │
│ ✓ File extension whitelist           │
│ ✓ XSS prevention (HTML escape)       │
└──────────────────────────────────────┘
                        │
                        v
        ┌───────────────────────────┐
        │  AJAX Request             │
        │ (FormData sent to API)    │
        └───────────────┬───────────┘
                        │
                        v
┌──────────────────────────────────────┐
│  SERVER-SIDE VALIDATION              │
├──────────────────────────────────────┤
│ ✓ Authentication check               │
│ ✓ Authorization check (role)         │
│ ✓ MIME type validation               │
│ ✓ File size validation               │
│ ✓ Filename sanitization              │
│ ✓ Path traversal prevention          │
│ ✓ CSRF token validation              │
└──────────────────────────────────────┘
                        │
                        v
┌──────────────────────────────────────┐
│  FILE STORAGE                        │
├──────────────────────────────────────┤
│ ✓ GUID-prefixed filename             │
│ ✓ Entity-based directory structure   │
│ ✓ Protected directory permissions    │
│ ✓ Outside webroot (secure location)  │
└──────────────────────────────────────┘
```

## API Contract

```
Endpoint: /api/attachments

┌─────────────────────────────────────────────────────┐
│ POST /api/attachments                               │
├─────────────────────────────────────────────────────┤
│ Content-Type: multipart/form-data                   │
│ Authorization: Bearer token (Auth required)         │
│ Required Role: Admin, User                          │
│                                                     │
│ Request Body:                                       │
│ ├─ entityType (string)                              │
│ ├─ entityId (int)                                   │
│ └─ file (IFormFile)                                 │
│                                                     │
│ Response (201 Created):                             │
│ {                                                   │
│   "id": 1,                                          │
│   "entityType": "Player",                           │
│   "entityId": 1,                                    │
│   "fileName": "document.pdf",                       │
│   "filePath": "uploads/Player/1/uuid_document.pdf", │
│   "contentType": "application/pdf",                 │
│   "fileSize": 102400,                               │
│   "createdAt": "2026-05-31T10:30:00Z"               │
│ }                                                   │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│ GET /api/attachments?entityType=X&entityId=Y       │
├─────────────────────────────────────────────────────┤
│ Authorization: None (Public)                        │
│                                                     │
│ Response (200 OK):                                  │
│ [                                                   │
│   {                                                 │
│     "id": 1,                                        │
│     "entityType": "Player",                         │
│     "entityId": 1,                                  │
│     "fileName": "document.pdf",                     │
│     "filePath": "uploads/Player/1/uuid_document",   │
│     "contentType": "application/pdf",               │
│     "fileSize": 102400,                             │
│     "createdAt": "2026-05-31T10:30:00Z"             │
│   }                                                 │
│ ]                                                   │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│ DELETE /api/attachments/{id}                        │
├─────────────────────────────────────────────────────┤
│ Authorization: Bearer token (Auth required)         │
│ Required Role: Admin                                │
│                                                     │
│ Response (204 No Content)                           │
│ (Empty body)                                        │
└─────────────────────────────────────────────────────┘
```

---

This comprehensive architecture diagram shows how all components connect and communicate in the File Upload UI system.
