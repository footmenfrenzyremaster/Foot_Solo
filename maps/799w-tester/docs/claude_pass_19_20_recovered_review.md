# Recovered Claude Pass 19/20 Review

## Recovery Result

Claude's local work was stored under:

`C:\Users\Ryan1\.claude\projects\C--Users-Ryan1-Documents-Warcraft-III-Maps-799b`

The relevant workflow was `wf_9f061d86-721`. It launched ten review agents and recorded roughly 773,000 tokens across 182 tool calls, but every agent ended at the session limit and the workflow produced no final findings. Several prompts also referenced broken `undefined/review1920/...` paths.

Verdict: the run was inefficient as a completed review, but its partial reasoning was useful as a list of leads. No lead was accepted without independent inspection of the actual Pass 19/20 script.

## Confirmed And Fixed In Pass 21

| Lead | Independent result | Pass 21 action |
|---|---|---|
| Global inactive cleanup can remove another team's `h02Q` controls | Confirmed major defect | Replaced the global switch with source-team/base-aware per-unit cleanup |
| Later cleanup can remove defeated players' `n02G` center vision | Confirmed | Preserved vision for connected spectators and removed it on departure |
| Votekick dialogs can outlive player/base state | Confirmed | Revalidated initiator, voter, and target at click time |
| Autopool dialogs can outlive source participation | Confirmed | Revalidated source at both selection and threshold clicks |
| Autopool threshold destroys stale `udg_temp_playergroup` | Confirmed pre-existing defect | Removed anonymous force creation and stale shared-force destruction |
| Reviewed votekick paths allocate temporary forces | Confirmed | Switched to direct player text and persistent `udg_all_players` |
| Departing base-less players can leave transient controls behind | Confirmed minor defect | Added targeted `h02Q`/`n02G` departure cleanup |
| Visibility trigger action is dead and unlinked | Confirmed | Removed the action and retained a disabled compatibility initializer |

## Refuted Leads

- AR does not read lifecycle state before initialization; bases and `udg_player_active` are initialized before mode selection.
- Replacing Votekick's temporary all-player force with `udg_all_players` does not alter eligibility; `udg_all_players` is initialized from `GetPlayersAll()`.
- Pass 20 preserves shared abandoned-base control through `bj_ALLIANCE_ALLIED_ADVUNITS`.
- Pass 20's integer gold remainder distribution preserves the complete source total.
- Pass 19's Unit Indexer filter and all 25 `bj_wantDestroyGroup` paths have valid ownership/consumption.
- Pass 19's Cripple Wave, Mirror Image, Purge the Dead, scroll, upgrade, Frostbolt, and base-check ownership changes are internally consistent.
- Removed KOTH departure handling is intentional because KOTH was retired from live setup.

## Residual Runtime Questions

- The one-shot inactive cleanup model should still be exercised around delayed spell dummies; no concrete unsafe delayed unit was found statically.
- Final-strike attribution still uses the inherited nested-trigger event response. Static parsers accept it, but Warcraft should confirm the winner message's killer name.
- Zero surviving teams produces no winner, matching prior behavior. This mainly covers all-player-abandonment and remains a design choice rather than a confirmed defect.

## Verified Output

- Pass 21 map: `releases/799W-tester-recode-pass-21.w3x`
- Pass 21 script: `build/war3map.recode-pass-21.j`
- Detailed checks: `build/player-lifecycle-review-pass-21/verification.md`
- Script SHA256: `525c1f7db454ed56c8dcac8f639d08f14215028162e0c63acb35d72fc90b1bd8`
- Map SHA256: `c2b1d6ee74b368f3a2504cf8ad3938fe408b53d1db2a7b6ca7f4520995831667`
