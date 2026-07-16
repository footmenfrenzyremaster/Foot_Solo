# Pass 17/18 External Verification (Claude Code session, 2026-07-15)

Independent review of the pass-17 and pass-18 builds produced on 2026-07-14
between 14:32 and 14:51. Performed with fresh tooling (`work/JassParseCheck`,
War3Net JASS parser) plus the existing `MpqExtract` and `JassAudit` tools.

## Verified — Pass 17 (`claude-review-pass-17`)

- Diff against pass 16 is exactly the confirmed external-review findings:
  shop-kill push center now `udg_centermap`, both Mass autosend callbacks now
  `udg_centermap`, Scroll of Beast enumerates `GetPlayableMapRect()`, Akama
  return nulls both released point slots, AFK comment corrected, and the two
  unlabeled vote stubs are labeled.
- The River Staff impact rewrite is correct: `s__Missiles_setup` repopulates
  every `udg_Missile*` global from the current missile instance before firing
  the OnHit trigger, so `udg_MissileSource` and
  `AngleBetweenPoints(udg_MissileStart, udg_MissileFinish)` are true
  per-missile values. `AngleBetweenPoints(start, finish)` equals the original
  cast-time `udg_Jungle_Angle` because both points were projected along that
  angle from the same origin. Overlapping casts by two Monkey Kings no longer
  interfere (review checklist item 11 resolved).
- `udg_MonkeyKing` now has zero operational references; `udg_Jungle_Angle` is
  written and consumed within a single cast dispatch only.
- War3Net syntax parse: OK (8,563 top-level declarations).

## Verified — Pass 18 (`guardian-force-field-pass-18`)

- War3Net syntax parse: OK (8,553 top-level declarations).
- Wall count mapping 1/1/3/3/5/5 confirmed in `GuardianFF_GetWallCount` plus
  the center wall created unconditionally at cast.
- The original defect is visible in the pass-17 baseline: the old damage
  branch set its side-wall loop bound to 1 in *both* branches of its level
  check, so levels 5-6 never stomped their outer walls, and expiry-side points
  could be stale from earlier casts. The rewrite derives side points
  deterministically from each instance's stored center and angle.
- Instance compaction is correctly ordered: the expiring child's hashtable
  effects are destroyed and flushed *before* the final instance's handles are
  moved into that slot; the vacated last slot is cleared without a second
  `RemoveLocation` (ownership moved). The `Loop - 1` re-process step gives the
  moved instance exactly one duration decrement per tick — no double-tick.
- The damage trigger reads instance state via `udg_Guardian_FF_Loop` while
  `TriggerExecute` runs synchronously and the instance arrays are released
  only afterwards — no use-after-free.
- Detonate lifecycle: Learn adds `AFFD` permanently but disabled; cast enables
  at the caster's first active field; expiry/detonate disables at zero fields.
  `gg_trg_DDE_Destroy_Effect` (used for effect teardown) is live, not a stub.
- Faithful oddities preserved, not regressions: trigger damage is
  `0.00 * level` (the stun comes from the `AFFS` stomp dummy) and the two
  branches use `DAMAGE_TYPE_MAGIC` vs `DAMAGE_TYPE_ENHANCED` exactly as the
  original did.
- Round trip: packed `war3map.j` SHA256
  `9fba8317db97a7b30e6112a7f0e191b0ad881e88c750f8408380a9c7d7478227` matches
  `build/war3map.recode-pass-18.j` byte for byte; `war3map.w3u` unchanged from
  the pass-16 baseline (`98647d0a...`); release and the Warcraft Maps copy are
  identical (`3d7dde8a...`); only the pre-existing broken `(listfile)` failed
  extraction.

## Notes for future passes

- `work/JassParseCheck` is a new offline syntax checker (War3Net
  `JassSyntaxFactory.ParseCompilationUnit`, cached NuGet package, no network).
  Usage: `dotnet run --project work/JassParseCheck -- <script.j> [...]`.
- The pass-17 verification noted that strict semantic parsing was impossible
  because `common.j`/`Blizzard.j` were missing — they do exist locally at
  `C:\Users\Ryan1\Documents\Warcraft III\JassHelper\` if a semantic pjass run
  is wanted later.
- Residual low-priority items from the pass-16 external review that remain
  open: inline `Condition(function ...)` boolexpr leaks in
  `GetUnitsInRangeOfLocMatching` call sites, the missing 512-byte HM3W header
  (matters only to some hosting bots), and the dead `InitTrig_Camera_Zoom`
  stub.
- Two agents worked this workspace concurrently on 2026-07-14 (~14:1x-14:5x)
  and produced duplicate pass-17 converters; the duplicates
  (`work/RecodeReviewFixesPass17`, `build/review-fixes-pass-17`,
  `src/systems/review_fixes_pass_17.jass`) were removed after confirming
  `RecodeClaudeReviewPass17` supersedes them. Check for concurrent runs before
  starting a new pass.
