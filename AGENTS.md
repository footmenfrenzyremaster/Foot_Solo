# Codex project instructions

This repository is the GitHub source of truth for Ryan's Warcraft III projects.

## Required workflow

- Work directly in the linked folders under `maps/`.
- Preserve original/base maps and create separate test or release builds.
- Do not commit generated `build`, `builds`, `releases`, `bin`, or `obj` folders.
- After completing and verifying an in-scope WC3 change, run:

  `powershell -ExecutionPolicy Bypass -File tools/sync-github.ps1 -Message "Short description of the change"`

- The sync command commits and pushes completed work to `footmenfrenzyremaster/Foot_Solo`.
- If verification fails, do not sync a broken build merely to make the working tree clean.
