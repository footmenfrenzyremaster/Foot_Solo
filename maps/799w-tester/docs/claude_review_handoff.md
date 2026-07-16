# Claude Review Handoff - WC3 799 Pass 21

## Review Target

- Map: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\releases\799W-tester-recode-pass-21.w3x`
- Warcraft test copy: `C:\Users\Ryan1\Documents\Warcraft III\Maps\799b\799W-tester-recode-pass-21.w3x`
- Script: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\build\war3map.recode-pass-21.j`
- Mode source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\mode_pruning_pass_12.jass`
- Runtime source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\runtime_location_cleanup_pass_13.jass`
- Startup source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\startup_dead_code_pass_14.jass`
- Handle-cleanup source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\local_handle_cleanup_pass_15.jass`
- Spell-lifecycle source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\spell_lifecycle_cleanup_pass_16.jass`
- External-review source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\claude_review_fixes_pass_17.jass`
- Guardian source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\guardian_force_field_pass_18.jass`
- Enumeration source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\enumeration_ownership_pass_19.jass`
- Player-lifecycle source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\player_lifecycle_pass_20.jass`
- Review-fix source: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\src\systems\player_lifecycle_review_pass_21.jass`
- Repeatable converter: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\work\RecodePlayerLifecycleReviewPass21`
- Verification: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\build\player-lifecycle-review-pass-21\verification.md`
- Recovered-review disposition: `C:\Users\Ryan1\Documents\Codex\2026-07-09\work-o\outputs\WC3-799\docs\claude_pass_19_20_recovered_review.md`

## User Requirements

- No voting; Red owns setup.
- Ten seconds of Red inactivity starts Pub Default / SD.
- Pub Custom retains SD/AP/AR hero selection.
- Keep only 777/2K, No Transmute, and No Pool as optional modes.
- Balanced mode is unused.
- Preserve twelve-choice taverns and individual per-unit spawn rates.

## Implemented

