# Whole Map JASS Recode Plan

This is the working plan for turning the generated `war3map.j` into cleaner, safer source without breaking a working map.

## Current Baseline

- Original script source: `src/jass/war3map.original.j`
- Original extracted script: `extracted/799W-tester/files/war3map.j`
- First cleaned script candidate: `build/war3map.cleaned.candidate.j`
- Current mod-line cleaned script: `build/war3map.individual_spawn.cleaned.j`
- Current recode script: `build/war3map.recode-pass-3.j`
- Current runtime-recode script: `build/war3map.recode-pass-4.j`
- Current spell-runtime script: `build/war3map.recode-pass-7.j`
- Item/runtime stage script: `build/war3map.recode-pass-9.j`
- Red-mode/tavern stage script: `build/war3map.recode-pass-10.j`
- Corrected startup script: `build/war3map.recode-pass-11.j`
- Current mode-pruned script: `build/war3map.recode-pass-12.j`
- Current runtime-cleanup script: `build/war3map.recode-pass-13.j`
- Current startup-cleanup script: `build/war3map.recode-pass-14.j`
- Current local-handle-cleanup script: `build/war3map.recode-pass-15.j`
- Current spell-lifecycle-cleanup script: `build/war3map.recode-pass-16.j`
- External-review-fix stage script: `build/war3map.recode-pass-17.j`
- Guardian Force Field stage script: `build/war3map.recode-pass-18.j`
- Enumeration-ownership stage script: `build/war3map.recode-pass-19.j`
- Current reviewed player-lifecycle script: `build/war3map.recode-pass-21.j`
- Cleaned-only test map: `releases/799W-tester-cleaned-jass.w3x`
- Individual-spawn plus cleaned JASS test map: `releases/799W-tester-individual-spawn-cleaned-jass.w3x`
- Current recode test map: `releases/799W-tester-recode-pass-3.w3x`
- Current runtime-recode test map: `releases/799W-tester-recode-pass-4.w3x`
- Current spell-runtime test map: `releases/799W-tester-recode-pass-7.w3x`
- Item/runtime stage test map: `releases/799W-tester-recode-pass-9.w3x`
- Superseded Red-mode/tavern map: `releases/799W-tester-recode-pass-10.w3x` (contains a corrected-in-pass-11 JASS syntax error)
- Corrected startup/tavern test map: `releases/799W-tester-recode-pass-11.w3x`
- Current mode-pruned test map: `releases/799W-tester-recode-pass-12.w3x`
- Current runtime-cleanup test map: `releases/799W-tester-recode-pass-13.w3x`
- Current startup-cleanup test map: `releases/799W-tester-recode-pass-14.w3x`
- Current local-handle-cleanup test map: `releases/799W-tester-recode-pass-15.w3x`
- Current spell-lifecycle-cleanup test map: `releases/799W-tester-recode-pass-16.w3x`
- External-review-fix stage test map: `releases/799W-tester-recode-pass-17.w3x`
- Guardian Force Field stage test map: `releases/799W-tester-recode-pass-18.w3x`
- Enumeration-ownership stage test map: `releases/799W-tester-recode-pass-19.w3x`
- Current reviewed player-lifecycle test map: `releases/799W-tester-recode-pass-21.w3x`

## Audit Snapshot

Original script:

- 52,453 lines
- 2,891 functions
- 499 trigger initializers
- 103 initially disabled triggers
- 100 init triggers with no registered event
- 122 dormant triggers have wake-up or execute links
- 64 dormant triggers still need manual review
- 380 `DoNothing()` calls

Individual-spawn plus cleaned script:

- 52,402 lines
- 2,914 functions
- 499 trigger initializers
- 0 `DoNothing()` calls
- Extracted `war3map.j` from the packed test map matches the generated script by SHA256.

Recode pass 3:

- 50,804 lines
- 2,917 functions
- 499 trigger initializers
- 0 `DoNothing()` calls
- Hero setup reduced to 91 validated data rows with original initialization timing.
- Spawn-unit stats reduced to 51 validated data rows covering indices 0-50.
- Packed and extracted script SHA256: `fac75e44235bc2372f6834e014147861d5de7e41f24e5fa36e31eab196e8559a`.

Recode pass 4:

- 50,632 lines
- 2,889 functions
- 499 trigger initializers
- Replaced the showroom page trigger while preserving all ten page mappings.
- Removed 28 generated helper functions.
- Fixed stale shared-group destruction on unrelated sales.
- Packed and extracted script SHA256: `11fe54580a9f171369c44e5e1e7138946d5a5335d137aa5bcd3930f28270ab62`.

