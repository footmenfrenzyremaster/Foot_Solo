# Warcraft III Map Workshop

This repository keeps the editable files for Ryan's Warcraft III maps in one safe, organized place.

## Folder layout

- `maps/799w-tester/` - the verified 799W tester/recode project
- `maps/map-source-kit/` - reusable source kit and migration project
- `shared/` - notes or tools that apply to more than one map

Each map folder keeps its own instructions. The editable script files are the source of truth; generated builds and temporary extraction folders are intentionally not stored here.

## Simple workflow

1. Make changes inside the appropriate map folder.
2. Keep the original/base map untouched.
3. Build a new test copy rather than overwriting a working map.
4. Save changes to GitHub with a short description of what changed.

## Important

Warcraft III map binaries can be large. Final releases and temporary build output are excluded by default so the repository stays reliable and easy to download.

## Syncing from Codex

The live Codex project folders are linked to the matching folders in this repository. Changes made in either project therefore appear in this Git working copy immediately.

After a completed and verified change, run:

`powershell -ExecutionPolicy Bypass -File tools/sync-github.ps1 -Message "Describe the change"`

This safely updates from GitHub, commits the WC3 changes, pushes them to `main`, and verifies that GitHub received the same commit.