- Main setup now has only Pub Default and Pub Custom.
- Pub Custom stocks only rawcodes `h03Z` (2K), `h03V` (No Transmute), and `h04B` (No Pool).
- No Transmute uses fixed catalog indices 18, 21, 52, 55, 78, and 88 rather than mutating hero category metadata.
- SD, AP, and AR all filter those heroes. No Transmute SD uses two wildcard rounds so each tavern still reaches twelve choices.
- No Pool keeps `MAP_LOCK_RESOURCE_TRADING` enabled, never starts the trading-unlock timer, and disables the autopool command/execute/tick triggers.
- Start game always creates the standard multiboard. KOTH and Colossal startup branches are gone.
- Thirty-seven unsupported trigger implementations plus two retired vote triggers are disabled compatibility stubs, for 39 total.
- `eight_mass_bonus` and the individual spawn scheduler remain unchanged.
- All 499 `InitTrig_` entry points remain so `InitCustomTriggers` still initializes every expected global trigger handle.
- Base, shop-block, shop-kill, and Phoenix boundary movement no longer allocates temporary locations.
- The shop-kill helper preserves each team warning, Ankh removal, inward 350-distance move, and kill order.
- Mass autosend no longer allocates a map-center location per footman.
- Both Mass Start branches explicitly destroy every player's enumerated footman group.
- Start Game's per-player callback now contains only its live starting-gold assignment.
- Removed the unreachable ten-base availability block guarded by an eventless trigger-unit test.
- Removed the player-13 level-zero `R00N` reset and its unobserved 280-second timer.
- The actual `R00D`, `R00G`, and `R00E` T1/T2/T3 timer triggers remain unchanged.
- River Staff impact and Alchemist bounty release their temporary locations immediately after use.
- Thor Powerhit releases each projected point before replacing it while preserving the original movement sequence.
- Jaina's ultimate keeps the frost, fire, and lightning orb handles local to each cast across timed waits.
- Jaina's temporary order points and lightning target group are released after use.
- Debug mode, debug commands, and debug control release their temporary points and selected-unit groups.
- River Staff reuses its existing caster point instead of creating an unowned inline missile-start source point.
- Akama Blink releases its stored return point when the five-second return window expires unused.
- The Nook and `MissileCreate` ownership paths were traced and left unchanged because they already release their locations.
- All four shop-kill zones now push 350 units toward `udg_centermap`; the prior corner target was inherited from the original map.
- Scroll of Beast now enumerates the playable map instead of the 64x96 `gg_rct_ENTIRE_MAP` corner region.
- Both Mass footman callbacks target the same center coordinates as hero autosend.
- River Staff impact reads its caster and angle from the active missile instance, making overlapping Monkey King casts independent.
- Akama Blink Return clears its released point slot, and the setup comment now correctly says ten seconds.
- Guardian Force Field now follows the six-level `AFOR`/`AFFS` object data and creates 1/1/3/3/5/5 walls.
- Side walls are derived from each field's own center and angle; the old cast path used uninitialized locations and an impossible level 7-8 branch.
- Expiry and damage use the current loop instance; the old code read the newest field's level in several branches.
- Field removal clears its pathing, locations, effects, and hashtable child, then safely compacts the last active field into the open slot.
- Detonating one Guardian's fields leaves other Guardians' active fields intact.
- Warcraft's three matching-group BJ wrappers were verified to destroy inline boolexpr filters; those correct call sites remain unchanged.
- The Unit Indexer's direct initial enumeration reuses one filter across sixteen players and destroys it afterward.
- All 18 previously unowned inline temporary groups now have an immediate `bj_wantDestroyGroup` flag.
- Cripple Wave releases its own copied points instead of Searing Bullet's point slots.
- Mirror Image no longer removes missile finish locations a second time after `MissileCreate` owns them.
- Purge the Dead uses matching-unit context, one target point, and destroyed corpse effects.
- Scroll of Invisibility and Scroll of Inner Fire no longer destroy stale shared group state.
- Team upgrade propagation owns each temporary unit group and one allied-player force; every ally receives the ping and message.
- Frostbolt tower upgrades enumerate their actual towers instead of an unrelated stale group.
- Base-destruction checks release their alive-town-hall group and allied-player force.
- Player connection, base survival, and team defeat are now separate states.
- A connected player whose own base is gone keeps the team alive while any allied base remains, including a departed ally's base.
- Base death marks only the owner inactive and preserves that player's abandoned-base controls until their mapped base dies.
- Departures and admin kicks share one idempotent handler, preventing duplicate `h02Q` controls.
- Abandoned-base collect/destroy actions validate their mapped source and share one gold-recipient calculation.
- Gold remainders are distributed instead of discarded.
- Departed players are removed from stale autopool links and rejected by AR, swap, votekick, and autopool participation checks.
- Inactive cleanup and defeated-team visibility now run once per transition.
- Victory requires exactly one surviving team and is guarded against duplicate execution.
- Retired KOTH departure logic and the unwritten 6v6 result branch are gone from the lifecycle.
- Pass 20 removed 114 generated helpers and added 28 named lifecycle helpers while retaining all 499 trigger initializers.
- Pass 21 replaces the global remove-controls switch with per-control source-team and base-state validation.
- Defeating one team no longer removes another surviving team's valid `h02Q` controls.
- Connected defeated players retain `n02G` center vision across later cleanup, and departure removes their transient controls/vision.
- Votekick and autopool revalidate current participation when each dialog is clicked.
- Autopool threshold selection no longer leaks a force or destroys stale shared force state.
- The dead Visibility action is removed while its disabled compatibility initializer remains.

## Verification

- Script audit: 44,873 lines, 2,500 functions, 499 trigger initializers.
- Reachability snapshot: 1,607 reachable and 893 unreachable functions.
- Static leak candidates: 26; no Pass 21 repair is listed.
- Strict `pjass` with `common.j` and `Blizzard.j`: parse successful.
- Bundled Warcraft `pjass +nosemanticerror`: parse successful.
- War3Net parser: successful with 8,379 top-level declarations.
- Modeled cross-team control, vision persistence, departure cleanup, and stale-dialog scenarios: passed.
- Script SHA256: `525c1f7db454ed56c8dcac8f639d08f14215028162e0c63acb35d72fc90b1bd8`.
- Unit object SHA256: `98647d0a0c2e8eeb8eb7f414e39afb4ef8dcd163ec6cc6c38719085ad03d6406`.
- Ability object SHA256: `a4f45afa6f0807f5fd6a6062351034eb50fe446be3d3184da7cf491122260df4`.
- GUI trigger SHA256: `36b2ec89289c98c126b84f1788374040274bb06189f7f17ffb3572957b320b9c`.
- Custom trigger text SHA256: `9637fc61300dd08be82eb8d6746006307efc16ba25ab10f0bd61f4e2a6132330`.
- Map SHA256: `c2b1d6ee74b368f3a2504cf8ad3938fe408b53d1db2a7b6ca7f4520995831667`.
- Packed/extracted script, object data, GUI trigger data, and custom trigger text match byte for byte.
- 1,207 known archive files extracted; only the pre-existing broken `(listfile)` entry warned.

## Review Focus