Recode passes 5-7:

- Sinkhole completion and reusable-cast cleanup repaired.
- Prison expiration now frees all eight persistent points once.
- Glaive movement uses coordinate math and supports simultaneous players.
- Both hook variants clean movement locations and temporary target groups.
- Copied hook no longer destroys Mjolnir state.
- 499 trigger initializers remain intact.
- Packed and extracted pass 7 SHA256: `aec3f0ca591c2bf2f5860030c3fbdb3595eaf2abc0843c1642fad2b569a4fbbb`.

Recode passes 8-9:

- Item limits reduced from 1,143 generated lines to 99 named lines.
- All 26 item rules, 20 caps, and 9 hero-restriction sets validated before replacement.
- 82 anonymous item-limit functions removed.
- Three self-contained gameplay location leaks removed.
- 49,745 script lines and 2,808 functions remain.
- All 499 trigger initializers remain intact.
- Packed and extracted pass 9 SHA256: `9ebf52f103f437ff8518323e5061570c5110b46bdd26de793b6f891a08ae52e1`.

Recode pass 10:

- Removed live player voting and made Red the sole setup controller.
- Added a ten-second Pub Default / Single Draft AFK fallback.
- Preserved Red-controlled Pub Custom special modes and Colossal configuration.
- Raised AP and SD tavern object capacity from 8 to 12.
- Added a unique twelfth Single Draft wildcard choice per team tavern.
- 49,307 script lines and 2,786 functions remain.
- All 499 trigger initializers remain intact.
- Packed script SHA256: `9a007576854df41dfe362c860628bd55ae494f9406b36a2a2cf5fc0396ce10a4`.
- Packed object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.

Pass 10 status:

- Superseded before runtime testing because the SD wildcard injection joined two JASS statements on one line.
- Bundled `pjass` reports a missing line break and syntax error at pass-10 line 17,573.

Recode pass 11:

- Corrected the malformed SD line and passed the bundled `pjass` syntax parser across the complete script.
- Repaired reviewed one-shot handle ownership in shared startup and SD/AP/AR initialization.
- Removed three SD setup counters and one obsolete AR helper.
- AP now labels all nine taverns and cannot stock undefined hero indices 92-93.
- AR now draws unique heroes and records the actual granted rawcode for repick.
- 49,320 audited lines and 2,785 functions remain; all 499 trigger initializers remain intact.
- Packed and extracted script SHA256: `1ee77a59cd44d6b8e4987af289fd9ec1a56ff209153cd18a8476b6594fee6428`.
- Packed and extracted object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed map SHA256: `87d663852503a83f35cd5ab633838c059d85efa0e45cf3090e782acd157fb387`.

Recode pass 12:

- Reduced Red's setup to Pub Default and Pub Custom.
- Pub Custom exposes only 2K, No Transmute, and No Pool before SD/AP/AR.
- No Transmute filters six cataloged heroes across all three hero modes and preserves twelve SD choices.
- No Pool now locks normal resource trading and disables scripted autopool.
- Replaced 37 unsupported mode, KOTH, Colossal, balanced, and grouped-spawn implementations with disabled compatibility stubs; together with two retired vote stubs, the map has 39 compatibility stubs total.
- Preserved the individual per-unit scheduler, `eight_mass_bonus`, tavern object data, and all 499 trigger initializers.
- 45,962 audited lines and 2,590 functions remain; static leak candidates fell from 51 to 46.
- Packed and extracted script SHA256: `cd1be9d4f08340ea730c4582222ef63d750decd69221cfd76064bfc3aafa2df3`.
- Packed and extracted object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed map SHA256: `1d6c00259c9f57ed949b2f65a7113b9206d5c09f6160a5fa7edc5d95b2df0e1b`.

Recode pass 13:

- Replaced base, shop-block, shop-kill, and Phoenix temporary point chains with direct coordinate movement.
- Preserved every movement offset, warning string, Ankh removal, and kill order.
- Replaced per-footman Mass Start center locations with coordinate orders.
- Fixed both Mass Start branches leaking one enumeration group per player.
- 45,931 audited lines and 2,592 functions remain; all 499 trigger initializers remain intact.
- Static leak candidates fell from 46 to 34.
- Packed and extracted script SHA256: `ed673f3b0205d877ce67ffc935603717d4d277b9428c78608451ffa6115d0f1d`.
- Packed and extracted object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed map SHA256: `d4e1fe9500cbd2eb68a6d79395978eff009818fc6158a8ad4a9aff5705e85430`.

Recode pass 14:

