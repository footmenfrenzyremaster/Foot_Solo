# Red-Only Game Setup

Implemented in `releases/799W-tester-recode-pass-21.w3x`.

## What Red Sees

One second after loading, Red receives two choices:

1. `Pub Default (SD)` starts Single Draft immediately with no optional modes.
2. `Pub Custom` selects the existing setup unit and lets Red choose optional modes, then SD, AP, or AR.

If Red does nothing for ten seconds, the game selects Pub Default and starts Single Draft once. Other players cannot make setup decisions.

## Retained Options

Pub Custom contains exactly three optional purchases:

- `2K / 777`: every active player starts with 2,000 gold.
- `No Transmute`: removes Falcon, Alchemist, Pander Prospero, Pirate, Corruptor, and Witch Doctor from SD, AP, and AR.
- `No Pool`: keeps normal resource trading locked and disables the `-pool` autopool system.

The options can be combined. Red then presses the SD, AP, or AR ability on the setup unit to start.

KOTH, Colossals, Inhouse, Pro, 1v1, balanced units, Madness, Strong, Overdrive, and the old debug-mode startup branches are reset off. Their main trigger implementations are no longer part of live setup.

## Tavern Capacity

AP tavern `n01A` and SD tavern `n02I` retain object field `utco = 12` from pass 10.

Normal SD deals one hero from each of eleven categories plus one unique wildcard. With No Transmute, category 8 is absent, so SD deals two unique non-Transmute wildcards instead. Both paths offer twelve heroes in each team tavern.

## Spawn Systems

The individual per-unit scheduler remains the live spawn system. The old grouped `eight`, `five`, `ten`, `eight_new`, `ten_new`, and `twelve_new` trigger bodies are retired. The separate mass-bonus system and `eight_mass_bonus` remain active.

## Runtime Test Checklist

- Leave Red idle and confirm Pub Default starts SD once at ten seconds.
- Choose Pub Default before ten seconds and confirm the fallback does nothing later.
- Choose Pub Custom and confirm only 2K, No Transmute, and No Pool are offered.
- Confirm non-Red players cannot purchase options or press SD/AP/AR.
- Test SD normally and confirm all four taverns show twelve distinct heroes.
- Test SD with No Transmute and confirm twelve choices with none of the six excluded heroes.
- Test AP and AR with No Transmute and confirm none of the six excluded heroes appear.
- Test 2K and confirm active players start at 2,000 gold.
- Test No Pool and confirm resource trading stays locked and `-pool` cannot transfer gold.
- Confirm ordinary Pub Custom without No Pool unlocks trading after 230 seconds.
- Confirm normal spawns and mass-bonus spawns both still run.
- In AR, use `-repick` and confirm the replacement matches the player's initial AR hero type.

## Editor Warning

Pass 21 is a packed test build. Saving it in World Editor will regenerate `war3map.j` from the unchanged GUI trigger data and remove the injected recode.
