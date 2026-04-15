# Lab 2 - HTML Binding Implementation (Dinamo Zagreb Theme)

## Overview
This document describes the changes made to implement Lab 2 requirements with a custom Dinamo Zagreb blue and white theme.

## Changes Made

### 1. **UI Theme (Blue & White - Dinamo Zagreb)**
- **File**: `FootballClub/wwwroot/css/site.css`
- Changed primary color from green (#16a34a) to Dinamo blue (#0047AB)
- Updated all color variables and references throughout the CSS
- Maintained all existing components (badges, cards, tables, responsive layout)

### 2. **Layout Template - Custom Sidebar**
- **File**: `FootballClub/Views/Shared/_Layout.cshtml`
- Replaced Bootstrap navbar with custom dark sidebar layout
- Implemented fixed left sidebar (240px wide)
- Added breadcrumb navigation in topbar
- Removed Bootstrap dependencies
- Added dynamic breadcrumb generation from ViewData

### 3. **Dashboard Page (Home)**
- **File**: `FootballClub/Views/Home/Index.cshtml`
- Created custom dashboard with stat cards (using ViewBag data)
- Added top scorer card with player stats
- Added upcoming match preview card
- Added recent matches table
- Added injured players alert section
- Fully styled with custom components

### 4. **Club Pages**
- **Files**: 
  - `FootballClub/Views/Club/Index.cshtml`
  - `FootballClub/Views/Club/Details.cshtml`
- Index: Table view of all clubs with sortable data
- Details: Split-panel layout with profile card and detailed information
- Integrated squad display with player links
- Coach information linked to coach details page

### 5. **Player Pages**
- **Files**:
  - `FootballClub/Views/Player/Index.cshtml`
  - `FootballClub/Views/Player/Details.cshtml`
- Index: Filterable player list with position badges
- Details: Comprehensive player profile with stats and match history
- Status indicators (Active/Injured)
- Links to club and match details

### 6. **Match Pages**
- **Files**:
  - `FootballClub/Views/Match/Index.cshtml`
  - `FootballClub/Views/Match/Details.cshtml`
- Index: Match schedule with results and status
- Details: Score display for finished matches, upcoming match info
- Player statistics table for finished matches
- Links to clubs and player profiles

### 7. **Coach Pages**
- **Files**:
  - `FootballClub/Views/Coach/Index.cshtml`
  - `FootballClub/Views/Coach/Details.cshtml`
- Index: List of all coaches with contract info
- Details: Coach profile with career overview and contract status

### 8. **Controller Updates**
- **File**: `FootballClub/Controllers/Controllers.cs`
- Added `ViewData["ActivePage"]` to all controller actions
- Ensures proper sidebar navigation highlighting
- All controllers follow the same pattern

### 9. **UX Agent Instructions (Lab 2)**
- **File**: `FootballClub/ux-agent-instructions.md`
- Updated with Dinamo Zagreb theme specifications
- Added detailed component guidelines
- Documented the blue and white color palette
- Added Lab 2 implementation log

## Key Design Features

### Color Palette
- **Primary Blue**: #0047AB (Dinamo Zagreb official color)
- **Light Blue**: #1E7BC1 (accents and highlights)
- **Dark Navy**: #001F3F (sidebar and dark backgrounds)
- **White**: #FFFFFF (main content area)
- **Red**: #EF4444 (errors, injuries, red cards)
- **Amber**: #F59E0B (warnings and midfielder position)

### Component System
1. **Badges**: Color-coded for status (active/injured), position (GK/DEF/MID/FWD)
2. **Stat Cards**: Dashboard overview with accent colors
3. **Tables**: Custom styled `fc-table` class with hover effects
4. **Detail Grids**: Two-column layout (sidebar + content)
5. **Profile Cards**: Dark background with avatar and stats
6. **Score Display**: Emphasized match results with blue highlights
7. **Navigation**: Breadcrumbs in topbar, active page highlighting in sidebar

### Navigation Structure
- **Sidebar**: Fixed left navigation with emoji icons
- **Breadcrumbs**: Shows page hierarchy
- **Links**: All entity lists link to details pages
- **Related Data**: Details pages link to related entities

## File Structure Created

```
FootballClub/
├── Views/
│   ├── Club/
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   ├── Player/
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   ├── Match/
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   ├── Coach/
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   ├── Home/
│   │   └── Index.cshtml (updated)
│   └── Shared/
│       └── _Layout.cshtml (updated)
├── wwwroot/css/
│   └── site.css (updated)
└── Controllers/
    └── Controllers.cs (updated)
```

## Requirements Met

- ✅ **Custom UX Sub-agent**: `ux-agent-instructions.md` defines styling rules
- ✅ **All Index Pages**: Club, Player, Match, Coach with tables
- ✅ **All Details Pages**: Comprehensive entity details with related data
- ✅ **Navigation**: Complete breadcrumbs and sidebar navigation
- ✅ **Unique Non-standard UX**: Custom CSS, no Bootstrap, Dinamo branding
- ✅ **Blue & White Theme**: Dinamo Zagreb colors throughout
- ✅ **Mock Data Integration**: All mock repositories used effectively

## Testing Recommendations

1. Test navigation between all pages
2. Verify breadcrumbs display correctly on each page
3. Check responsive design on mobile (sidebar collapses to icons)
4. Verify all links to entity details work correctly
5. Test viewport behavior at 900px breakpoint
6. Validate badge styling for all position types

## Notes for Lab 2 Submission

- All views use proper MVC pattern with ViewData and Models
- No logic in views except `@if`, `@foreach`, and property access
- All color values use CSS variables (no hardcoded colors)
- Follows HTML semantic structure
- Proper breadcrumb and navigation setup for evaluation
- UX agent instructions document full design system for future enhancements