- Traced the Start Game tower branch to an eventless `GetTriggerUnit()` condition that could never pass.
- Removed the unreachable ten-base availability calls while preserving starting-gold assignment.
- Identified `R00N` as Reinforced Defenses and removed its lone level-zero write to player 13.
- Removed the 280-second upgrade-delay timer because no trigger registered its expiration.
- Preserved the real `R00D`, `R00G`, and `R00E` timed tier unlocks.
- 45,905 audited lines and 2,591 functions remain; all 499 trigger initializers remain intact.
- Packed and extracted script SHA256: `c79b987d78b73c4987c58a1750531f6ca94ae54c3cefdccfd8aa7eac5d509769`.
- Packed and extracted object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed map SHA256: `9096493d60f4b5b7fed45dd550e537ee1868e65216f20e9376e3dfa41649e8b7`.

Recode pass 15:

- Cleaned local one-shot locations in River Staff impact and Alchemist bounty.
- Preserved Thor Powerhit movement while releasing every replaced loop point.
- Made Jaina's three ultimate orbs local per cast across its timed waits.
- Cleaned Jaina's temporary points and lightning target group.
- Cleaned temporary points and selection groups in three debug helpers.
- 45,944 audited lines and 2,591 functions remain; all 499 trigger initializers remain intact.
- Static leak candidates fell from 34 to 27.
- Packed and extracted script SHA256: `23c8610b38ab23d81036da5d7556fdf66777c738eeb84e554f647ecef0534c9b`.
- Packed and extracted object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed map SHA256: `2f989352d4ab455a70e5c940bd91d083a7e613dde7816a56a24f64214ecd8b30`.

Recode pass 16:

- Reused River Staff's existing caster point for missile-start projection.
- Released Akama's stored Blink return point when its five-second return window expires unused.
- Preserved Akama's existing early-return cleanup path.
- Confirmed The Nook and the missile system already own their apparent cross-trigger location candidates.
- 45,927 audited lines and 2,591 functions remain; all 499 trigger initializers remain intact.
- Static leak candidates fell from 27 to 26.
- Packed and extracted script SHA256: `72ae5e0faacecc88b7df157f5cf4cabee7cfbcb8aa6fe8317013e7872629f604`.
- Packed and extracted object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed map SHA256: `08be008853313299f0854bf24ccbc38568af292a1f5011c568f57d3656894c0d`.

Recode pass 17:

- Corrected shop-kill movement to target `udg_centermap`; the corner target was inherited from the original map.
- Corrected Scroll of Beast to enumerate `GetPlayableMapRect()`.
- Matched Mass footman autosend coordinates to the existing hero autosend center.
- Made River Staff impact read source and trajectory from the active missile instance.
- Cleared Akama's returned point slot, labeled all 39 compatibility stubs, and corrected the stale AFK comment.
- 45,928 audited lines and 2,591 functions remain; all 499 trigger initializers remain intact.
- Conservative static leak candidates remain 26.
- Packed and extracted script SHA256: `78718ad626b6e5748be1991fcdaac2f963b1c56a4f5dd6d03fe52e6fe65ac9dd`.
- Packed and extracted object-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed map SHA256: `723866134ec9c6856ec328e2a4fb2582f6be455442ee0c88de03c87fea63365a`.

Recode pass 18:

- Rebuilt Guardian Force Field around the six levels defined by `AFOR` and `AFFS` object data.
- Restored the intended 1/1/3/3/5/5 wall pattern with deterministic center and side-wall geometry.
- Replaced impossible level branches, uninitialized side points, newest-instance lookups, and duplicate effect-handle compaction.
- Natural expiry and Detonate now remove the correct field's pathing and effects, apply its stun, and preserve other active fields.
- Removed eleven obsolete generated branch helpers.
- 45,841 audited lines and 2,586 functions remain; all 499 trigger initializers remain intact.
- Reachability snapshot: 1,638 reachable functions and 948 unreachable candidates.
- Conservative static leak candidates remain 26; the returned side point is removed by each caller.
- Packed and extracted script SHA256: `9fba8317db97a7b30e6112a7f0e191b0ad881e88c750f8408380a9c7d7478227`.
- Packed and extracted unit-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed and extracted ability-data SHA256: `a4f45afa6f0807f5fd6a6062351034eb50fe446be3d3184da7cf491122260df4`.
- Packed map SHA256: `3d7dde8a8c54b725abad21f15d05624cbd0d97c47642785b11f3d88ac1c3bfd9`.

Recode pass 19:

- Verified from Warcraft's `Blizzard.j` that matching-group wrappers destroy their inline boolexpr filters.
- Reused and destroyed the Unit Indexer's direct one-shot filter and protected all 18 remaining anonymous group enumerations.
- Corrected Cripple Wave, Mirror Image, Purge the Dead, Scroll of Invisibility, and Scroll of Inner Fire ownership errors.
- Rebuilt allied upgrade propagation, Frostbolt tower enumeration, upgrade pings, and base-survival checks with explicit group/force ownership.
- Removed one obsolete upgrade group-copy helper.
- 45,851 audited lines and 2,585 functions remain; all 499 trigger initializers remain intact.
- Reachability snapshot: 1,637 reachable functions and 948 unreachable candidates.
- Conservative static leak candidates remain 26; direct enumeration invariants are clean.
- Packed and extracted script SHA256: `e6daffec228c9587b0dc130800a5e7f6583dff87fdc8e13be68f03ba07ce8acc`.
- Packed and extracted unit-data SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed and extracted ability-data SHA256: `a4f45afa6f0807f5fd6a6062351034eb50fe446be3d3184da7cf491122260df4`.
- Packed map SHA256: `6d5247861801bf38787527daf468c48fc253624ac31bc68dd06d1385a21caff0`.

Recode pass 20:

- Separated connected-player, living-base, and team-defeat state across leave, base death, cleanup, visibility, and victory.
- Fixed the surviving abandoned-base case and preserved its controls for a connected base-less ally.
- Unified abandoned-base gold/destruction handling and made admin-kick departure idempotent.
- Rejected departed players from AR, swap, votekick, and autopool participation.
- Replaced repeated four-team visibility/endgame branches and removed retired KOTH/6v6 lifecycle paths.
- 44,828 audited lines and 2,499 functions remain; all 499 trigger initializers remain intact.
- Reachability snapshot: 1,607 reachable functions and 892 unreachable candidates.
- Conservative static leak candidates remain 26; lifecycle, swap, autopool, and kick candidates are zero.
- Packed and extracted script SHA256: `3a0a59ea43df125618a4ca7968223d76f3dce578f74c664e4ed4c5ac855d06de`.
- Packed and extracted unit-data SHA256: `98647d0a0c2e8eeb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Packed and extracted ability-data SHA256: `a4f45afa6f0807f5fd6a6062351034eb50fe446be3d3184da7cf491122260df4`.
- Packed map SHA256: `cffb863a822a30841c39b605adc46fb322f92ddb82b5379b08d8dc8d84d6d1f5`.

Recode pass 21:

- Independently recovered and classified the unfinished Claude Pass 19/20 review.
- Replaced the cross-team cleanup switch with source-team/base-aware control ownership checks.
- Preserved defeated connected players' center vision and removed transient controls/vision on departure.
- Revalidated stale votekick and autopool dialogs and repaired the autopool threshold force bug.
- Retired the uncalled Visibility action while preserving all 499 trigger initializers.
- 44,873 audited lines and 2,500 functions remain.
- Reachability snapshot: 1,607 reachable functions and 893 unreachable candidates.
- Conservative static leak candidates remain 26; no Pass 21 repair is listed.
- Packed and extracted script SHA256: `525c1f7db454ed56c8dcac8f639d08f14215028162e0c63acb35d72fc90b1bd8`.
- Packed map SHA256: `c2b1d6ee74b368f3a2504cf8ad3938fe408b53d1db2a7b6ca7f4520995831667`.

## What Is Safe So Far

- The original packed map remains untouched.
- The cleaned pass only removes whole-line `call DoNothing()` statements.
- Every current pass remains versioned and reproducible from its prior script.
- All 499 generated trigger initializer entry points remain intact.
- GUI trigger data is unchanged; current behavior lives in the injected `war3map.j`.
- Object-data edits are limited to the verified tavern-capacity build inherited by later passes.

## What Is Not Safe To Delete Blindly

- Initially disabled spell loops. Many are enabled by learn/cast triggers.
- Triggers with no registered event. Many are executed manually with `TriggerExecute` or `ConditionalTriggerExecute`.
- Generated missile/MMD/library functions. Some look unreachable to a simple call graph because they are used indirectly.
- Global groups/timers created in `InitGlobals`. Those may be persistent handles, not leaks.

## Recode Order

1. Source control and verification
   - Keep `src/jass/war3map.original.j` as the immutable reference.
   - Build candidate scripts into `build/`.
   - Pack only release/test copies under `releases/`.
   - Round-trip extract `war3map.j` after every packed build and compare hashes.

2. Mechanical safe cleanup
   - Done: remove `DoNothing()` no-ops.
   - Next: normalize generated comments/spacing only if it helps diff review.

3. Data-table migrations
   - Done: individual spawn rates.
   - Done: hero number/stat setup.
   - Done: spawn-unit stat setup.
   - Done: compact item limits and hero-specific pickup restrictions.

4. Leak and handle cleanup
   - Start with high-frequency gameplay triggers, not one-time init globals.
   - Done: Guardian Force Field lifecycle.
   - Done: matching-filter ownership and anonymous group enumerations.
   - Done: player-leave, base-survival, team-defeat, visibility, and endgame lifecycle.
   - Current top candidates: remaining item/hero effects after Pass 21 runtime testing.
   - Replace repeated `GetRectCenter`/`GetUnitLoc` patterns with locals plus `RemoveLocation`.

5. System rewrites
   - Spawn scheduler: first replacement already exists.
   - Done: Red-only mode/vote replacement and ten-second AFK fallback.
   - Done: first reviewed shared-start and SD/AP/AR repair pass.
   - Done: unsupported live mode and grouped/balanced spawn retirement.
   - Partial: hero selection and tavern pools; runtime testing and GUI-source migration remain.
   - Done: item limit and pickup rules.
   - Hero spell clusters, one hero at a time.

6. Dead-code removal
   - Only remove a trigger/function after the audit shows no event, no enable link, no execute link, no object-data dependency, and no generated-library indirect use.
   - Keep a deletion ledger with trigger name, original line, reason, and verification result.

## Tools Added

- `work/JassAudit`: whole-script trigger/function/leak audit.
- `work/JassCleanCandidate`: safe mechanical no-op cleaner.
- `work/ReplaceWar3MapScript`: generic map-script replacement builder.
- `work/RecodeHeroSetup`: validates and generates compact hero setup rows.
- `work/RecodeSpawnData`: validates and generates compact spawn-unit stat rows.
- `work/RecodePageSystem`: validates and installs the repaired showroom page selector.
- `work/RecodeSpellCleanup`: installs validated sinkhole and prison cleanup functions.
- `work/RecodeGlaiveLoop`: installs the coordinate-based multiplayer-safe glaive loop.
- `work/RecodeHookLoops`: applies validated lifecycle fixes to both hook variants.
- `work/RecodeItemLimits`: validates and installs the compact 26-rule item-limit system.
- `work/RecodeSimpleLocationLeaks`: applies reviewed single-location cleanup patches.
- `work/RecodeRedModeSetup`: installs Red-only setup, AFK fallback, and immediate SD/AP/AR selection.
- `work/PatchTavernCapacity`: raises AP/SD tavern stock capacity from 8 to 12.
- `work/RecodeStartupPass11`: validates and installs the corrected startup and SD/AP/AR ownership repairs.
- `work/RecodeModePruningPass12`: keeps only the requested optional modes and retires reviewed unused systems.
- `work/RecodeRuntimeCleanupPass13`: installs reviewed coordinate movement and Mass Start group ownership fixes.
- `work/RecodeStartupDeadCodePass14`: removes proven no-op startup tower, upgrade, and timer fragments.
- `work/RecodeLocalHandleCleanupPass15`: installs seven reviewed local spell/debug handle repairs.
- `work/RecodeSpellLifecyclePass16`: installs traced River Staff and Akama lifecycle repairs.
- `work/RecodeClaudeReviewPass17`: installs the confirmed Pass 16 external-review fixes.
- `work/RecodeGuardianForceFieldPass18`: validates and installs the six-level, instance-safe Guardian Force Field lifecycle.
- `work/RecodeEnumerationOwnershipPass19`: validates and installs transient group/filter, spell, upgrade, and base-survival ownership repairs.
- `work/RecodePlayerLifecyclePass20`: validates and installs the connected-player, abandoned-base, team-defeat, visibility, and victory lifecycle.
- `work/RecodePlayerLifecycleReviewPass21`: validates and installs the recovered cleanup, vision, votekick, and autopool review fixes.

## Next Recode Target

Runtime-test pass 21 first. Include the existing boundary, Scroll of Beast, Mass, River Staff, Akama, Guardian Force Field, Cripple Wave, Mirror Image, Purge the Dead, repaired scroll, upgrade, and Frostbolt checks. Add the Pass 21 matrix: preserve a valid abandoned-base control while an unrelated team is defeated, preserve connected defeated-player vision across later cleanup, reject stale votekick/autopool clicks, then verify final abandoned-base destruction, gold splitting, admin-kick deduplication, and one-time victory. After that, continue the next high-frequency item/hero effect cluster. Do not remove missile inputs already owned by `MissileCreate`.