1. Run the checklist in `docs/red_mode_setup.md` against pass 21.
2. Confirm No Transmute SD has twelve choices and excludes all six named heroes.
3. Confirm No Pool blocks both Warcraft trading and scripted autopool for the whole match.
4. Confirm ordinary Pub Custom still unlocks trading after 230 seconds.
5. Confirm individual normal spawns and mass-bonus spawns both continue.
6. Confirm disabled compatibility stubs cannot be reached by remaining endgame/player-leave branches.
7. Walk units into every team shop-block edge and confirm the expected push direction.
8. Blink into T4's shop-kill zone first, then all other zones; confirm centerward movement, Ankh removal, death, and team warning.
9. Start both normal Mass and no-mass paths and confirm footmen autosend correctly.
10. Confirm starting gold is correct and T1/T2/T3 unlock at their existing timer expirations.
11. Cast River Staff simultaneously with two Monkey Kings, then test Alchemist Chemical Rage and Thor Powerhit repeatedly.
12. Overlap two Jaina ultimate casts and confirm each cast removes only its own three orbs.
13. Test Akama Blink once with an early return and once by allowing the five-second return window to expire.
14. Cast Scroll of Beast with allied heroes spread across the map and confirm each receives the intended cooldown.
15. Cast Guardian Force Field at levels 1-6 and confirm wall counts of 1/1/3/3/5/5, correct side spacing, collision, and removal.
16. Test natural expiry and Detonate; confirm every wall stuns once, all pathing disappears, and Detonate disables when that Guardian has no fields left.
17. Create several fields from one Guardian and two Guardians, then expire an older field before a newer one; confirm remaining fields keep their own position, duration, effects, and owner.
18. Cast Cripple Wave, then Searing Bullet, and confirm each missile keeps its own trajectory and cleanup state.
19. Repeatedly cast Mirror Image and confirm every illusion moves correctly and every missile visual completes.
20. Cast Purge the Dead around normal corpses, living units, structures, and dead heroes; only eligible normal corpses should be removed.
21. Cast Anti-Magic Scroll, then Scroll of Invisibility and Scroll of Inner Fire; confirm later scrolls do not disturb prior group state.
22. Research each shared team upgrade and confirm every ally receives the tech, ping, and message exactly once.
23. Research the Frostbolt tower upgrade and confirm every eligible tower receives the intended multi-target change.
24. Destroy one teammate's base while another allied town hall remains alive; the surviving teammate and team should remain active.
25. Have one player leave with their base alive, then destroy the connected ally's own base; the team must remain active and the connected ally must retain the abandoned-base control.
26. Destroy that final abandoned base through its control; only then should the team be defeated.
27. Collect 101 abandoned gold with two eligible allies and confirm all 101 is distributed.
28. Admin-kick a player and confirm each eligible ally receives exactly one control even when the leave event follows.
29. Leave while a swap/autopool dialog is open and confirm the stale target is rejected.
30. Eliminate three teams and confirm visibility and the final result each run once.
31. Keep a departed ally's base alive under a connected base-less controller, defeat a different team, and confirm the control survives.
32. Defeat a connected team, trigger later cleanup through another death/defeat, and confirm the first team retains center vision.
33. Lose the votekick initiator, voter, or target base after opening each dialog and confirm the stale action is rejected.
34. Lose the autopool source base after opening target and threshold dialogs and confirm neither stale action applies.

## Pass 16 Review Disposition

- Finding 1: fixed. The bad shop direction was inherited from the original script; Scroll of Beast's use of the same rect was also corrected.
- Finding 2: fixed. Mass footmen and heroes now share `udg_centermap`.
- Finding 3: fixed in documentation and script labels; there are 39 compatibility stubs total.
- Akama nulling and stale AFK comment: fixed.
- Overlapping River Staff casts: fixed using per-missile source and trajectory state.
- Reusable `Condition(function ...)` expressions: resolved in Pass 19. The three matching-group BJ wrappers destroy their filters; the one direct Unit Indexer filter is now reused and destroyed.
- `InitTrig_Camera_Zoom`: left intact to preserve the generated 499-entry initializer contract; `zoom_Init()` remains active through `InitCustomTriggers`.
- HM3W lobby header: unchanged and documented as a third-party hosting compatibility risk.

## Intentionally Unchanged

- GUI trigger data is unchanged. Saving in World Editor regenerates the old `war3map.j` and removes injected passes.
- Most hero spells, remaining debug utility behavior, and generated missile libraries still need later review.

## Next Safe Work

1. Runtime-test pass 21 before changing startup, boundary, repaired spell, or player-lifecycle behavior again.
2. Exercise the full leaver-base, base-less ally, admin-kick, gold, visibility, and victory checklist above.
3. Continue high-frequency hero/item spell clusters only after each cross-trigger ownership path is documented.
