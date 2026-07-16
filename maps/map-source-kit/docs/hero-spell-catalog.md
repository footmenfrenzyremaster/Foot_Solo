# Hero Spell Catalog

Generated from src/war3map.j, war3map.w3u, war3map.w3a, and war3map.wts.

Notes:
- Hero base stats come from Trig_set_hero_stats_Actions.
- Spell lists come from unit uhab object data when present.
- If uhab is not present, the catalog uses the inherited Warcraft III base hero spell list and marks it as inherited.
- Ability numbers are raw object-editor fields. cooldown, mana, ange, rea, duration, and hero_duration are shown as slash-separated values by level.
- data_fields are raw ability data fields such as Htc1 or Ucs3; these usually contain the actual damage, bonus, count, or scaling values, depending on the base ability.

## 1. Paladin (`H005`)

Base: `Hpal` | Category: 1 | STR: 18 + 2.5 | AGI: 13 + 1.5 | INT: 15 + 1.8 | Move: 310 | Turn: 0.6

Spell source: inherited from base Hpal

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AHhb` | Holy Light | `AHhb` | 6 | -/6/7/7/6 | 80/80/80/80/80/40 | 850/875/900/925/950/1000 |  |  |  | Hhb1=300/600/900/1200/1500/2000 |
| 2 | `AHds` | Divine Shield | `AHds` | 6 | 60/55/50/45/40/35 | 75/50/40/30/20/10 |  |  | 10/12/14/16/18/20 | 6/12/14/16/18/20 |  |
| 3 | `AHad` | Devotion Aura | `AHad` | 6 |  |  |  | 1000/1100/1200/1300/1400/1500 | 4 | 4/4/4/4/4/4 | Had1=4.5/6/8/10/15/18 |
| 4 | `AHre` | Resurrection | `AHre` | 3 | 180/190/200/120 | -/-/-/240 | -/450/500 | 1000/1300/1500 |  |  | Hre1=10/16/24/9 |

## 2. Archmage (`H008`)

Base: `Hamg` | Category: 11 | STR: 17 + 1.7 | AGI: 17 + 1.7 | INT: 19 + 2.5 | Move: 320 | Turn: 0.6

Spell source: inherited from base Hamg

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AHbz` | Blizzard | `AHbz` | 6 | 8/9/10/11/12/12 | 125/125/125/130/140/150 | -/-/925/925/925/950 | -/250/275/300/350/400 |  |  | acas=-/-/0.75/0.75/0.75/0.75; Hbz1=5/6/6/6/7/8; Hbz2=33/42/90/115/130/155; Hbz3=5/-/9/-/12/14; Hbz4=0.1/0.1/0.1/0.1/0.1/0.1; Hbz6=99999/99999/99999/99999/99999/99999 |
| 2 | `AHab` | Brilliance Aura | `AHab` | 6 |  |  |  | 1100/1350/1400/1500/1700/1800 |  |  | Hab1=2.5/3.5/6/8/12/15 |
| 3 | `AHwe` | Summon Water Elemental | `AHwe` | 6 | 25/25/25/25/25 | 150/150/150/175/175/200 |  |  | 35/35/35/35/35/35 | 35/35/35/35/35/35 | Hwe1=hwt2/-/-/h007/h007/h006; Hwe2=-/2/2/2/3/3 |
| 4 | `AHmt` | Mass Teleport | `AHmt` | 3 | 60/60/60 | 75/50/50 |  | -/800/900 |  |  | Hmt1=16/30/40; Hmt2=2/1.5/0 |

## 3. Mountain King (`H00A`)

Base: `Hmkg` | Category: 10 | STR: 22 + 2.7 | AGI: 18 + 2 | INT: 17 + 1.6 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0G9` | Thunder Clap | `AHtc` | 6 | 8/8/8/8/9/9 | 100/100/100/125/150/150 |  | 300/375/400/450/475/500 | 3.5/3.5/4/4/4 |  | Htc1=75/-/180/250/300/500; Htc3=0.2/0.25/0.3/0.35/0.4/0.45; Htc4=0.15/0.25/0.3/0.35/0.4/0.45 |
| 2 | `AHtb` | Storm Bolt | `AHtb` | 6 | 10/10/10/10/10/10 | 100/125/125/125/125/150 | -/650/675/700/725/750 |  | 1.5/2/2.5/3/3.5/4 | 1.5/2/2.5/-/3.25/3.5 | Htb1=125/-/-/450/550/800 |
| 3 | `A00D` | Bash | `AHbh` | 6 |  |  |  |  | -/3/4/4/4/5 | -/1.25/1.5/1.75/2/2 | Hbh1=10/15/20/25/30; Hbh3=-/35/45/55/65/80 |
| 4 | `AHav` | Avatar | `AHav` | 3 | 240/240/240 | -/175/200 |  |  | 45/47/50 | 45/47/50 | Hav1=-/10/15; Hav2=-/750/1000; Hav3=30/40/50 |

## 4. Blood Mage (`H009`)

Base: `Hblm` | Category: 11 | STR: 18 + 2.1 | AGI: 18 + 2.1 | INT: 18 + 2.1 | Move: 320 | Turn: 0.6

Spell source: inherited from base Hblm

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AHfs` | Flame Strike | `AHfs` | 6 | -/-/11/12/12/12 | 125/125/125/125/125/125 | -/825/875/900/900/925 | 220/230/240/250/260/280 | 4/4/4/4/4/4 | 2/2/2/2/2/2 | acas=0.2/0.2/0.2/0.2/0.2/0.2; Hfs1=5.5/6.5/11/13.5/20/29; Hfs2=0.1/0.1/0.1/0.1/0.1/0.1; Hfs3=2.2/3/4.2/6.7/7.8/8.5; Hfs4=0.1/0.1/0.1/0.1/0.1/0.1; Hfs5=0.1/0.1/0.1/0.1/0.1/0.1; Hfs6=1200/2300/3000/4000/5000/8000 |
| 2 | `AHbn` | Banish | `AHbn` | 6 | 4/4/4/4/4/3 | 125/125/125/125/125/125 | -/825/850/875/900/925 |  | -/-/-/48/60/120 | -/-/-/7/8/9 | Hbn1=0.3/0.4/-/0.6/0.7/0.8; Hbn2=0.2/0.3/0.4/0.5/0.6/0.7 |
| 3 | `AHdr` | Siphon Mana | `AHdr` | 6 | 15/15/15/15/15/15 |  | 650/675/750/800/800/800 | -/-/825/875/875/900 |  | 4/4/5/5/5 | Ndr1=-/-/-/8/10/12; Ndr2=5/7/12/17/20/25; Ndr3=0.1/0.1/0.1/0.1/0.1/0.1; Ndr4=-/-/-/5/7/9; Ndr5=7.5/9/12/15/17/20; Ndr8=0.1/0.15/0.2/0.25/0.3/0.4; Ndr9=10/10/10/10/10/10 |
| 4 | `AHpx` | Phoenix | `AHpx` | 3 | 200/200/200 | -/175/175 |  |  |  |  | Hwe1=-/h00P/h00Q; Hwe2=-/2/3 |

## 5. Blademaster (`O003`)

Base: `Obla` | Category: 5 | STR: 18 + 2 | AGI: 22 + 2.5 | INT: 17 + 1.6 | Move: 320 | Turn: 0.7

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOwk` | Wind Walk | `AOwk` | 6 | 8/8/9/9/9/9 | 90/90/90/90/90/90 |  |  | 18/20/23/25/27/30 | 18/20/23/25/27/30 | Owk2=0.2/0.25/0.3/0.35/0.45/0.55; Owk3=90/150/225/275/375/500 |
| 2 | `A00G` | Critical Strike | `AOcr` | 6 |  |  | 1500/1500/1500/1500/1500/1500 |  |  |  | Ocr1=20/20/22/23/24/25; Ocr2=-/2.5/3/3.5/4.5/6 |
| 3 | `A041` | Mirror Image | `AOmi` | 6 | 7/7/7/7/7/15 | 50/50/50/100/125 |  |  | 50/50/50/50/50/50 | 50/50/50/50/50/50 | Omi1=-/-/-/4/5/8; Omi2=-/-/-/0.1/0.2/0.4 |
| 4 | `AOww` | Bladestorm | `AOww` | 3 | -/180/180 | 175/200/225 |  | 250/300/325 | -/7/7 | 7/7/7 | Oww1=135/235/350 |

## 6. Far Seer (`O004`)

Base: `Ofar` | Category: 10 | STR: 18 + 1.8 | AGI: 18 + 1.7 | INT: 20 + 2.4 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A001` | Silence | `ANsi` | 6 | -/16/17/18/19/20 | 100/100/110/125/125/150 | 750/800/850/875 | -/250/300/-/400/400 | 9/9/9/9/9/9 | 3/4/5/6/7/8 | Nsi1=-/-/-/-/-/11 |
| 2 | `AOsf` | Feral Spirit | `AOsf` | 6 | 20/20 | -/-/-/75/75/75 |  |  |  |  | Osf1=osw2/osw3/o015/o007/o014/o001; Osf2=-/3/4/5/6/7 |
| 3 | `AOcl` | Chain Lightning | `AOcl` | 6 | -/-/11/12/12/12 | 100/110/115/125/140/150 | -/725/750/775/800/850 | 550/575/600/625/650/800 |  |  | Ocl1=115/155/220/265/330/575; Ocl2=5/8/9/11/13/15; Ocl3=0.1/0.1/0.1/0.09/0.07/0.01 |
| 4 | `AOeq` | Earthquake | `AOeq` | 3 | -/80/70 | -/140/130 | 1150/1150/1150 | 775/775/775 | 40/50/60 | 40/50/60 | Oeq2=75/100/150; Oeq3=-/0.85/0.99; Oeq4=675/675/675 |

## 7. Tauren Chieftain (`O005`)

Base: `Otch` | Category: 9 | STR: 22 + 2.6 | AGI: 18 + 1.7 | INT: 17 + 1.8 | Move: 310 | Turn: 0.6

Spell source: inherited from base Otch

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOsh` | Shockwave | `AOsh` | 6 | 9/9/11/12/12/13 | 130/130/130/135/150/200 | -/800/850/900/950/975 | -/135/145/155/165/175 |  |  | Osh1=-/-/-/270/340/570; Osh2=-/-/-/3240/4080/7200; Osh3=750/-/875/925/950/975; Osh4=-/135/145/155/165/175 |
| 2 | `AOae` | Endurance Aura | `AOae` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Oae1=0.15/-/0.25/-/0.5/0.6; Oae2=-/-/0.2/0.25/0.3/0.4 |
| 3 | `AOws` | War Stomp | `AOws` | 6 | 9/9/11/11/11/11 | 100/100/110/130/140/150 |  | 300/350/-/400/450/550 | 2/2.25/2.5/2.75/3/3.25 | -/2.25/2.5/2.75/3/3.25 | Wrs1=50/80/100/135/180/250 |
| 4 | `AOre` | Reincarnation | `AOre` | 3 | 400/380/300 |  |  |  |  |  | acas=2/2/1; Ore1=6/-/4 |

## 8. Shadow Hunter (`O006`)

Base: `Oshd` | Category: 4 | STR: 17 + 1.9 | AGI: 20 + 2.1 | INT: 20 + 2 | Move: 320 | Turn: 0.6

Spell source: inherited from base Oshd

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOhw` | Healing Wave | `AOhw` | 6 | 8.5/8.5/9.5/9.5/9.5/9.5 | 100/100/110/110/115/125 | -/725/750/775/800/825 | -/525/550/575/600/625 |  |  | Ocl1=140/220/-/385/470/750; Ocl2=4/-/-/6/7/12; Ocl3=0.2/0.2/0.2/0.2/0.15/0.05 |
| 2 | `AOhx` | Hex | `AOhx` | 6 | 8/8/8/7.5/7.5/7.5 | -/-/-/60/50/40 | 700/750/-/850/900/1000 |  | 4/5/6/7/8/9 | 2/2.5/3/3.5/4/4.5 | Ply2=npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat |
| 3 | `AOvd` | Big Bad Voodoo | `AOvd` | 3 | 160/140/120 | 70/60/50 |  | -/900/1000 | 20/20/20 | 20/20/20 |  |
| 4 | `AOsw` | Serpent Ward | `AOsw` | 6 | 9/8/7/6/6/6 | 40/45/50/55/60/60 | -/525/550/575/600/650 |  | 20/22.5/25/25/25/30 | 20/22.5/25/25/25/30 | Hwe1=-/osp1/osp2/-/osp4/osp4; Hwe2=2/3/2/3/3/4 |

## 9. Death Knight (`U002`)

Base: `Udea` | Category: 1 | STR: 20 + 2.7 | AGI: 16 + 1.5 | INT: 17 + 1.8 | Move: 320 | Turn: 0.6

Spell source: inherited from base Udea

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AUdc` | Death Coil | `AUdc` | 6 | -/-/7/8/9/5 | 100/100/100/120/150/175 | -/850/900/950/1000/1050 |  |  |  | Udc1=-/500/800/1045/1500/2070 |
| 2 | `AUdp` | Death Pact | `AUdp` | 6 |  | 0/0/0/0/0/0 | -/825/850/875/900/950 |  |  |  | Udp1=0.05/0.1/0.2/0.3/0.4/0.6; Udp2=0.75/1.5/2/-/4/5 |
| 3 | `AUau` | Unholy Aura | `AUau` | 6 |  |  |  | -/950/1000/1050/1100/1250 | 2/2/2/2/2/2 | 2/2/2/2/2/2 | Uau1=0.05/0.1/0.15/-/0.35/0.45; Uau2=2.8/4.2/6/8/10/15 |
| 4 | `AUan` | Animate Dead | `AUan` | 3 | 140/140/160 |  |  |  | 30/30/30 | 30/30/30 | Hre2=0/0/0; Uan1=8/16/24; Uan3=1/1/1 |

## 10. Lich King (`U003`)

Base: `Ulic` | Category: 9 | STR: 17 + 1.7 | AGI: 17 + 1.6 | INT: 22 + 2.5 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A00P` | Frost Nova | `AUfn` | 6 | 11/11/13/13/13/13 | -/-/135/150/160/175 | 725/775/-/825/825/850 | -/250/300/350/400/425 | 2/2.5/2.75/3/3/4 | 1/2/2.5/2.75/3/4 | Ufn1=60/130/180/240/315/570; Ufn2=110/150/175/200/225/300 |
| 2 | `AUdr` | Dark Ritual | `AUdr` | 6 | 2/1.9/1.8/1.7/1.6/1.25 | 10/15/20/-/35/50 | -/825/850/875/900/925 |  |  |  | Udp1=0.1/0.15/0.2/0.25/0.3/0.35; Udp2=-/-/-/0.1/0.15/0.2 |
| 3 | `AUfu` | Frost Armor | `AUfu` | 6 | 3/-/-/-/1.75/1 | -/-/-/-/50/60 | -/825/850/875/900/925 |  | 3/-/-/-/10/12 | 4/4/-/6/10/12 | Ufa1=40/40/-/-/-/60; Ufa2=2/3/6/9/12/15 |
| 4 | `AUdd` | Death And Decay | `AUdd` | 3 | 180/170/160 | 175/200 | 1300/1325/1350 | 500/550/600 | 15/15/15 | 15/15/15 | Udd1=0.06/0.08/0.1; Udd2=-/1/1 |

## 11. Dread Lord (`U004`)

Base: `Udre` | Category: 9 | STR: 19 + 2.4 | AGI: 17 + 1.8 | INT: 17 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AUav` | Vampiric Aura | `AUav` | 6 |  |  |  | 800/850/-/950/1000/1250 |  |  | Uav1=0.17/0.35/-/0.6/0.65000004/0.75 |
| 2 | `AUsl` | Sleep | `AUsl` | 6 | 10/10/10/10/10/10 | -/100/100/100/100/100 | 600/625/650/675/700/725 |  | -/-/-/80/100/110 | 3/4/5/6/7/8 | Usl1=0.75/1/1.1/1.2/1.3/1.4 |
| 3 | `AUcs` | Carrion Swarm | `AUcs` | 6 | -/-/11/12/12/12 | -/-/125/135/145/155 | -/725/750/775/800/800 |  |  |  | Ucs1=85/-/-/275/360/570; Ucs2=900/1500/2400/3200/4500/8000; Ucs3=-/-/-/-/-/850; Ucs4=315/330/345/360/375/390 |
| 4 | `A0HT` | Inferno | `AUin` | 3 | 200/200/200 | -/200/250 |  | -/300/350 | 2/2/2 | 1.5/1.75 | Uin1=-/100/150; Uin2=-/160/160; Uin4=-/n009/n008 |

## 12. Crypt Lord (`U005`)

Base: `Ucrl` | Category: 9 | STR: 21 + 2.5 | AGI: 16 + 1.6 | INT: 17 + 1.7 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AUim` | Impale | `AUim` | 6 | 10/10/12/12/12/12 | -/-/110/120/130/150 | 500/525/575/600/625 | 275/275/300/-/325/350 | 0.75/1/1.5/2.25/2.5/3 | -/1.5/2/2.25/2.5 | Uim1=500/525/575/-/625/700; Uim3=65/115/185/225/320/540 |
| 2 | `AUts` | Spiked Carapace | `AUts` | 6 |  |  |  |  |  |  | Uts1=0.25/0.5/0.75/1/1.25/2.25; Uts3=-/6/9/12/15/20 |
| 3 | `A029` | Carrion Beetles | `ACs7` | 6 | -/-/-/-/-/30 | 90/90/90/90/-/125 |  |  | 50/50/50/50/50/50 | 50/50/50/50/50/50 | Osf1=u001/u006/u007/u008/u000/u000; Osf2=3/4/5/6/7/9 |
| 4 | `AUls` | Locust Swarm | `AUls` | 3 |  | -/200/225 |  | -/825/850 | 25/25/25 | 25/25/25 | Uls1=32/35/40; Uls2=0.01/0.01/0.01; Uls3=22/25/35; Uls4=4/4/4; Uls5=-/25/30; Ulsu=-/u00C/u00A |

## 13. Keeper of the Groove (`E002`)

Base: `Ekee` | Category: 3 | STR: 17 + 1.8 | AGI: 18 + 1.8 | INT: 21 + 2.3 | Move: 320 | Turn: 0.7

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AEer` | Entangling Roots | `AEer` | 6 | -/8.5/9/9.5/9.5/9.5 |  | 600/625/650/700/750 |  | 7/10/12/15/20/25 | 2/3/4/5/6 | Eer1=35/45/60/75/90/105 |
| 2 | `AEah` | Thorns Aura | `AEah` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Eah1=0.18/0.25/0.55/0.65000004/0.75000006/0.9 |
| 3 | `A007` | Force Of Nature | `ACs7` | 6 | 20/20/20/20/20/25 | 50/75/80/90/-/150 |  |  |  |  | Osf1=e00R/e00M/e00N/e00C/e006/e00S; Osf2=3/4/5/6/7/9 |
| 4 | `AEtq` | Tranquility | `AEtq` | 3 | -/60/60 | 90/80/70 |  | 1000/1100/1200 |  |  | Etq1=3.75/7/9.5; Etq2=0.1/0.1/0.1; Etq3=-/1/1 |

## 14. Priestess of the Moon (`E003`)

Base: `Emoo` | Category: 2 | STR: 17 + 1.8 | AGI: 20 + 2.4 | INT: 17 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AEst` | Scout | `AEst` | 6 |  | -/100/100/100/125/125 |  |  | 30/30/30/30/30/30 | 30/30/30/30/30/30 | Hwe1=n03F/n00W/n03G/n00X/n03H/n00B; Hwe2=2/2/3/3/3/3 |
| 2 | `A0DY` | Stun Arrow | `ANfb` | 6 | -/-/10/10/10/10 |  |  |  | 5/6/7/8/9/10 | 1.5/1.75/2/2.25/2.5/3 | Htb1=150/200/250/300/375/450 |
| 3 | `AEar` | Trueshot Aura | `AEar` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Ear1=0.25/0.3/0.35/0.45/0.55/0.6 |
| 4 | `AEsf` | Starfall | `AEsf` | 3 | 220/210/200 | -/225/250 |  | 1200/1250/1300 | 15/17/20 | 15/17/20 | Esf1=175/275/350; Esf2=1.25/1.25/1.25; Esf3=0.1/0.1/0.1 |

## 15. Demon Hunter (`E00F`)

Base: `Edem` | Category: 6 | STR: 19 + 1.9 | AGI: 21 + 2.5 | INT: 18 + 1.7 | Move: 320 | Turn: 0.7

Spell source: inherited from base Edem

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AEmb` | Mana Burn | `AEmb` | 6 | 10/10/6/6/6/6 | 60/60/60/60/60/60 | 450/500/550/625/650/650 |  |  |  | Emb1=175/250/350/500/600/750; Emb2=0.15/0.15/0.15/0.15/0.15/0.15 |
| 2 | `AEim` | Immolation | `AEim` | 6 |  | 50/50/-/20/15/10 |  | 210/215/225/240/260/290 | 0.1/0.1/0.1/0.1/0.1/0.1 | 0.1/0.1/0.1/0.1/0.1/0.1 | Eim1=2.5/4/7.5/10/15/17; Eim2=8/10/11/12/13/15 |
| 3 | `AEev` | Evasion | `AEev` | 6 |  |  |  |  |  |  | Eev1=0.2/0.3/0.4/0.5/0.6/0.7 |
| 4 | `AEme` | Metamorphosis | `AEme` | 3 |  | 100/100/100 |  |  | 0/0/0 | -/70/80 | Eme5=-/750/1200; Emeu=E009/E000/E001 |

## 16. Warden (`E005`)

Base: `Ewar` | Category: 9 | STR: 18 + 1.8 | AGI: 18 + 2.2 | INT: 17 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AEfk` | Fan of Knives | `AEfk` | 6 | 8/-/10/10/11/11 |  |  | 500/500/500/500/550/600 |  |  | Efk1=110/150/220/275/330/570; Efk2=1000/1600/2500/3200/4600/7200 |
| 2 | `AEbl` | Blink | `AEbl` | 6 | -/9/8/7/6/5 | -/40/40/40/30/25 |  |  |  |  | Ebl1=900/1000/1100/1200/1300/1400; Ebl2=-/150/100/50/25/0 |
| 3 | `AEsh` | Shadow Strike | `AEsh` | 6 |  |  | 400/450/475/500/525/550 |  | 15/16/17/18/19/20 | 7/7/7/7/7/7 | Esh1=20/60/90/120/150/180; Esh2=0.75/0.75/0.75/0.75/0.75/0.75; Esh5=-/-/-/300/375/550 |
| 4 | `AEsv` | Vengeance | `AEsv` | 3 | -/170/160 | -/140/130 |  |  | -/180/180 | -/180/180 | Esv1=-/2/3 |

## 17. Brewmaster (`N00I`)

Base: `Npbm` | Category: 10 | STR: 22 + 2.5 | AGI: 18 + 1.7 | INT: 18 + 1.8 | Move: 320 | Turn: 0.6

Spell source: inherited from base Npbm

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANbf` | Breath of Fire | `ANbf` | 6 | -/-/12/12/12/12 | 110/110/110/110/110/200 | -/525/550/575/600/650 | -/-/-/-/150/150 |  |  | Nbf5=10/20/22/25/30/35; Ucs1=85/135/190/250/325/570; Ucs2=1200/1800/2750/3800/5000/8000; Ucs3=400/600/650/700/800/850 |
| 2 | `ANdh` | Drunken Haze | `ANdh` | 6 | 10/10/10/10/10/10 | 90/90/90/90/100/100 | -/600/650/700/750/800 | 250/350/400/450/500/550 | 5/5/5/5/5/5 |  | Nsi2=0.1/0.2/0.35/0.45/0.55/0.65; Nsi3=0.1/0.2/0.3/0.4/-/0.6 |
| 3 | `ANdb` | Drunken Brawler | `ANdb` | 6 |  |  | 1500/1500/1500/1500/1500/1500 |  |  |  | Ocr1=-/12/14/16/18/20; Ocr2=-/-/-/-/6/7; Ocr3=5/7.5/10/12.5/15/20; Ocr4=0.2/0.25/0.3/0.4/0.5/0.6; Ocr5=1/1/1/1/1/1 |
| 4 | `ANef` | Storm, Earth, And Fire | `ANef` | 3 | 260/260/260 | 200/200/200 |  |  | 50/50/50 | 50/50/50 | Nef1=n00L,n00L,n00L/n00J,n00J,n00J/n00Y,n00Y,n00Y |

## 18. Falcon (`E00E`)

Base: `Ewar` | Category: 8 | STR: 18 + 2.2 | AGI: 18 + 1.9 | INT: 17 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANsg` | Summon Bear | `ANsg` | 6 | 30/45/45/45/45/45 | -/-/-/-/150/200 |  |  |  |  | Hwe1=n02Q/n02Q/n02R/n02S/n02S/n00E; Hwe2=-/2/2/2/3/2 |
| 2 | `AOhx` | Hex | `AOhx` | 6 | 8/8/8/7.5/7.5/7.5 | -/-/-/60/50/40 | 700/750/-/850/900/1000 |  | 4/5/6/7/8/9 | 2/2.5/3/3.5/4/4.5 | Ply2=npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat |
| 3 | `AHab` | Brilliance Aura | `AHab` | 6 |  |  |  | 1100/1350/1400/1500/1700/1800 |  |  | Hab1=2.5/3.5/6/8/12/15 |
| 4 | `ANtm` | Transmute | `ANtm` | 3 | 145/130/120 | 175/-/225 | 450/450/450 |  |  |  | Ntm1=1.2/1.25/1.3; Ntm3=6/7/8 |

## 19. Dino Din (`O00D`)

Base: `Othr` | Category: 1 | STR: 18 + 2 | AGI: 20 + 2 | INT: 18 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A005` | Unholy Aura | `AUau` | 6 |  |  |  | -/950/1000/1050/1100/1250 | 2/2/2/2/2/2 | 2/2/2/2/2/2 | Uau1=0.15/-/0.25/-/0.5/0.6; Uau2=2.8/4.2/6/8/10/15 |
| 2 | `A0IB` | Summon Hawk | `ANsw` | 6 | 40/40/40/40/40/40 | 100/100/100/100/100/100 |  |  | 50/50/50/50/50/50 | 50/50/50/50/50/50 | Hwe1=-/n04W/-/n04T/n04U/n04V; Hwe2=2/2/2/2/2/2 |
| 3 | `ANfb` | Firebolt | `ANfb` | 6 | 9/9.5/10/10.5/11/11 | -/-/-/-/-/85 | 750/775/775/-/825/850 |  | 4/5/6/7/8/15 | -/2.25/2.5/2.75/3/3.5 | Htb1=-/-/-/250/300/550 |
| 4 | `A00Y` | Summon Druids | `ACs7` | 3 | 180/170/160 | 150/140/130 |  |  | 20/30/40 | 20/30/40 | Osf1=e00B/e00B/e00B; Osf2=5/6/7 |

## 20. Heavenly Death Healer (`E00T`)

Base: `Efur` | Category: 4 | STR: 17 + 1.8 | AGI: 16 + 1.8 | INT: 18 + 2.2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AHhb` | Holy Light | `AHhb` | 6 | -/6/7/7/6 | 80/80/80/80/80/40 | 850/875/900/925/950/1000 |  |  |  | Hhb1=300/600/900/1200/1500/2000 |
| 2 | `A03Q` | Death Coil | `AUdc` | 6 | -/-/-/8/8/9 | -/-/-/125/150/175 | -/825/900/950/1000/1050 |  |  |  | Udc1=-/500/800/1045/1500/2070 |
| 3 | `A08B` | Healing Wave | `AOhw` | 6 | 8.5/8.5/9.5/9.5/9.5/9.5 | 100/100/110/110/115/125 | -/725/750/775/800/825 | -/525/550/575/600/625 |  |  | Ocl1=140/220/-/385/470/750; Ocl2=4/-/-/6/7/12; Ocl3=0.2/0.2/0.2/0.2/0.15/0.05 |
| 4 | `A06C` | AoE Invisibility | `ACsi` | 3 | 220/200/160 | 150/150/150 | 800/800/800 | 350/400/500 | 0.01/0.01/0.01 | 0.01/0.01/0.01 | Nsi1=-/8/8 |

## 21. Alchemist (`N01C`)

Base: `Nalc` | Category: 8 | STR: 18 + 2.2 | AGI: 17 + 1.8 | INT: 17 + 2 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANhs` | Healing Spray | `ANhs` | 6 | 1/1/1/1/1/1 | 85/85/95/100/110/115 | -/850/900/900/900/900 | -/300/325/350/375/400 |  |  | Ncs1=-/-/65/75/85/100; Ncs3=7/9/11/13/15/20; Ncs4=-/-/550/800/1000/1400; Nhs6=4/-/-/5/5/5 |
| 2 | `ANcr` | Chemical Rage | `ANcr` | 6 | -/28/26/24/22/20 | 0/0/0/0/0/0 |  |  | 0/0/0/0/0/0 | 20/20/20/20/20/20 | Eme1=N01C/N01C/N01C/N01C/N01C/N01C; Emeu=-/-/-/N01G/N01H/N01I; Ncr5=1/1/1/1/1/1; Ncr6=0.5/-/1/-/1.5/1.75 |
| 3 | `ANab` | Acid Bomb | `ANab` | 6 | 10/10/-/-/13/13 |  | -/-/775/800/850/900 | -/225/250/275/300/350 | 10/10/10/10/10/10 | 10/10/10/10/10/10 | Nab1=-/-/-0.05/-0.1/-0.15/-0.2; Nab2=-/-/-0.1/-0.2/-0.3/-0.4; Nab3=-/6/8/10/12/15; Nab4=11/16/24/34/46/60; Nab5=8/12/17/22/27/47 |
| 4 | `ANtm` | Transmute | `ANtm` | 3 | 145/130/120 | 175/-/225 | 450/450/450 |  |  |  | Ntm1=1.2/1.25/1.3; Ntm3=6/7/8 |

## 22. Pit Lord (`N00P`)

Base: `Nplh` | Category: 11 | STR: 22 + 2.6 | AGI: 20 + 1.8 | INT: 18 + 1.8 | Move: 320 | Turn: 0.6

Spell source: inherited from base Nplh

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANrf` | Rain of Fire | `ANrf` | 6 | 9/9/9/9/11/1 | 100/100/100/100/100/100 | -/-/900/900/925/950 | -/250/275/300/350/400 |  |  | acas=0.9/0.9/0.8/0.8/0.8/0.8; Hbz1=5/6/6/7/8/9; Hbz2=-/38/70/90/115/135; Hbz3=5/-/9/-/12/14; Hbz4=0.1/0.1/0.1/0.1/0.1/0.1; Hbz5=6/8/9/10/11/12; Hbz6=9999/9999/9999/9999/99999/9999 |
| 2 | `ANht` | Howl of Terror | `ANht` | 6 | 10/10/11/11/11/11 | 70/80/90/100/110/125 |  | 600/700/900/900/950/1000 | 7/7/7/7/7/7 | 7/7/7/7/7/7 | Roa1=0.15/0.2/0.3/0.4/-/0.5; Roa2=2/3/4/6/8/10 |
| 3 | `ANca` | Cleaving Attack | `ANca` | 6 |  |  |  | 275/300/300/300/300/325 |  |  | nca1=-/0.4/0.5/0.6/0.70000005 |
| 4 | `ANdo` | Doom | `ANdo` | 3 | 180/170/160 | -/150/150 | 700/750/800 |  |  |  | Ndo1=75/75/75; Ndo2=2/2/2; Ndo3=70/70/70; Ndo4=-/5/5; Ndou=-/test/n04P |

## 23. The Craziest Cat (`E00U`)

Base: `Emfr` | Category: 11 | STR: 17 + 1.8 | AGI: 17 + 1.8 | INT: 16 + 2 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A019` | Moon Walk | `AOwk` | 6 | 8/9/10/10/10/10 | 90/90/90/90/90/90 |  |  | 16/20/22/25/27/30 | 16/20/23/25/27/30 | Owk2=0.2/0.25/0.3/0.4/0.45/0.6; Owk3=75/140/200/250/350/500 |
| 2 | `AHtc` | Thunder Clap | `AHtc` | 6 | 8/8/9/9/10/10 | 75/80/85/100/150/150 |  | 300/375/400/450/475/500 | 3.5/4/4/4/4.5 |  | Htc1=75/-/180/250/300/500; Htc3=0.2/0.25/0.3/0.35/0.4/0.45; Htc4=0.15/0.25/0.3/0.35/0.4 |
| 3 | `ANca` | Cleaving Attack | `ANca` | 6 |  |  |  | 275/300/300/300/300/325 |  |  | nca1=-/0.4/0.5/0.6/0.70000005 |
| 4 | `A0TN` | Reincarnation | `AOre` | 3 | 360/320/280 |  |  |  |  |  | acas=2/2/1; Ore1=6/4/2 |

## 24. Edge (`E007`)

Base: `Ekgg` | Category: 9 | STR: 17 + 1.6 | AGI: 16 + 2 | INT: 18 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A01V` | Electric Backstab | `ANfl` | 6 | 10/10/12/12/12/12 | 125/125/125/135/145/155 | -/625/775/800/800/825 | 140/150/170/175/180/185 |  |  | Ocl1=90/-/225/290/350/585; Ocl2=6/6/7/8/9/12; Ucs3=-/925/925/950/950/950; Ucs4=-/325/350/375/400/450 |
| 2 | `AHbn` | Banish | `AHbn` | 6 | 4/4/4/4/4/3 | 125/125/125/125/125/125 | -/825/850/875/900/925 |  | -/-/-/48/60/120 | -/-/-/7/8/9 | Hbn1=0.3/0.4/-/0.6/0.7/0.8; Hbn2=0.2/0.3/0.4/0.5/0.6/0.7 |
| 3 | `A0D5` | Blink | `AEbl` | 6 | -/9/8/7/6/5 | -/45/40/35/30/25 |  |  |  |  | Ebl1=900/1000/1100/1200/1300/1500; Ebl2=-/150/100/50/25/0 |
| 4 | `AHpx` | Phoenix | `AHpx` | 3 | 200/200/200 | -/175/175 |  |  |  |  | Hwe1=-/h00P/h00Q; Hwe2=-/2/3 |

## 25. Monkey King (`O01B`)

Base: `Obla` | Category: 6 | STR: 19 + 2 | AGI: 17 + 2.1 | INT: 16 + 2 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0LO` | Staff of the Jungle | `AUcs` | 6 | 11/12/13/13/13/13 | 100/100/100/100/100/100 | 1100/1100/1100/1100/1100/1100 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=900/900/900/900/900/900; Ucs4=0/0/0/0/0/0 |
| 2 | `A0R7` | Staff of the River | `AOsh` | 6 | 13/13/14/14/14/14 |  | 99999/99999/99999/99999/99999/99999 | -/135/145/155/165/175 |  |  | Osh1=0/0/0/0/0/0; Osh2=0/0/0/0/0/0; Osh3=0/0/0/0/0/0; Osh4=-/135/145/155/165/175 |
| 3 | `A0RG` | Jungle Brawler | `ANdb` | 6 |  |  | 1500/1500/1500/1500/1500/1500 |  |  |  | Ocr1=15/17/18/20/20/20; Ocr2=-/2.25/2.5/3/3.5/4; Ocr3=4/6/8/10/12/15; Ocr4=0.2/0.25/0.3/0.4/0.45/0.5; Ocr5=1/1/1/1/1/1 |
| 4 | `AHCR` | Fury Dive | `ANcl` | 6 | 260/240/220/0.5/0.5/0.5 | 200/225/250 | 700/800/900/1000/1000/1000 | 400/500/600/250/250/250 |  |  | Ncl1=0/0/0/0/0/0; Ncl2=2/2/2/2/2/2; Ncl3=3/3/3/3/3/3; Ncl4=0.01/0.01/0.01/0.01/0.01/0.01; Ncl5=0/0/0/0/0/0; Ncl6=stasistrap/stasistrap/stasistrap/stasistrap/stasistrap/stasistrap |

## 26. Avalanche (`N00O`)

Base: `Nbst` | Category: 11 | STR: 21 + 2.6 | AGI: 18 + 1.8 | INT: 18 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ACRA` | Craggy Exterior | `AHad` | 6 |  |  |  | 0/0/0/0/0/0 |  |  | Had1=4/5/-/8/10/12 |
| 2 | `ATSS` | Toss | `Acri` | 6 | 9/9/12/12/12/12 | 80/80/80/90/100/100 | 750/775/800/825/850/875 |  | 0.001/0.001/0.001/0.001/0.001/0.001 | 0.001/0.001/0.001/0.001/0.001/0.001 | Cri1=0; Cri2=0; Cri3=0 |
| 3 | `A0ST` | Trample | `AOsh` | 6 | 12/12/12/12/12/12 | -/110/115/125/145/150 | 1100/1100/1100/1200/1200/1200 | 0/0/0/0/0/0 |  |  | Osh1=0/0/0/0/0/0; Osh2=0/0/0/0/0/0; Osh3=0/0/0/0/0/0; Osh4=0/0/0/0/0/0 |
| 4 | `AHCR` | Fury Dive | `ANcl` | 6 | 260/240/220/0.5/0.5/0.5 | 200/225/250 | 700/800/900/1000/1000/1000 | 400/500/600/250/250/250 |  |  | Ncl1=0/0/0/0/0/0; Ncl2=2/2/2/2/2/2; Ncl3=3/3/3/3/3/3; Ncl4=0.01/0.01/0.01/0.01/0.01/0.01; Ncl5=0/0/0/0/0/0; Ncl6=stasistrap/stasistrap/stasistrap/stasistrap/stasistrap/stasistrap |

## 27. Jaood (`E00A`)

Base: `Edem` | Category: 5 | STR: 20 + 1.8 | AGI: 21 + 2.4 | INT: 17 + 1.7 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOwk` | Wind Walk | `AOwk` | 6 | 8/8/9/9/9/9 | 90/90/90/90/90/90 |  |  | 18/20/23/25/27/30 | 18/20/23/25/27/30 | Owk2=0.2/0.25/0.3/0.35/0.45/0.55; Owk3=90/150/225/275/375/500 |
| 2 | `A09T` | Shadow Strike | `AEsh` | 6 | 6/6 |  | 500/550/575/600/625/650 |  | 15/16/17/18/19/20 | 7/7/7/7/7/7 | Esh1=20/60/90/120/150/180; Esh2=0.75/0.75/0.75/0.75/0.75/0.75; Esh5=100/225/275/350/375/525 |
| 3 | `A0ID` | Bloodlust | `Ablo` | 6 | 3/3/3/4/4/4 | -/35/35/35/35/35 | -/625/650/675/700/725 |  | 30/35/40/45/50/60 | 30/35/40/45/50/60 | Blo1=0.25/0.5/0.75/1/1.25/1.5; Blo2=-/0.4/0.55/0.70000005/0.85/1; Blo3=0.2/0.25/0.3/0.3/0.35/0.4 |
| 4 | `A0ED` | Powerhit | `ANfb` | 6 | 180/175/170/10.5/11/11 | 150/150/150/-/-/85 | 150/150/150/875/900/925 |  | 2.5/-/3.5/7/8/15 | 1.5/2/2.5/3.5/-/4.5 | Htb1=400/600/900/250/300/550 |

## 28. Fox (`O008`)

Base: `Obla` | Category: 11 | STR: 21 + 2.5 | AGI: 18 + 1.9 | INT: 17 + 1.7 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A003` | War Stomp | `AOws` | 6 | 8/8/9/10/10/11 | 80/80/100/100/125/150 |  | 275/-/325/425/450/500 | 2/2.25/2.5/2.75/2.85/3 | -/2.25/2.5/2.75/2.85/3 | Wrs1=80/100/145/175/225/275 |
| 2 | `AHtc` | Thunder Clap | `AHtc` | 6 | 8/8/9/9/10/10 | 75/80/85/100/150/150 |  | 300/375/400/450/475/500 | 3.5/4/4/4/4.5 |  | Htc1=75/-/180/250/300/500; Htc3=0.2/0.25/0.3/0.35/0.4/0.45; Htc4=0.15/0.25/0.3/0.35/0.4 |
| 3 | `A01H` | Lion Skin | `AUts` | 6 |  |  |  |  |  |  | Uts1=0.5/0.75/1/1.25/1.5/2; Uts3=-/7/11/15/19/25 |
| 4 | `A00K` | Demonic Transformation | `AEme` | 3 | 360/380/420 | 200/225/250 |  |  | 0/0/0 | 25/30/35 | Eme1=O008/O008/O008; Eme5=-/750/1000; Emeu=E00J/E00K/E00L |

## 29. HatoUP (`N015`)

Base: `Nbrn` | Category: 1 | STR: 18 + 1.9 | AGI: 20 + 2.2 | INT: 17 + 1.8 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0DZ` | Frost Arrows | `ANfa` | 6 |  |  |  |  |  |  | Hca1=-/15/25/30/35/60; Hca2=-/0.4/0.5/0.6/-/0.8; Hca3=-/0.4/0.5/0.6/-/0.8 |
| 2 | `A09H` | Flaming Lasso | `Amls` | 6 | 12/12/12/12/12/12 | -/75/75/75/75/75 | -/550/550/550/550/550 |  | 5/6/7/8/9/10 | 2/3/4/5/6/7 | mls1=20/25/30/35/40/45 |
| 3 | `A06H` | Unholy Aura | `AUau` | 6 |  |  |  | -/950/1000/1050/1100/1250 | 2/2/2/2/2/2 | 2/2/2/2/2/2 | Uau1=0.15/-/0.25/-/0.5/0.6; Uau2=2.8/4.2/6/9/10/15 |
| 4 | `ANst` | Stampede | `ANst` | 3 |  | 200/225/250 | 500/500/500 | -/1050/1150 | 15/15/15 | 15/15/15 | Nst1=4/5/6; Nst2=60/60/60; Nst3=65/100/120; Nst4=270/280/290 |

## 30. Shadow (`E00O`)

Base: `Edem` | Category: 5 | STR: 17 + 1.8 | AGI: 19 + 2.5 | INT: 17 + 1.7 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A019` | Moon Walk | `AOwk` | 6 | 8/9/10/10/10/10 | 90/90/90/90/90/90 |  |  | 16/20/22/25/27/30 | 16/20/23/25/27/30 | Owk2=0.2/0.25/0.3/0.4/0.45/0.6; Owk3=75/140/200/250/350/500 |
| 2 | `A04Z` | Firebolt | `ANfb` | 6 | 9/9.5/10/10.5/11/11 | 85/85/100/100/125/150 |  |  | 4/5/6/7/8/15 | -/2.25/2.5/2.75/3/3.25 | Htb1=125/175/225/275/350/550 |
| 3 | `A050` | ICE Blast | `AUfn` | 6 | 9/9/10/10/10/10 | -/-/-/-/150/175 |  |  | 2/3/4/5/-/15 | 1/2/2.5/3/3.5/4 | Ufn1=0/0/0/0/0/0; Ufn2=175/250/300/450/500/800 |
| 4 | `A044` | Torrential Tribute | `ANfd` | 3 | 400/360/330 | 450/450/450 | 450/550/650 |  |  |  | Nfd2=2/2/2; Nfd3=99999/99999/99999 |

## 31. Ravage (`O000`)

Base: `Ofar` | Category: 2 | STR: 16 + 1.8 | AGI: 16 + 2 | INT: 20 + 2 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AEer` | Entangling Roots | `AEer` | 6 | -/8.5/9/9.5/9.5/9.5 |  | 600/625/650/700/750 |  | 7/10/12/15/20/25 | 2/3/4/5/6 | Eer1=35/45/60/75/90/105 |
| 2 | `A0AC` | Trueshot Aura | `AEar` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Ear1=0.25/0.3/0.35/0.45/0.55/0.6 |
| 3 | `A0HF` | Hop | `AEbl` | 6 | -/9/8/7/6/5 | -/45/40/35/30/25 |  |  |  |  | Ebl1=700/800/900/925/950/1000; Ebl2=-/150/100/50/25/0 |
| 4 | `AHre` | Resurrection | `AHre` | 3 | 180/190/200/120 | -/-/-/240 | -/450/500 | 1000/1300/1500 |  |  | Hre1=10/16/24/9 |

## 32. Tinker (`N00Z`)

Base: `Ntin` | Category: 10 | STR: 21 + 2 | AGI: 17 + 1.8 | INT: 19 + 2.2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANs1` | Pocket Factory | `ANs1` | 6 | -/-/-/-/-/30 | -/-/-/135/145/150 | -/515/530/545/560/575 |  | -/41/42/43/44/50 | 42/44/46/48/50/55 | Nsy1=3.5/3.5/3.2/3/2.75/2.5; Nsy2=-/ncgb/ncg1/ncg1/ncg2/ncg2; Nsy3=15/15/15/15/16/17; Nsy5=1300/1300/1300/1350/1350/1400; Nsyu=-/-/-/n013/n016/n016 |
| 2 | `ANcs` | Cluster Rockets | `ANcs` | 6 | 7/8/10/11/11/11 | -/-/100/100/125/125 | -/810/820/830/840/850 | 250/250/250/250/250/250 |  |  | Ncs1=17/27/44/55/72/96; Ncs3=8/10/12/15/18/21; Ncs4=750/1700/2500/4500/7000/9000; Ncs5=0.1/0.1/0.1/0.1/0.1/0.1; Ncs6=1.5/1.5/1.5/1.75/1.75/2 |
| 3 | `ANeg` | Engineering Upgrade | `ANeg` | 6 |  |  |  |  |  |  | Neg1=0.12/0.24/0.36/0.48/0.6/0.9; Neg2=10/15/20/35/45/80; Neg3=ANs1,ANs1/ANs1,ANs1/ANs1,ANs1/ANs1,ANs1/ANs1,ANs1/ANs1,ANs1; Neg4=ANcs,ANcs/ANcs,ANcs/ANcs,ANcs/ANcs,ANcs/ANcs,ANcs/ANcs,ANcs |
| 4 | `AHav` | Avatar | `AHav` | 3 | 240/240/240 | -/175/200 |  |  | 45/47/50 | 45/47/50 | Hav1=-/10/15; Hav2=-/750/1000; Hav3=30/40/50 |

## 33. Shadow Shifter (`N01L`)

Base: `Naka` | Category: 11 | STR: 21 + 2.1 | AGI: 22 + 2.1 | INT: 17 + 1.9 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0M0` | Mana Burn | `AEmb` | 6 | 8/8/8/8/7.5/7 | 75/75/75/75/75/75 | 450/500/525/550/575/600 |  |  |  | Emb1=150/225/300/375/450/550; Emb2=0.1/0.1/0.1/0.1/0.1/0.1 |
| 2 | `A0G9` | Thunder Clap | `AHtc` | 6 | 8/8/8/8/9/9 | 100/100/100/125/150/150 |  | 300/375/400/450/475/500 | 3.5/3.5/4/4/4 |  | Htc1=75/-/180/250/300/500; Htc3=0.2/0.25/0.3/0.35/0.4/0.45; Htc4=0.15/0.25/0.3/0.35/0.4/0.45 |
| 3 | `A092` | Swift Strike | `AOwk` | 6 | 200/200/200/7/7/7 | 150/175/200 |  |  | 25/25/30/-/80/120 | 25/25/30/-/80/120 | Owk2=0.25/0.45/0.65/0.5/0.6; Owk3=750/1250/2000/275/350/600 |
| 4 | `A0LZ` | Blink | `AEbl` | 6 | -/9.5/9/8/7/6 | -/50/50/50/50/50 |  |  |  |  | Ebl1=-/1100/-/1200/1200/1200; Ebl2=-/150/100/50/25/0 |

## 34. Firelord (`N01D`)

Base: `Nfir` | Category: 7 | STR: 19 + 1.9 | AGI: 21 + 2.4 | INT: 19 + 1.7 | Move: 320 | Turn: 0.7

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANso` | Soul Burn | `ANso` | 6 | 10/10/10/9/9/9 |  | 950/975/1000/1050/1075/1100 |  | 9/9/9/9/10/10 | 4/4.5/5/5.5/6/6.5 | Nso1=4.5/5.5/7.5/9/11/13; Nso2=0.1/0.1/0.1/0.1/0.1/0.1; Nso3=-/0.55/0.6/0.65/0.7/0.75 |
| 2 | `ANlm` | Summon Lava Spawn | `ANlm` | 6 | 25/25/25 | 100/100/100/100/100 |  |  | 60/60/60/-/55/55 | 60/60/60/-/55/55 | Hwe1=-/-/-/n01E/n01E/n01F; Hwe2=-/2/2/2/3/2; Nlm2=3/3/2/2/2/2; Nlm3=17/17/17/18/18/18; Nlm6=2 |
| 3 | `ANic` | Incinerate | `ANic` | 6 |  |  |  | 10/10/10/10/10/10 | 4.5/5/5/6/6.5/7 | 4/4.5/4.5/5/5.5/6 | Nic1=1.5/-/6/-/12/20; Nic2=35/55/165/200/250/325; Nic3=100/150/200/250/300/375; Nic4=20/40/75/90/140/175; Nic5=200/230/280/330/380/450 |
| 4 | `ANvc` | Volcano | `ANvc` | 3 | 181 | 150 | 1400/1500/1600 | 400/450 | 1.5/1.5/1.5 | 1.25/1.25/1.25 | Nvc1=7/8/9; Nvc2=11/13/15; Nvc3=1.5/1.5/1.5; Nvc4=0.2/0.2/0.2; Nvc5=275/375/450; Nvc6=0.7/0.7/0.7 |

## 35. Halpmeh (`H00K`)

Base: `Hpal` | Category: 0 | STR: 17 + 1.8 | AGI: 17 + 1.7 | INT: 19 + 2.4 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A01C` | The Shocker | `AOcl` | 6 | 10/10/11/11/12/12 | 100/100/100/100/100/100 | 800/800/800/800/825/850 | 550/575/600/625/650/800 |  |  | Ocl1=115/155/220/255/320/575; Ocl2=5/8/9/11/13/15; Ocl3=0.1/0.1/0.1/0.09/0.07/0.01 |
| 2 | `A0OA` | Time Spiral | `AHbz` | 3 | 220/210/200 | 200/225/250 | 1100/1200/1300 |  |  |  | acas=0/0/0; Hbz1=0/0/0; Hbz2=0/0/0; Hbz3=0/0/0; Hbz4=0/0/0; Hbz6=0/0/0 |
| 3 | `A020` | Zoz | `Alsh` | 6 | 20/12/8.5/6.5/6/6 | -/90/80/70/70/70 | -/625/650/675/700/700 | -/170/180/190/200/225 | 15/15/18/20/20/20 | 10/10/10/10/10/10 | Lsh1=-/30/40/50/60/80 |
| 4 | `A0J1` | Tichmeh Armorz Aura | `AHad` | 6 |  |  | 500/550/600/650/700/750 | 500/550/650/700/750/750 | 1/1.5/1.5/1.75/2/2.5 | 1/1.5/1.5/1.75/-/2.5 | Had1=-2/-3/-5/-6/-7/-9 |

## 36. Angel of Doom (`E00V`)

Base: `Eevi` | Category: 2 | STR: 20 + 2.5 | AGI: 20 + 2.5 | INT: 1 + 1 | Move: 360 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A012` | Tichmeh Armorz Aura | `AHad` | 6 |  |  | 650/700/725/775/800/850 | 575/600/650/700/725/750 | 2/2/2/2/2/2 | 1/1/1/1/1/1 | Had1=-3/-4/-6/-8/-10/-12 |
| 2 | `A03J` | Awe Aura | `ACac` | 6 |  |  |  | 625/650/700/725/750/800 | 1/1/1/1/1/1 | 1/1/1/1/1/1 | Cac1=-0.1/-0.15/-0.2/-0.3/-0.4/-0.45; Ear2=-/1/1/1/1/1; Ear3=-/1/1/1/1/1 |
| 3 | `A03K` | Slow Aura | `AOae` | 6 |  |  |  | 650/675/700/750/775/800 | 1/1/1/1/1/1 | 1/1/1/1/1/1 | Oae1=-0.1/-0.15/-0.2/-0.3/-0.35/-0.4; Oae2=-0.1/-0.15/-0.2/-0.3/-0.35/-0.4 |
| 4 | `A03F` | Terror Wolf | `ANef` | 3 | 220/250/280 | 0/0/0 |  | 200/200/200 | 60/60/60 | 60/60/60 | Nef1=o00F/o00C/o00E |

## 37. Strongest Man Ever (`H00T`)

Base: `Harf` | Category: 1 | STR: 20 + 2.5 | AGI: 20 + 2.5 | INT: 1 + 0 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A013` | Vigor Aura | `AOae` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Oae1=-/-/-/0.4/0.5/0.6; Oae2=-/-/-/0.2/0.25/0.4 |
| 2 | `A00J` | Vampiric Aura | `AUav` | 6 |  |  |  | 800/850/-/1000/1000/1250 |  |  | Uav1=0.18/-/0.4/0.6/0.65/0.75 |
| 3 | `A030` | Call of the Gods | `ANmo` | 3 | 310/310/310 | 115/315/515 | 350/350/350 | 250/-/450 | 5/5/5 | 5/5/5 | Esf1=99999/99999/99999; Esf2=0.5/0.4/0.3; Esf3=0/0/0 |
| 4 | `A02C` | Command Aura | `ACac` | 6 |  |  |  | -/900/1000/1050/1100/1250 |  |  | Cac1=-/0.2/0.3/0.4/0.45/0.5; Ear2=-/1/1/1/1/1; Ear3=-/1/1/1/1/1 |

## 38. Sea Witch (`N00C`)

Base: `Nngs` | Category: 10 | STR: 17 + 1.7 | AGI: 20 + 2 | INT: 21 + 2.4 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANfl` | Forked Lightning | `ANfl` | 6 | 10/10/12/12/12/12 | 100/100/-/130/140/150 | 750/750/800/825/850/850 | 140/150/170/175/180/185 |  |  | Ocl1=110/175/-/300/360/585; Ocl2=7/8/9/10/12/15; Ucs3=-/915/930/945/950/950; Ucs4=-/350/400/450/500/550 |
| 2 | `A01N` | Evasion | `AEev` | 6 |  |  |  |  |  |  | Eev1=0.2/0.3/0.4/0.5/0.6/0.7 |
| 3 | `ANfa` | Frost Arrows | `ANfa` | 6 |  | 5/5/5/5/5/5 | 650/650/650/650/650/650 |  |  | -/-/-/2/2.5/3 | Hca1=20/40/70/90/95/120; Hca2=0.4/-/0.6/-/0.8/0.9; Hca3=0.4/-/0.6/-/0.8/0.9 |
| 4 | `ANto` | Tornado of Fire | `ANto` | 3 | 140/130 | -/200/200 | 900/950/1000 |  | 20/25 | 20/25 | Ntou=-/n00K/n00M |

## 39. Elemental Spirit (`O00I`)

Base: `Otch` | Category: 11 | STR: 21 + 2 | AGI: 20 + 2 | INT: 19 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A04A` | Chaotic Frost | `AOws` | 6 | 10/10/12/12/12/12 | 100/100/115/130/140/150 |  | 300/350/400/450/500/550 | 1.5/2/2.5/2.75/3/3.25 | 1.5/2/2.25/2.5/2.75/3 | Wrs1=80/145/220/280/375/550 |
| 2 | `A0CB` | Static Screen | `AEim` | 6 |  | 50/50/50/50/50/50 |  | 175/185/200/225/235/250 | 0.1/0.1/0.1/0.1/0.1/0.1 | 0.1/0.1/0.1/0.1/0.1/0.1 | Eim1=1.8/3.3/4.5/6.5/8.5/12; Eim2=-/9/10/12/15/20 |
| 3 | `S002` | Cyclone | `SCc1` | 6 | 10/10/10/10/10/10 | 75/75/75/75/75/75 | 800/850/900/950/1000/1000 |  | 3/3.25/3.5/3.75/4/4.25 | 3/3.25/3.5/3.75/4/4.25 | cyc1=-/1/1/1/1/1 |
| 4 | `A0CC` | Hellfire | `ACt2` | 3 | 200/190/180 | 175/200/225 |  | 0 | 0 | 0 | Ctc1=0; Ctc3=0; Ctc4=0 |

## 40. Dark Ranger (`N05H`)

Base: `Nbrn` | Category: 2 | STR: 18 + 1.8 | AGI: 22 + 2.4 | INT: 19 + 1.9 | Move: 330 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANsi` | Silence | `ANsi` | 6 | -/16/17/18/19/20 | 100/100/125/150/150/175 | -/925/950/975/975/1000 | -/250/300/-/400/425 | 10/10/10/10/10/10 | 3/5/6/7/8/9 | Nsi1=-/-/-/-/-/11 |
| 2 | `ANba` | Black Arrow | `ANba` | 6 |  | 7/7/-/5/5/5 | 1300/1300/1300/1300/1300/1300 |  | 4/4/5/5/5/5 |  | Nba1=5/-/-/25/30/50; Nba2=-/-/2/2/2/2; Nba3=15/20/20/20/25/30; Nbau=u00I/u012/u00K/u00G/abom/u00L |
| 3 | `A06H` | Unholy Aura | `AUau` | 6 |  |  |  | -/950/1000/1050/1100/1250 | 2/2/2/2/2/2 | 2/2/2/2/2/2 | Uau1=0.15/-/0.25/-/0.5/0.6; Uau2=2.8/4.2/6/9/10/15 |
| 4 | `ANch` | Charm | `ANch` | 3 | 10/5/3 | 50/40/25 | 500/550/600 |  |  |  | Nch1=-/7/9 |

## 41. Beastmaster (`N00D`)

Base: `Nbst` | Category: 7 | STR: 20 + 2.4 | AGI: 15 + 1.6 | INT: 17 + 1.8 | Move: 320 | Turn: 0.6

Spell source: inherited from base Nbst

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANsg` | Summon Bear | `ANsg` | 6 | 30/45/45/45/45/45 | -/-/-/-/150/200 |  |  |  |  | Hwe1=n02Q/n02Q/n02R/n02S/n02S/n00E; Hwe2=-/2/2/2/3/2 |
| 2 | `ANsq` | Summon Quilbeast | `ANsq` | 6 | 22/26/26/26/26/26 | -/-/-/-/100/125 |  |  | 50/40/45/50/55/55 | 45/30/35/35/40/40 | Hwe1=-/-/n04A/-/nqb4/n00V; Hwe2=-/2/3/3/3/4 |
| 3 | `ANsw` | Summon Hawk | `ANsw` | 6 | 40/40/40/40/40/40 | 100/100/100/100/100/100 |  |  | 50/50/50/50/50/50 | 50/50/50/50/50/50 | Hwe1=-/-/-/n00F/n00G/n01K; Hwe2=2/2/2/2/2/2 |
| 4 | `ANst` | Stampede | `ANst` | 3 |  | 200/225/250 | 500/500/500 | -/1050/1150 | 15/15/15 | 15/15/15 | Nst1=4/5/6; Nst2=60/60/60; Nst3=65/100/120; Nst4=270/280/290 |

## 42. Cattle Bruiser (`N00N`)

Base: `Nbst` | Category: 5 | STR: 18 + 2.3 | AGI: 16 + 2 | INT: 17 + 1.5 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AUdc` | Death Coil | `AUdc` | 6 | -/-/7/8/9/5 | 100/100/100/120/150/175 | -/850/900/950/1000/1050 |  |  |  | Udc1=-/500/800/1045/1500/2070 |
| 2 | `AUfn` | Cold Strike | `AUfn` | 6 | 9/9/9/9/9/9 | 100/100/110/110/110 | -/-/825/850/875/900 | 0/0/0/0/0/0 | -/-/-/10/12/16 | 1.5/2/2.5/3/3.5/4 | Ufn1=0/0/0/0/0/0; Ufn2=175/250/300/400/450/750 |
| 3 | `A00F` | Contract of Death | `AUdp` | 6 |  |  | -/825/850/875/900/950 |  |  |  | Udp2=0.5/1/1.5/2/2.5/4 |
| 4 | `Afod` | Camato Yannon | `Afod` | 3 | 375/350/325 | 200/225/250 | 400/500/600 |  |  |  | Nfd1=-/0.25/0.25; Nfd2=-/1/1; Nfd3=1000/2000/3000 |

## 43. The Summoner (`H02E`)

Base: `Hlgr` | Category: 1 | STR: 18 + 2.3 | AGI: 16 + 1.8 | INT: 16 + 1.7 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AEer` | Entangling Roots | `AEer` | 6 | -/8.5/9/9.5/9.5/9.5 |  | 600/625/650/700/750 |  | 7/10/12/15/20/25 | 2/3/4/5/6 | Eer1=35/45/60/75/90/105 |
| 2 | `A0IJ` | Endurance Aura | `AOae` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Oae1=0.15/-/0.25/-/0.5/0.6; Oae2=0.1/0.15/0.2/0.25/0.3/0.45 |
| 3 | `ANsq` | Summon Quilbeast | `ANsq` | 6 | 22/26/26/26/26/26 | -/-/-/-/100/125 |  |  | 50/40/45/50/55/55 | 45/30/35/35/40/40 | Hwe1=-/-/n04A/-/nqb4/n00V; Hwe2=-/2/3/3/3/4 |
| 4 | `A039` | CHIMERA | `AHpx` | 3 | -/190/200 | -/200/225 |  |  |  |  | Hwe1=h019/h01C/h01A; Hwe2=-/2/3 |

## 44. Ranger (`H02D`)

Base: `Hvwd` | Category: 2 | STR: 17 + 1.9 | AGI: 21 + 2.3 | INT: 16 + 1.6 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0I9` | Summon Hawk | `ANsw` | 6 | 40/40/40/40/40/40 | 100/100/100/100/100/100 |  |  | 50/50/50/50/50/50 | 50/50/50/50/50/50 | Hwe1=-/n04R/-/n04T/n04U/n04V; Hwe2=2/2/2/2/2/2 |
| 2 | `A0DY` | Stun Arrow | `ANfb` | 6 | -/-/10/10/10/10 |  |  |  | 5/6/7/8/9/10 | 1.5/1.75/2/2.25/2.5/3 | Htb1=150/200/250/300/375/450 |
| 3 | `AEar` | Trueshot Aura | `AEar` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Ear1=0.25/0.3/0.35/0.45/0.55/0.6 |
| 4 | `A03V` | Multiple Arrows | `Aroc` | 3 | 1.8/1.8/1.8/1 |  | 650/650/650/600 | 600/600/600/600 | -/1/1/1 | -/1/1/1 | Efk1=60/100/140/2000; Efk2=99999/99999/99999/99999; Efk3=4/5/6/8 |

## 45. Warrior Mage (`H01B`)

Base: `Hpb2` | Category: 10 | STR: 18 + 2.2 | AGI: 17 + 1.6 | INT: 18 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A051` | Firestorm | `ANcs` | 6 | 10/11/12/12/12/12 | 100/100/100/110/125/150 | -/815/830/845/850/850 | 250/260/270/275/275/275 |  |  | Ncs1=12/26/48/60/73/90; Ncs2=0.27/0.27/0.27/0.27/0.27/0.27; Ncs3=7/10/14/18/20/22; Ncs4=120/420/660/840/1400/1800; Ncs5=0.1/0.1/0.1/0.1/0.1/0.1; Ncs6=-/1.25/1.5/1.75/2/2 |
| 2 | `A04Y` | Sea of Flames | `ANst` | 3 | -/200/200 | -/-/250 | 500/500/500 | 950/1100/1150 | 20 | 20 | Nst1=4/5/6; Nst2=60/60/60; Nst3=70/100/135; Nst4=-/285/295 |
| 3 | `A042` | Dark Flame | `Ainf` | 6 | 10/10/10/10/10/10 | 125/125/125/125/125/125 | -/500/500/500/500/500 |  | -/60/60/60/60/60 | -/60/60/60/60/60 | Inf1=-/0.18/0.25/0.32/0.4/0.65; Inf2=3/5/7/9/11/15; Inf3=-/500/500/500/500/500 |
| 4 | `ANab` | Acid Bomb | `ANab` | 6 | 10/10/-/-/13/13 |  | -/-/775/800/850/900 | -/225/250/275/300/350 | 10/10/10/10/10/10 | 10/10/10/10/10/10 | Nab1=-/-/-0.05/-0.1/-0.15/-0.2; Nab2=-/-/-0.1/-0.2/-0.3/-0.4; Nab3=-/6/8/10/12/15; Nab4=11/16/24/34/46/60; Nab5=8/12/17/22/27/47 |

## 46. Jaina (`H01I`)

Base: `Hjai` | Category: 10 | STR: 17 + 1.7 | AGI: 17 + 1.7 | INT: 19 + 2.3 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A046` | Breath of Frost | `ANbf` | 6 | -/-/12/12/12/12 | 100/110/125/145/150/175 | 500/525/550/575/600/650 | -/-/-/-/150/150 | -/-/-/-/6/6 | -/-/-/-/6/6 | Nbf5=0/0/0/0/0/0; Ucs1=80/-/190/250/325/570; Ucs2=1000/1800/2500/3300/4100/7200; Ucs3=500/550/600/650/750/800; Ucs4=150 |
| 2 | `AEbl` | Blink | `AEbl` | 6 | -/9/8/7/6/5 | -/40/40/40/30/25 |  |  |  |  | Ebl1=900/1000/1100/1200/1300/1400; Ebl2=-/150/100/50/25/0 |
| 3 | `AHab` | Brilliance Aura | `AHab` | 6 |  |  |  | 1100/1350/1400/1500/1700/1800 |  |  | Hab1=2.5/3.5/6/8/12/15 |
| 4 | `A048` | Triple Orb Explosion | `AHbz` | 6 | 180/180/180/9/10/11 | 150/150/150/150/150/150 | -/825/850/875/900/950 | 250/300/350/350/400/450 |  |  | Hbz1=1/1/1/8/9/11; Hbz2=1/1/1/100/130/180; Hbz3=1/1/1/11/13/16; Hbz4=0.1/0.1/0.1/0.1/0.1/0.1; Hbz6=0/0/0/99999/99999/99999 |

## 47. Hydralisk (`H01H`)

Base: `Hvwd` | Category: 9 | STR: 17 + 1.9 | AGI: 18 + 2.4 | INT: 17 + 1.7 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AUim` | Impale | `AUim` | 6 | 10/10/12/12/12/12 | -/-/110/120/130/150 | 500/525/575/600/625 | 275/275/300/-/325/350 | 0.75/1/1.5/2.25/2.5/3 | -/1.5/2/2.25/2.5 | Uim1=500/525/575/-/625/700; Uim3=65/115/185/225/320/540 |
| 2 | `A01Y` | Summon Zerglings | `ANsq` | 6 | -/-/-/-/-/30 | -/-/100/100/125/150 |  |  | -/-/50/65/60/60 | 60/61/60/60/55/65 | Hwe1=zzrg/zzrg/z001/z001/z002/z000; Hwe2=2/3/3/3/3/3 |
| 3 | `A01I` | Poison Arrows | `AEpa` | 6 |  | 5/4/4/3/3/2 | -/700/700/700/700/700 |  | -/12/14/16/18/20 | -/11/12/13/14/15 | Poa1=-/15/25/-/40/50; Poa2=6/8/10/12/14/20; Poa4=0.05/0.1/0.15/0.2/0.25/0.3 |
| 4 | `A0A8` | Avatar | `AHav` | 3 | 215/215/215 | -/175/200 |  | 0.8/0.8/0.8 | 45/45/45 | 45/45/45 | Hav1=-/10/15; Hav2=-/750/1000; Hav3=30/40/50 |

## 48. Mad Wizard (`N027`)

Base: `Nngs` | Category: 4 | STR: 18 + 1.8 | AGI: 15 + 1.5 | INT: 22 + 2.5 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOhw` | Healing Wave | `AOhw` | 6 | 8.5/8.5/9.5/9.5/9.5/9.5 | 100/100/110/110/115/125 | -/725/750/775/800/825 | -/525/550/575/600/625 |  |  | Ocl1=140/220/-/385/470/750; Ocl2=4/-/-/6/7/12; Ocl3=0.2/0.2/0.2/0.2/0.15/0.05 |
| 2 | `A04S` | Mass Hex | `AOeq` | 6 | 14/13/12/10/10/10 | 50/50/50/50/50/50 | 800/850/900/925/950 | 150/175/200/225/250/275 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Oeq1=100/100/100/100/100/100; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=150/175/200/225/250/275 |
| 3 | `A0SA` | Forked Bolt | `ANfl` | 6 | 10/10/12/12/12/12 | 100/100/100/100/100/100 | 750/750/800/800/800/800 | 140/150/160/165/170/175 |  |  | Ocl1=80/135/180/225/325/500; Ocl2=4/4/4/5/5/6; Ucs3=850/875/-/-/925/950; Ucs4=275/-/325/375/400/450 |
| 4 | `A08Z` | Not Guilty | `ACsi` | 3 | 140/130/120 | 150/150/150 | 750/775/800 | 350/450/500 | 0.01/0.01/0.01 | 0.01/0.01/0.01 | Nsi1=-/8/8 |

## 49. Myrmidon Warrior (`N02O`)

Base: `Nplh` | Category: 9 | STR: 19 + 2.5 | AGI: 16 + 1.7 | INT: 17 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A05X` | Water Blast | `AUcs` | 6 | -/-/11/12/12/12 | -/-/-/115/125/150 | -/725/750/775/800/800 |  |  |  | Ucs1=-/-/-/275/330/600; Ucs2=900/1500/2400/3200/4000/7200; Ucs3=-/-/-/825/850/900; Ucs4=315/330/345/360/375/390 |
| 2 | `AHwe` | Summon Water Elemental | `AHwe` | 6 | 25/25/25/25/25 | 150/150/150/175/175/200 |  |  | 35/35/35/35/35/35 | 35/35/35/35/35/35 | Hwe1=hwt2/-/-/h007/h007/h006; Hwe2=-/2/2/2/3/3 |
| 3 | `A05W` | Cleaving Attack | `ANca` | 6 |  |  |  | -/225/-/275/300/350 |  |  | nca1=-/0.6/0.9/1.2/1.5/1.9 |
| 4 | `A05Y` | Sinkhole | `ANto` | 3 | 240/230/220 | 250/250/250 |  |  | -/20/20 | -/20/20 | Ntou=h02A/h02A/h02A |

## 50. Thrall (`H02C`)

Base: `Hmkg` | Category: 3 | STR: 18 + 2.5 | AGI: 17 + 1.5 | INT: 19 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0VM` | Command Aura | `ACac` | 6 |  |  |  | -/1000/1050/1100/1100/1250 |  |  | Cac1=0.08/0.16/0.25/0.3/0.35/0.4; Ear2=-/1/1/1/1/1; Ear3=-/1/1/1/1/1 |
| 2 | `AHtb` | Storm Bolt | `AHtb` | 6 | 10/10/10/10/10/10 | 100/125/125/125/125/150 | -/650/675/700/725/750 |  | 1.5/2/2.5/3/3.5/4 | 1.5/2/2.5/-/3.25/3.5 | Htb1=125/-/-/450/550/800 |
| 3 | `AOsf` | Feral Spirit | `AOsf` | 6 | 20/20 | -/-/-/75/75/75 |  |  |  |  | Osf1=osw2/osw3/o015/o007/o014/o001; Osf2=-/3/4/5/6/7 |
| 4 | `A062` | Stasis Trap | `Asta` | 3 | 35/35/30/10/12/15 | 80/80/80/125/150/150 | 700/800/800/650/700/850 |  | 45/50/55/120/180/320 | 2/2.5/3.5/120/180/320 | Sta1=1.25/1/0.75/3.5/3/0.5; Sta2=350/400/450/300/350/400; Sta3=500/550/600/450/500/600; Sta4=2/3/4/10/12/15; Sta5=0.8/0.7/0.6; Stau=-/otot/otot/otot/otot/otot |

## 51. Blooddancer (`O00G`)

Base: `Obla` | Category: 0 | STR: 21 + 2.2 | AGI: 19 + 2 | INT: 17 + 1.9 | Move: 320 | Turn: 0.6

Spell source: object uhab + trigger-detected Blooddancer handlers

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0C6` | Pulverize | `ACpv` | 6 |  |  |  |  |  |  | War1=25/25/25/25/25/25; War2=100/125/150/200/275/300; War3=250/275/300/350/350/350; War4=300/325/400/400/450/500 |
| 2 | `A0K6` | Blood Mark | `AEsh` | 6 |  |  |  |  | 7/7/7/7/7/7 | 7/7/7/7/7/7 | Esh1=0/0/0/0/0/0; Esh4=0/0/0/0/0/0; Esh5=100 |
| 3 | `A0EA` | Ancient Ritual | `Arpl` | 6 | 4/4/4/4/4/4 | 50/50/50/50/50/50 | -/250/250/250/250/250 | 500/550/600/625/650/700 | -/1/1/1/1/1 | -/1/1/1/1/1 | Rej1=100/150/200/300/350/400; Rpb5=1/1/1/1/1/1; Rpb6=8/10/12/14/16/20 |
| 4 | `A0EK` | Rejuvenation | `Arej` | 6 | 9/9/9/9/9/9 | 75/75/80/85/90/90 | 500/550/600/700/750/800 |  | 10/9/8/7/6/6 | 10/9/8/7/6/6 | Rej1=250/350/450/550/650/750; Rej2=100/150/175/200/225/250; Rej3=-/3/3/3/3/3 |
| 5 | `A07X` | Bloodrush | `AEim` | 6 |  | 40/50/60/80/90/120 |  | 220/230/240/260/280/300 | 0.1/0.1/0.1/0.1/0.1/0.1 | 0.1/0.1/0.1/0.1/0.1/0.1 | Eim1=2.5/3/6/8/10/13; Eim2=0/0/0/0/0/0; Eim3=0/0/0/0/0/0 |
| 6 | `A078` | Soul Link | `ACs7` | 3 | 220/210/200 | 300/350/400 |  |  | 90/90/90 | 90/90/90 | Osf1=n02T/n02T/n02T; Osf2=1/1/1 |
| 7 | `A07D` | Sacrifice | `AHtc` | 1 | 0 | 50/125/125/125/125/150 |  | 0/-/-/-/450/500 | 0/-/6/7/8/9 | 0 | Htc1=0/-/-/200/250/350; Htc3=0/0.55/0.6/0.65/0.7/0.8; Htc4=0/0.55/0.6/0.65/0.7/0.75 |

## 52. Pander Prospero (`H02J`)

Base: `Hblm` | Category: 8 | STR: 17 + 1.7 | AGI: 17 + 2.1 | INT: 17 + 1.9 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A00X` | Bloodlust | `Ablo` | 6 | 2.5/2.5/2.5/3/4/4 | -/35/35/35/35/35 | -/625/650/675/700/725 |  | 30/35/40/45/50/60 | 30/35/40/45/50/60 | Blo1=0.25/0.5/0.75/1/1.25/1.5; Blo2=-/0.4/0.55/0.70000005/0.85/1; Blo3=0.2/0.25/0.3/0.3/0.35/0.4 |
| 2 | `A09B` | Preserve | `ANpr` | 6 | 11/10.5/10/9.5/9/8.5 | 85/85/85/85/85/85 | 750/850/950/1000/1100/1200 |  |  |  | Npr1=-/15/15/15/15/15 |
| 3 | `A07P` | Devour Magic | `Advm` | 6 | 10/10/10/10/10/10 | 85/85/95/95/95/95 | 700/700/750/775/775/775 | 150/150/200/200/300/400 |  |  | dvm1=35/55/65/75/85/95; dvm2=25/40/50/60/70/90; dvm5=300/500/700/900/1000/1200; dvm6=-/1/1/1/1/1 |
| 4 | `ANtm` | Transmute | `ANtm` | 3 | 145/130/120 | 175/-/225 | 450/450/450 |  |  |  | Ntm1=1.2/1.25/1.3; Ntm3=6/7/8 |

## 53. Expohate (`H01U`)

Base: `Hjai` | Category: 7 | STR: 20 + 2.3 | AGI: 16 + 1.7 | INT: 18 + 2 | Move: 330 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A07Q` | No Pool/No Share | `ACt2` | 3 | 160/140/120 | 180/160/140 |  | 50/50/50 | 0 | 5/5/5 | Ctc1=150/150/150; Ctc3=0.5/0.4/0.3; Ctc4=0.2/0.2/0.2 |
| 2 | `A07I` | Leave if you don’t get Myrmidon | `AUsl` | 6 | 10/10/10/10/10/10 | 75/-/75/75/75/75 | 700/725/750/775/-/850 |  | -/-/-/80/100/110 | -/5/7/8/9/10 | Usl1=0.5/0.5/0.5/0.5/0.5/0.5 |
| 3 | `A07R` | Slow Aura | `AOae` | 6 |  |  |  | 650/675/725/750/775/800 | 1/1/1/1/1/1 | 1/1/1/1/1/1 | Oae1=-0.1/-0.15/-0.2/-0.3/-0.35/-0.4; Oae2=-0.1/-0.15/-0.2/-0.25/-0.3/-0.4 |
| 4 | `A079` | Oh no, I forgot to mass | `AOsh` | 6 | 10/11/13/13/13/13 | -/-/-/110/115/120 | -/725/750/775/800/850 | -/135/145/155/165/175 |  |  | Osh1=95/140/185/250/325/515; Osh2=-/-/-/3240/4080/7200; Osh3=750/-/850/875/950/975; Osh4=-/135/145/155/165/175 |

## 54. W-a-r-l-o-c-k (`H02R`)

Base: `Hblm` | Category: 10 | STR: 18 + 1.9 | AGI: 17 + 1.9 | INT: 20 + 2.2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A07Z` | Forked Lightning | `ANfl` | 6 | -/-/-/-/12/12 | 120/120/125/125/125/125 | 700/700/750/750/800/850 | 135/145/155/165/175/185 |  |  | Ocl1=100/170/180/220/320/550; Ocl2=5/6/7/8/9/13; Ucs3=800/815/830/845/860/875; Ucs4=-/350/400/450/500/550 |
| 2 | `A08G` | Charge | `AOsh` | 6 | 11/12/14/14/14/14 | 110/110/125/125/125/150 | 1100/1100/1100/1200/1200/1200 | 0/0/0/0/0/0 |  |  | Osh1=0/0/0/0/0/0; Osh2=0/0/0/0/0/0; Osh3=0/0/0/0/0/0; Osh4=0/0/0/0/0/0 |
| 3 | `AHab` | Brilliance Aura | `AHab` | 6 |  |  |  | 1100/1350/1400/1500/1700/1800 |  |  | Hab1=2.5/3.5/6/8/12/15 |
| 4 | `A080` | Vengeance of the Gods | `ACt2` | 3 | 220/220/220 | 150/175/200 |  | 0 | 0 | 0 | Ctc1=0; Ctc3=0; Ctc4=0 |

## 55. Pirate (`H02G`)

Base: `Hblm` | Category: 8 | STR: 17 + 1.7 | AGI: 17 + 2 | INT: 17 + 1.8 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `S003` | Cripple | `Scri` | 6 | 15/15/15/15/15/15 | 50/50/50/50/50/50 | 900/900/900/900/900/900 |  | -/25/30/35/40/45 | 3/4/5/6/7/10 | Cri1=0.4/0.5/0.6/0.70000005/0.8000001/0.9000001; Cri2=0.3/0.4/0.5/0.6/0.70000005/0.8000001; Cri3=0.4/0.5/0.6/0.70000005/0.8000001/0.9000001 |
| 2 | `A06T` | Send Pirate Ship | `AOeq` | 6 | 10/10/10/10/10/10 | 100/100/100/100/100/100 | 875/875/875/875/875/875 | 175/200/225/250/275/300 | 0/0/0/0/0/0 | 0/0/0/0/0/0 | Oeq1=0.001/0.001/0.001/0.001/0.001/0.001; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=175/200/225/250/275/300 |
| 3 | `A0C2` | Blink | `AEbl` | 6 | -/9.5/9/8.5/8/7 | 75/75/75/75/75/75 |  |  |  |  | Ebl1=-/1000/1000/1100/-/1200; Ebl2=150/150/150/150/150/150 |
| 4 | `ANtm` | Transmute | `ANtm` | 3 | 145/130/120 | 175/-/225 | 450/450/450 |  |  |  | Ntm1=1.2/1.25/1.3; Ntm3=6/7/8 |

## 56. Tactician (`H02Z`)

Base: `Hpal` | Category: 4 | STR: 17 + 1.8 | AGI: 16 + 1.7 | INT: 20 + 2.2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOhw` | Healing Wave | `AOhw` | 6 | 8.5/8.5/9.5/9.5/9.5/9.5 | 100/100/110/110/115/125 | -/725/750/775/800/825 | -/525/550/575/600/625 |  |  | Ocl1=140/220/-/385/470/750; Ocl2=4/-/-/6/7/12; Ocl3=0.2/0.2/0.2/0.2/0.15/0.05 |
| 2 | `A0V1` | Command Aura | `ACac` | 6 |  |  |  | -/1000/1000/1050/1100/1250 |  |  | Cac1=0.08/0.16/0.25/0.3/0.35/0.4; Ear2=-/1/1/1/1/1; Ear3=-/1/1/1/1/1 |
| 3 | `A08K` | Reinforcements | `ANsw` | 6 | 25/25/25/25/25/25 | 90/90/95/100/110/125 |  |  | 35/45/50/50/50/50 | 35/35/35/35/35/35 | Hwe1=h034/h030/h031/h032/h035/h033; Hwe2=2/2/2/2/2/2 |
| 4 | `A0DK` | Summon Mauroder | `AUin` | 3 |  | 150/150/150 |  | 225/275/300 | 1.5/2/2.5 | 1.5/-/2.5 | Uin1=75/150/200; Uin2=60/80/90; Uin3=0.3/0.3/0.3; Uin4=n042/n043/n044 |

## 57. YETI (`H02I`)

Base: `Hblm` | Category: 11 | STR: 22 + 2.3 | AGI: 19 + 2.2 | INT: 17 + 1.6 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A08S` | Hibernate | `AUsl` | 6 | 15/15/15/15/15/15 | 120/120/120/120/120/120 | 550/600/625/650/650/650 |  | 0/0/0/0/0/0 | -/6/7/8/9/10 | Usl1=0.6/0.8/1/1/1/1 |
| 2 | `A0B5` | Transformation | `AHav` | 3 | 220/220/220 | 250/250/250 |  |  | 50/50/50 | 50/50/50 | Hav1=-/10/15; Hav2=-/750/1000; Hav3=30/40/50 |
| 3 | `A04A` | Chaotic Frost | `AOws` | 6 | 10/10/12/12/12/12 | 100/100/115/130/140/150 |  | 300/350/400/450/500/550 | 1.5/2/2.5/2.75/3/3.25 | 1.5/2/2.25/2.5/2.75/3 | Wrs1=80/145/220/280/375/550 |
| 4 | `A08O` | Cleaving Attack | `ANca` | 6 |  |  |  | 275/300/325/375/400/400 |  |  | nca1=-/0.4/0.5/0.6/0.70000005 |

## 58. Soviet Sniper (`H036`)

Base: `Hpal` | Category: 6 | STR: 16 + 1.7 | AGI: 19 + 2.5 | INT: 16 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AREW` | Reposition | `AOwk` | 6 | 12/12/12/12/12/12 | -/-/-/85/90/100 |  |  | 15/16/18/19/20/23 | 15/16/18/19/20/23 | Owk1=0/0/0/0/0/0; Owk2=-/0.15/0.2/0.25/0.3/0.35; Owk3=75/150/200/250/300/350 |
| 2 | `A0OE` | Searing Shot | `AUcs` | 6 | 9/9/9/9/8/8 | 90/90/90/90/90/90 | 1150/1150/1150/1150/1150/1150 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=0/0/0/0/0/0; Ucs4=0/0/0/0/0/0 |
| 3 | `Afae` | Faerie Fire | `Afae` | 6 | 4/4/3.5/3/2.5/2 | 50/50/40/40/40/40 | -/750/800/825/850/900 |  | 15/18/20/22/25/30 | 10/11/12/13/14/15 | Fae1=3/5/7/10/12/16 |
| 4 | `A08W` | Golovu Shot | `AOsh` | 6 | 220/210/200/9/9/9 | 150/175/200/130/130/200 | 1300/1300/1300/775/800/850 | 0/0/0/25/25/25 |  |  | acas=0.01/0.01/0.01; Osh1=0/0/0/270/340/600; Osh2=0/0/0/3240/4080/7200; Osh3=0/0/0/950/1000/1350; Osh4=0/0/0/25/25/25 |

## 59. Lawyer (`O00O`)

Base: `Oshd` | Category: 7 | STR: 18 + 2 | AGI: 18 + 1.5 | INT: 21 + 2.5 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ANs2` | Student Debt | `ANs2` | 6 | 25/-/-/30/30/30 | 150/150/150/150/175/200 | -/515/530/545/560/575 |  |  |  | Nsy1=4/4/3/2.5/2.5/2; Nsy2=n02W/n02W/n02X/n02X/n02Y/n02Y; Nsy3=-/13/14/15/15/15; Nsy5=1000/-/1300/1400/1460/1500; Nsyu=n038/-/-/n013/n016/n016 |
| 2 | `A0M1` | Objection! | `Auhf` | 6 | 10/10/10/10/10/10 | 90/90/90/90/90/90 | 600/750/800/850/900/950 |  | 1/1/1/1/1/1 | 1/1/1/1/1/1 | Uhf1=0; Uhf2=0 |
| 3 | `A08Y` | Legal Research | `AOsw` | 6 | 9/8/7/6/5/5 | 40/45/50/55/60/65 | -/525/550/575/600/650 |  | 20/22.5/25/26/27/28 | 20/22.5/25/26/27/28 | Hwe1=o00P/o00P/o00Q/o00R/o00S/o00S; Hwe2=2/3/2/3/3/4 |
| 4 | `A08Z` | Not Guilty | `ACsi` | 3 | 140/130/120 | 150/150/150 | 750/775/800 | 350/450/500 | 0.01/0.01/0.01 | 0.01/0.01/0.01 | Nsi1=-/8/8 |

## 60. Reptile (`O00T`)

Base: `Obla` | Category: 11 | STR: 18 + 1.9 | AGI: 20 + 2.5 | INT: 18 + 1.8 | Move: 330 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A09J` | Acid Bomb | `ANab` | 6 | 9/10/-/-/13/13 |  | -/-/750/775/800/850 | -/225/250/275/300/350 | 10/10/10/10/10/10 | 10/10/10/10/10/10 | Nab1=-/-/-0.05/-0.1/-0.15/-0.2; Nab2=-/-/-0.1/-0.2/-0.3/-0.4; Nab3=-/6/9/12/15/20; Nab4=10/16/24/34/46/60; Nab5=7/12/17/22/27/47 |
| 2 | `A09E` | Acid Armor | `Alsh` | 6 | 15/15/15/15/15/15 | -/90/80/70/60/50 | 650/650/700/750/750/750 | -/170/180/190/200/225 | -/20/20/20/20/20 | 15/15/15/15/15/15 | Lsh1=15/25/35/50/65/80 |
| 3 | `A09F` | Decapitate | `Aams` | 6 | 11/11/11/11/11/11 | -/50/50/50/50/50 | 700/700/700/700/700/700 |  | 0.1/0.1/0.1/0.1/0.1/0.1 | 0.1/0.1/0.1/0.1/0.1/0.1 | Ams3=1/1/1/1/1/1 |
| 4 | `A092` | Swift Strike | `AOwk` | 6 | 200/200/200/7/7/7 | 150/175/200 |  |  | 25/25/30/-/80/120 | 25/25/30/-/80/120 | Owk2=0.25/0.45/0.65/0.5/0.6; Owk3=750/1250/2000/275/350/600 |

## 61. Footgirl (`H01F`)

Base: `Hblm` | Category: 9 | STR: 19 + 2.1 | AGI: 18 + 1.8 | INT: 19 + 1.9 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AINC` | Interchange | `Acri` | 6 | 15/15/15/15/15/15 | 110/110/110/110/110/110 | -/680/700/750/800/850 |  | 0.001/0.001/0.001/0.001/0.001/0.001 | 0.001/0.001/0.001/0.001/0.001/0.001 | acas=0.2/0.2/0.2/0.2/0.2/0.2; Cri1=0; Cri2=0; Cri3=0 |
| 2 | `A0B1` | Mass Entangling Roots | `AOeq` | 6 | 10/10/10/10/10/10 | 90/95/105/110/115/125 | 800/800/800/800/800/800 | 175/200/225/225/225/225 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Oeq1=100/100/100/100/100/100; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=175/200/225/250/275/300 |
| 3 | `A09M` | Anti-magic Shell | `Aam2` | 6 | 10/10/10/10/10/10 | 50/50/50/50/50/50 | 800/800/800/800/800/800 |  | 40/40/40/40/40/40 | 40/40/40/40/40/40 | Ams3=350/400/450/500/550/600 |
| 4 | `A04C` | Mystical Bomb | `AOws` | 6 | 160/150/140 | 150/175/200/125/125/125 |  | 0/0/0/325/-/500 | 0.001/0.001/0.001/6/7/8 | 0.001/0.001/0.001 | Wrs1=0/0/0/135/170/300 |

## 62. Ship's Doctor (`E00P`)

Base: `Ewar` | Category: 2 | STR: 19 + 2.1 | AGI: 15 + 1.5 | INT: 19 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0B6` | Healing Ward | `Ahwd` | 6 | 12/12/12/12/12/12 | 75/75/75/75/75/75 | -/500/550/600/650/700 |  | 15/15/15/15/15/15 | 15/15/15/15/15/15 | hwdu=-/ohwd/ohwd/ohwd/ohwd/ohwd |
| 2 | `A0AD` | Ensnare | `Aens` | 6 | 9/9/9/9/9/9 | 75/75/75/75/75/75 | 700/725/750/800/850/900 |  | 4/4.5/5/5.5/6/6.5 | 2.5/3.5/4/4.5/5/5.5 | Ens1=-/0.6/0.6/0.6/0.6/0.6; Ens2=-/200/200/200/200/200; Ens3=-/128/128/128/128/128 |
| 3 | `A0BA` | Endurance Aura | `AOae` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Oae1=0.15/-/0.25/-/0.5/0.6; Oae2=0.1/0.15/0.2/0.25/0.3/0.45 |
| 4 | `A0BI` | Cannon Ball Rain | `AHbz` | 6 | 220/200/180/9/10/11 | 150/150/150/150/150/150 | -/850/900/875/900/950 | 225/275/325/350/400/450 | 3/3/3 | 3/3/3 | acas=0.45/0.45/0.45; Hbz1=3/3/3/8/9/11; Hbz2=145/180/250/100/130/180; Hbz3=3/3/3/11/13/16; Hbz4=0.1/0.1/0.1/0.1/0.1/0.1; Hbz6=99999/99999/99999/99999/99999/99999 |

## 63. Architect (`N035`)

Base: `Nbst` | Category: 7 | STR: 19 + 2 | AGI: 18 + 1.6 | INT: 20 + 2.4 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0AF` | Recruit | `Aprg` | 6 | 10/10/9/8/8/7 | 70/60/50/40/30/20 | 800/800/800/800/800/800 |  | 0 | 0 | Prg1=0; Prg3=0 |
| 2 | `A0IH` | Tower Defense | `AOsw` | 6 | 15/15/12/12/12/12 | 100/100/100/100/100/100 | 100/100/100/100/100/100 |  | 15/15/15/15/15/15 | 15/15/15/15/15/15 | Hwe1=h04H/h04I/h04L/h04M/h04N/h04O |
| 3 | `A0AH` | Summon Clockwerks | `ANsw` | 6 | 12/12/12/12/12/12 |  |  |  | 25/25/25/25/25/25 | 25/25/25/25/25/25 | Hwe1=n037/n037/n036/n03A/n06B/n06C; Hwe2=-/2/2/2/2/2 |
| 4 | `A0HB` | Raise Morale | `AOsh` | 6 | 180/180/180/11.5/12/12.5 | 150/150/150/165/180/195 | 350/350/350/1200/1200/1200 | 0/0/0/0/0/0 |  |  | Osh1=0/0/0/0/0/0; Osh2=0/0/0/0/0/0; Osh3=0/0/0/0/0/0; Osh4=0/0/0/0/0/0 |

## 64. Wheelchair Warlock (`N03B`)

Base: `Nbst` | Category: 5 | STR: 17 + 2 | AGI: 17 + 1.9 | INT: 18 + 2.2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0AL` | Energy Ball | `ANfb` | 6 | 7/7/-/-/7/6 | 85/85/85/85/85/85 | -/825/850/875/900/925 |  | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Htb1=150/240/400/530/750/1000 |
| 2 | `A08G` | Charge | `AOsh` | 6 | 11/12/14/14/14/14 | 110/110/125/125/125/150 | 1100/1100/1100/1200/1200/1200 | 0/0/0/0/0/0 |  |  | Osh1=0/0/0/0/0/0; Osh2=0/0/0/0/0/0; Osh3=0/0/0/0/0/0; Osh4=0/0/0/0/0/0 |
| 3 | `A09F` | Decapitate | `Aams` | 6 | 11/11/11/11/11/11 | -/50/50/50/50/50 | 700/700/700/700/700/700 |  | 0.1/0.1/0.1/0.1/0.1/0.1 | 0.1/0.1/0.1/0.1/0.1/0.1 | Ams3=1/1/1/1/1/1 |
| 4 | `A0AM` | Duplicate | `ACs7` | 3 | 240/220/200 | 200/200/200 |  |  | 20/25/25 | 20/25/25 | Osf1=o00V/o00V/o00V; Osf2=1/1/1 |

## 65. Dragon Tamer (`H03I`)

Base: `Hamg` | Category: 11 | STR: 18 + 2 | AGI: 18 + 2 | INT: 18 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0JK` | Swift Aura | `AUau` | 6 |  |  |  | 1100/1100/1100/1100/1200/1400 | 2/2/2/2/2/2 | 2/2/2/2/2/2 | Uau1=0.15/0.25/0.4/0.6/0.7/1; Uau2=0/0/0/0/0/0 |
| 2 | `A0AQ` | Dragon Flare | `AOsh` | 6 | 10/10/10/10/10/10 | -/-/-/-/110/125 | 825/825/850/850/875/900 | -/135/145/160/170/175 |  |  | Osh1=80/-/210/285/375/570; Osh2=-/-/-/3800/5000/7800; Osh3=-/825/850/900/950/975; Osh4=-/135/145/160/170/175 |
| 3 | `A0AY` | Summon Deathwing | `ACs7` | 3 | 180/165/155 | 200/225/250 | 0/0/0 |  | 100/100/100 | 100/100/100 | Osf1=e00W/e00X/e00Q; Osf2=1/1/1 |
| 4 | `A09D` | Summon Elemental Dragons | `ANht` | 6 | 35/35/35/35/35/35 | 100/100/100/125/125/150 |  | 0/0/0/0/0/0 | 35/35/35/35/40/40 | 35/35/35/35/40/40 | Roa1=0/0/0/0/0/0 |

## 66. The Nook (`N03J`)

Base: `Npbm` | Category: 0 | STR: 20 + 2.4 | AGI: 17 + 1.6 | INT: 17 + 1.7 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AHES` | Hellseeker | `ANcl` | 6 | 11/11/12/12/12/12 | 75/75/75/75/75/75 | 700/725/725/750/775/800 |  |  |  | Ncl1=0/0/0/0/0/0; Ncl2=1/1/1/1/1/1; Ncl3=1/1/1/1/1/1; Ncl4=0.01/0.01/0.01/0.01/0.01/0.01; Ncl5=0/0/0/0/0/0; Ncl6=creepthunderbolt/creepthunderbolt/creepthunderbolt/creepthunderbolt/creepthunderbolt/creepthunderbolt |
| 2 | `AREA` | Reap | `AOw2` | 6 | 9/9/11/11/12/12 |  |  | 0/0/0/0/0/0 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Wrs1=0/0/0/0/0/0 |
| 3 | `AUav` | Vampiric Aura | `AUav` | 6 |  |  |  | 800/850/-/950/1000/1250 |  |  | Uav1=0.17/0.35/-/0.6/0.65000004/0.75 |
| 4 | `A0JQ` | Hellstorm | `AOww` | 3 | -/180/180 | 175/175/175 |  | 250/275/300 | -/7/7 | 7/7/7 | Oww1=125/250/300 |

## 67. Wooj (`H03N`)

Base: `Hmkg` | Category: 5 | STR: 19 + 1.8 | AGI: 17 + 1.7 | INT: 18 + 2.3 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0QF` | Shock | `Auhf` | 6 | 9/9/9/9/9/8 | 100/100/100/100/110/125 | 700/800/825/850/875/900 |  | 1/1/1/1/1/1 | 1/1/1/1/1/1 | Uhf1=0; Uhf2=0 |
| 2 | `A0JO` | Dash | `AOsh` | 6 | 15/10/-/7/6/5 | -/-/80/70/60/50 | 1500/1500/1500/1500/1500/1500 | 0/0/0/0/0/0 |  |  | Osh1=0/0/0/0/0/0; Osh2=0/0/0/0/0/0; Osh3=0/0/0/0/0/0; Osh4=0/0/0/0/0/0 |
| 3 | `ANSP` | Hammer | `AUcs` | 6 | 8/8/8/8/8/7 | 80/80/80/85/90/90 | 1200/1200/1200/1200/1200/1200 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=0/0/0/0/0/0; Ucs4=0/0/0/0/0/0 |
| 4 | `AHAM` | God Hammer | `AOw2` | 3 | 180/170/160/10/10/10 | 0/0/0 |  | 0/0/0/0/0/0 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Wrs1=0/0/0/0/0/0 |

## 68. Dark Necromancer (`H01R`)

Base: `Hblm` | Category: 7 | STR: 18 + 2.1 | AGI: 17 + 1.7 | INT: 19 + 2.1 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0C5` | Summon Skeleton Archers | `ANsq` | 6 | 30/30/30/30/30/30 | 100/100/100/100/125/155 |  |  | 65/40/45/45/45/45 | 60/40/45/45/45/45 | Hwe1=n03U/n03T/n03V/n03Y/n03S/n03R; Hwe2=-/2/2/2/2/2 |
| 2 | `A0I7` | Corrupted Attack | `Ansk` | 6 |  |  |  |  |  |  | Ssk1=0; Ssk2=0; Ssk3=0; Ssk4=0; Ssk5=0 |
| 3 | `A0II` | Summon Frost Wyrm | `ACs7` | 3 | 200/200/200 | 175/200/250 | 0/0/0 |  | 50/70/80 | 45 | Osf1=u00Q/u00P/u00O; Osf2=1/1/1 |
| 4 | `A0C4` | Summon Skeleton Guards | `ANsg` | 6 | 31/31/30/30/30/30 | 110/110/120/130/140/150 |  |  | 60/60/60/60/60/60 | 60/60/60/60/60/60 | Hwe1=n03P/n03W/n03O/n03Q/n03X/n03N; Hwe2=2/2/2/2/2/2 |

## 69. Man's Best Friend (`H03Q`)

Base: `Hamg` | Category: 2 | STR: 14 + 2 | AGI: 13 + 2 | INT: 1 + 1 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0CA` | Holy Dog Aura | `AHad` | 6 |  |  |  | -/1000/1100/1100/1150/1200 |  |  | Had1=2.5/3.5/5.5/7.5/9/11 |
| 2 | `A0HR` | Unholy Dog Aura | `AUau` | 6 |  |  |  | -/950/1000/1000/1100/1250 |  |  | Uau1=-/-/0.2/-/0.35/0.5; Uau2=2.2/3.3/4/5.5/8/10 |
| 3 | `A0HS` | Trueshot Dog Aura | `AEar` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Ear1=0.15/-/0.25/-/0.4/0.5 |
| 4 | `ANst` | Stampede | `ANst` | 3 |  | 200/225/250 | 500/500/500 | -/1050/1150 | 15/15/15 | 15/15/15 | Nst1=4/5/6; Nst2=60/60/60; Nst3=65/100/120; Nst4=270/280/290 |

## 70. Santa (`U00F`)

Base: `Ulic` | Category: 0 | STR: 18 + 2 | AGI: 17 + 1.5 | INT: 19 + 2.4 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0CK` | Throw Snowball | `AOeq` | 6 | 6/6/6/6/6/6 | 120/120/120/120/120/120 |  | 175/200/225/250/275/300 | 0/0/0/0/0/0 | 0/0/0/0/0/0 | Oeq1=0.001/0.001/0.001/0.001/0.001/0.001; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=175/200/225/250/275/300 |
| 2 | `AUfu` | Frost Armor | `AUfu` | 6 | 3/-/-/-/1.75/1 | -/-/-/-/50/60 | -/825/850/875/900/925 |  | 3/-/-/-/10/12 | 4/4/-/6/10/12 | Ufa1=40/40/-/-/-/60; Ufa2=2/3/6/9/12/15 |
| 3 | `A0CL` | Toy Factory | `ANs1` | 6 | -/-/-/-/40/40 | 150/150/150/150/150/150 | -/515/530/545/560/575 |  | -/41/42/43/44/50 | 42/44/46/48/50/55 | Nsy1=3.75/3.5/3.5/3/3/2.5; Nsy2=-/ncgb/ncg1/ncg2; Nsy3=15/16/17/18/19/20; Nsy5=1500/1500/1500/1500/1500/1500; Nsyu=-/-/-/n013/n016/n016 |
| 4 | `A00V` | Santa-pede | `ANst` | 3 |  | -/-/250 | -/300/300 | -/1050/1100 | 25/25/25 | 25/25/25 | Nst1=3/4/6; Nst2=58/60/60; Nst3=70/95/120; Nst4=-/285/295 |

## 71. Shere-Khan (`O00X`)

Base: `Otch` | Category: 11 | STR: 19 + 1.8 | AGI: 20 + 2.4 | INT: 17 + 1.7 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A019` | Moon Walk | `AOwk` | 6 | 8/9/10/10/10/10 | 90/90/90/90/90/90 |  |  | 16/20/22/25/27/30 | 16/20/23/25/27/30 | Owk2=0.2/0.25/0.3/0.4/0.45/0.6; Owk3=75/140/200/250/350/500 |
| 2 | `A0CT` | Tiger Strike | `ANdb` | 6 |  |  | 1500/1500/1500/1500/1500/1500 |  |  |  | Ocr1=-/12/14/16/18/20; Ocr2=-/-/-/5.5/6/6.5; Ocr3=5/7.5/10/12.5/15/20; Ocr4=0.1/0.2/0.3/0.4/0.5/0.6; Ocr5=1/1/1/1/1/1 |
| 3 | `A0DG` | Tiger Swipe | `AEfk` | 6 | 10/10/11/11/11/11 |  |  | 300/300/350/350/375/400 |  |  | Efk1=95/155/230/300/375/575; Efk2=1000/1600/2500/3200/4400/7500 |
| 4 | `A062` | Stasis Trap | `Asta` | 3 | 35/35/30/10/12/15 | 80/80/80/125/150/150 | 700/800/800/650/700/850 |  | 45/50/55/120/180/320 | 2/2.5/3.5/120/180/320 | Sta1=1.25/1/0.75/3.5/3/0.5; Sta2=350/400/450/300/350/400; Sta3=500/550/600/450/500/600; Sta4=2/3/4/10/12/15; Sta5=0.8/0.7/0.6; Stau=-/otot/otot/otot/otot/otot |

## 72. Guardian (`H043`)

Base: `Hblm` | Category: 0 | STR: 18 + 2 | AGI: 18 + 1.5 | INT: 21 + 2.5 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0DL` | Purification Nova | `ANrf` | 6 | 11/11/12/12/13/13 | 100/100/110/125/135/135 | 900/900/900/900/900/900 | 250/275/300/325/335/350 | 0/0/0/0/0/0 | 0/0/0/0/0/0 | acas=0/0/0/0/0/0; Hbz1=1/1/1/1/1/1; Hbz2=1/1/1/1/1/1; Hbz3=1/1/1/1/1/1; Hbz4=0/0/0/0/0/0; Hbz5=0/0/0/0/0/0; Hbz6=0/0/0 |
| 2 | `AFOR` | Force Field | `AUcs` | 6 | 1/1/1/1/1/1 | 90/90/90/90/90/90 | -/750/775/800/825/850 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=0/0/0/0/0/0; Ucs4=0/0/0/0/0/0 |
| 3 | `AENC` | Conduit Aura | `AHab` | 6 |  |  |  | 800/800/800/800/800/800 |  |  | Hab1=1.2/2.5/3/4/5/6 |
| 4 | `A0GG` | Time Spiral | `AEbl` | 6 | 9/9.5/9/8.5/8/7 | 75/75/75/75/75/75 | 1100/1200/1400 | 225/250/300 | 0/0/0 |  | Ebl1=0/0/0/0/0/1200; Ebl2=400/400/400/150/150/150 |

## 73. Archangel (`H03U`)

Base: `Hpal` | Category: 10 | STR: 19 + 2.2 | AGI: 17 + 2 | INT: 17 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0EE` | Divine Smite | `AOsh` | 6 | 10/10/12/12/12/12 | 110/110/115/125/125/135 | -/750/800/850/875/900 | -/135/145/155/165/175 |  |  | Osh1=90/-/190/250/340/570; Osh2=950/1600/2650/3800/4700/7700; Osh3=750/-/850/875/900/950; Osh4=135/145/155/165/175/185 |
| 2 | `A0EI` | Holy Light | `AHhb` | 6 | -/6/7/7/7 | 80/80/80/80/80/40 | 850/875/900/900/925/950 |  |  |  | Hhb1=300/500/800/1000/1200/1600 |
| 3 | `A0NM` | Reincarnation | `AOre` | 3 | 400/380/300 |  |  |  |  |  | acas=2/2/1; Ore1=6/-/4 |
| 4 | `AUav` | Vampiric Aura | `AUav` | 6 |  |  |  | 800/850/-/950/1000/1250 |  |  | Uav1=0.17/0.35/-/0.6/0.65000004/0.75 |

## 74. Thor (`H048`)

Base: `Hmkg` | Category: 5 | STR: 21 + 2.5 | AGI: 17 + 1.6 | INT: 18 + 1.6 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0EC` | Throw Mjölnir  | `AUcs` | 6 |  | 90/90/90/90/90/90 | 800/800/850/950/1000/1100 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=900/900/900/900/900/900; Ucs4=0/0/0/0/0/0 |
| 2 | `AHtc` | Thunder Clap | `AHtc` | 6 | 8/8/9/9/10/10 | 75/80/85/100/150/150 |  | 300/375/400/450/475/500 | 3.5/4/4/4/4.5 |  | Htc1=75/-/180/250/300/500; Htc3=0.2/0.25/0.3/0.35/0.4/0.45; Htc4=0.15/0.25/0.3/0.35/0.4 |
| 3 | `A00D` | Bash | `AHbh` | 6 |  |  |  |  | -/3/4/4/4/5 | -/1.25/1.5/1.75/2/2 | Hbh1=10/15/20/25/30; Hbh3=-/35/45/55/65/80 |
| 4 | `A0ED` | Powerhit | `ANfb` | 6 | 180/175/170/10.5/11/11 | 150/150/150/-/-/85 | 150/150/150/875/900/925 |  | 2.5/-/3.5/7/8/15 | 1.5/2/2.5/3.5/-/4.5 | Htb1=400/600/900/250/300/550 |

## 75. Dark Sorceress (`O010`)

Base: `Oshd` | Category: 4 | STR: 17 + 1.8 | AGI: 16 + 1.7 | INT: 19 + 2.2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOhw` | Healing Wave | `AOhw` | 6 | 8.5/8.5/9.5/9.5/9.5/9.5 | 100/100/110/110/115/125 | -/725/750/775/800/825 | -/525/550/575/600/625 |  |  | Ocl1=140/220/-/385/470/750; Ocl2=4/-/-/6/7/12; Ocl3=0.2/0.2/0.2/0.2/0.15/0.05 |
| 2 | `ANht` | Howl of Terror | `ANht` | 6 | 10/10/11/11/11/11 | 70/80/90/100/110/125 |  | 600/700/900/900/950/1000 | 7/7/7/7/7/7 | 7/7/7/7/7/7 | Roa1=0.15/0.2/0.3/0.4/-/0.5; Roa2=2/3/4/6/8/10 |
| 3 | `A0FD` | Death Ward | `AOeq` | 6 | 11/11/11/11/11/11 | 75/75/75/75/75/75 | 550/550/600/650/675/700 | 450/475/500/525/550/600 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Oeq1=100/100/100/100/100/100; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=450/475/500/525/550/600 |
| 4 | `AUin` | Inferno | `AUin` | 3 |  | -/185/200 |  | 275/325/375 | 2/2.5/3 | 1.5/-/2.5 | Uin1=75/150/175; Uin2=160/160/160; Uin3=0.3/0.3/0.3; Uin4=-/n009/n008 |

## 76. Drunken Templar (`E00Y`)

Base: `Efur` | Category: 4 | STR: 19 + 1.9 | AGI: 17 + 1.6 | INT: 21 + 2.3 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `AOhw` | Healing Wave | `AOhw` | 6 | 8.5/8.5/9.5/9.5/9.5/9.5 | 100/100/110/110/115/125 | -/725/750/775/800/825 | -/525/550/575/600/625 |  |  | Ocl1=140/220/-/385/470/750; Ocl2=4/-/-/6/7/12; Ocl3=0.2/0.2/0.2/0.2/0.15/0.05 |
| 2 | `A0IW` | Drunken Haze | `ANdh` | 6 | 10/11 | 90/90/90/90/100/100 | 600/750/800/825/850/900 | 250/350/400/450/500/550 | 7/8/9/9/9/9 | -/5.5/6/6.5/7/7.5 | Nsi2=0.1/0.2/0.3/0.4/0.5/0.6; Nsi3=0.1/0.2/0.3/0.4/-/0.6 |
| 3 | `A0F2` | Summon Brewers | `ANsw` | 6 | 30/30/30/30/30/30 | 75/75/75/75/90/100 |  |  | 50/50/50/50/50/50 | 39/50/50/50/50/50 | Hwe1=n049/n049/n049/n049/n049/n049; Hwe2=2/2/2/2/2/2 |
| 4 | `A0GC` | Absinthe | `ACsi` | 3 | 160/155/150 | 100/100/100 | 900/900/900 | 500/550/600 | 0.01/0.01/0.01 | 0.01/0.01/0.01 | Nsi1=0 |

## 77. Akama (`E00Z`)

Base: `Edem` | Category: 5 | STR: 20 + 2.2 | AGI: 20 + 2 | INT: 18 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ASHS` | Shadow Step | `AEbl` | 6 | -/9/8/7/6/5 | 70/80/50/40/30/25 |  |  |  |  | Ebl1=900/1000/1100/1250/1400/1500; Ebl2=100/100/100/100/100/100 |
| 2 | `A0LH` | Shadow Strike | `AEsh` | 6 | 6/7 | 65/65/85/85/90/90 | 500/525/550/575/600/650 |  | 7.5/7.5/7.5/7.5/7.5/7.5 | 7.5/7.5/7.5/7.5/7.5/7.5 | acas=2.5/2.5/2.5/2.5/2.5/2.5; Esh1=20/60/80/100/125/150; Esh2=0.75/0.75/0.75/0.75/0.75/0.75; Esh5=100/225/300/325/375/500 |
| 3 | `ALDR` | Life Drain | `ANdr` | 6 | 9/9 | -/70/65/60/55/50 | 600/650/675/700/725/750 | 650/700/725/775/825/900 | 5/5/5/5/5/5 | 4/4/5/5/5/5 | Ndr1=7/11/16/24/29/36; Ndr2=4/7/9/12/15/18; Ndr3=0.1/0.1/0.1/0.1/0.1/0.1; Ndr4=9/13/15/16/20/25; Ndr5=4/6/8/10/11/12; Ndr6=0.1/0.15/0.2/0.25/0.3/0.5; Ndr7=10/11/12/15/20/25; Ndr9=10/10/10/10/10/10 |
| 4 | `A00Q` | Shadow Clone | `Absk` | 3 | 180/180/180 | 50/50/50 |  |  | 45/50/55 | 45/50/55 | bsk1=0; bsk2=0; bsk3=0 |

## 78. Corruptor (`H015`)

Base: `Hblm` | Category: 8 | STR: 18 + 2 | AGI: 18 + 2 | INT: 18 + 2 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0G8` | Defile | `AOeq` | 6 | 7/6.5/6/6/5.5/5 | 60/60/60/60/60/60 | 900/900/950/950/-/1050 | 140/140/140/140/140/140 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Oeq1=100/100/100/100/100/100; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=140/140/140/140/140/140 |
| 2 | `A0G5` | Corruption | `ACsi` | 6 | 10/10/10/10/10/10 | 85/85/85/85/85/85 | 900/900/900/900/900/900 | 250/265/280/295/310/325 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Nsi1=4/4/4/4/4/4 |
| 3 | `A0G7` | Corrupted Attack | `Ansk` | 6 |  |  |  |  |  |  | Ssk1=0; Ssk2=0; Ssk3=0; Ssk4=0; Ssk5=0 |
| 4 | `ANtm` | Transmute | `ANtm` | 3 | 145/130/120 | 175/-/225 | 450/450/450 |  |  |  | Ntm1=1.2/1.25/1.3; Ntm3=6/7/8 |

## 79. The Guard (`E010`)

Base: `Ewar` | Category: 3 | STR: 20 + 2.4 | AGI: 17 + 1.6 | INT: 19 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0GU` | Justice Hook | `AUcs` | 6 | 8/12/12/12/12/12 | 100/100/100/100/100/100 | -/1150/1200/1250/1300/1400 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=700/1150/1200/1250/1300/1400; Ucs4=0/0/0/0/0/0 |
| 2 | `ANht` | Howl of Terror | `ANht` | 6 | 10/10/11/11/11/11 | 70/80/90/100/110/125 |  | 600/700/900/900/950/1000 | 7/7/7/7/7/7 | 7/7/7/7/7/7 | Roa1=0.15/0.2/0.3/0.4/-/0.5; Roa2=2/3/4/6/8/10 |
| 3 | `A0H8` | Invisibility | `Aivs` | 6 | 20/15/15/15/15/15 | 75/75/75/75/75/75 | -/350/400/450/500/550 |  | 45/55/65/75/85/95 | 15/20/22/24/26/30 |  |
| 4 | `A0GL` | Prison Cell | `AOeq` | 3 | 300/300/300/11/11/11 | 200/200/200/65/75/75 | 700/700/700/850/900/950 | 400 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Oeq1=100/100/100/100/100/100; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=400 |

## 80. Zeus (`H04T`)

Base: `Hblm` | Category: 9 | STR: 17 + 1.8 | AGI: 16 + 1.6 | INT: 19 + 2.4 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0LG` | Brilliance Aura | `AHab` | 6 |  |  |  | 1300/1400/1500/1700/1800/2000 |  |  | Hab1=2.5/3.5/6/8/12/15 |
| 2 | `AOcl` | Chain Lightning | `AOcl` | 6 | -/-/11/12/12/12 | 100/110/115/125/140/150 | -/725/750/775/800/850 | 550/575/600/625/650/800 |  |  | Ocl1=115/155/220/265/330/575; Ocl2=5/8/9/11/13/15; Ocl3=0.1/0.1/0.1/0.09/0.07/0.01 |
| 3 | `A0QD` | Shock | `Auhf` | 6 | 12/12/12/12/12/12 | 90/90/90/90/90/90 | 600/650/800/825/850/850 |  | 1/1/1/1/1/1 | 1/1/1/1/1/1 | Uhf1=0; Uhf2=0 |
| 4 | `A0U6` | Monsoon | `AHbz` | 6 | 220/220/220/9/10/10 | 200/225/250/125/125/125 | 850/900/950/950/950/950 | 250/275/300/300/350/400 | 3/3/3 | 3/3/3 | acas=0.3/0.3/0.3/0.7/0.7/0.7; Hbz1=12/14/16/6/6/6; Hbz2=42/62/75/100/120/140; Hbz3=1/1/1/-/12/14; Hbz4=0.1/0.1/0.1/0.1/0.1/0.1; Hbz5=10/15/20; Hbz6=99999/99999/99999/99999/99999/99999 |

## 81. Reaper (`U00R`)

Base: `Ulic` | Category: 10 | STR: 17 + 2 | AGI: 17 + 1.8 | INT: 19 + 2.2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0KN` | Scythe | `AUcs` | 6 | -/-/12/12/12/12 | 125/125/125/130/140/150 | -/725/750/750/800/800 |  |  |  | Ucs1=80/120/195/260/330/570; Ucs2=850/1400/2300/3100/3900/7200; Ucs3=-/-/-/-/-/850; Ucs4=315/330/345/360/375/390 |
| 2 | `A0CI` | Dark Ritual | `AUdr` | 6 | 2/1.9/1.8/1.7/1.6/1.25 | 10/15/20/-/35/50 | -/825/850/875/900/925 |  |  |  | Udp1=0.1/0.15/0.2/0.25/0.3/0.35; Udp2=-/-/-/0.1/0.15/0.2 |
| 3 | `A0KT` | Murder | `AUls` | 3 | 260/260/260 | -/175/200 |  | 850/875/900 | 15/17/20 | 15/17/20 | Uls1=25/32/38; Uls2=0.01/0.01/0.01; Uls3=14/16/20; Uls4=2/2.2/2.75; Uls5=30/35/40; Ulsu=u00Z/u00Y/u010 |
| 4 | `A0KM` | Summon Ghouls | `ANsq` | 6 | 40/40/40/40/40/40 | 100/110/120/130/140/150 |  |  | 20/20/20/20/20/20 | 20/20/20/20/20/20 | Hwe1=u00S/u00T/u00U/u00V/u00W/u00X; Hwe2=2/3/4/4/5/5 |

## 82. Troll (`O016`)

Base: `Obla` | Category: 3 | STR: 17 + 2 | AGI: 19 + 2.4 | INT: 17 + 1.6 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0L0` | Thunder Axe | `AUcs` | 6 | 9/9/9/9/9/9 | 90/90/85/80/70/60 | 99999/99999/99999/99999/99999/99999 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=0/0/0/0/0/0; Ucs4=0/0/0/0/0/0 |
| 2 | `A0IA` | Hex | `AOhx` | 6 | 8/8/8/8/8/8 | -/-/-/60/50/40 | 700/750/-/-/850/900 |  | 4/5/6/7/8/9 | 2/2.5/3/3.5/4/4.5 | Ply2=npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat/npig,nsea,ncrb,nhmc,nrat |
| 3 | `A0ID` | Bloodlust | `Ablo` | 6 | 3/3/3/4/4/4 | -/35/35/35/35/35 | -/625/650/675/700/725 |  | 30/35/40/45/50/60 | 30/35/40/45/50/60 | Blo1=0.25/0.5/0.75/1/1.25/1.5; Blo2=-/0.4/0.55/0.70000005/0.85/1; Blo3=0.2/0.25/0.3/0.3/0.35/0.4 |
| 4 | `A0OH` | Portal | `ANs1` | 6 | 200/190/180/-/40/40 | 175/175/175/150/150/150 | 300/300/300/545/560/575 |  | 25/30/35/43/44/50 | 25/30/35/48/50/55 | Nsy1=3/2.75/2.75/2.75/2.5/2; Nsy2=z004/z005/z003/ncg1/ncg2/ncg2; Nsy3=20/23/25/15/15/15; Nsy4=300/300/300; Nsy5=2000/2250/2500/1300/1400/1400; Nsyu=n060/n05Z/n064/n013/n016/n016 |

## 83. Ratling Gunner (`E011`)

Base: `Emoo` | Category: 6 | STR: 17 + 1.8 | AGI: 21 + 2.4 | INT: 18 + 1.8 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0KE` | Spirit Link | `Aspl` | 6 | 12/12/12/12/12/12 | -/75/75/75/75/75 | -/750/750/750/750/750 | -/550/600/650/650/650 | 30/30/30/30/30/30 | 30/30/30/30/30/30 | spl1=-/0.55/0.6/0.6/0.6/0.6; spl2=8/10/12/14/16/20 |
| 2 | `ADEC` | Decoy Bomb | `ANcs` | 6 | 15/15/15/15/15/15 | 80/80/80/90/90/90 | 700/725/750/775/810/900 | 250/250/250/250/250/250 | 0.001/0.001/0.001/0.001/0.001/0.001 | 0.001/0.001/0.001/0.001/0.001/0.001 | Ncs1=0/0/0/0/0/0; Ncs2=0.27/0.27/0.27/0.27/0.27/0.27; Ncs3=1/1/1/1/1/1; Ncs4=0/0/0/0/0/0; Ncs6=0.001/0.001/0.001/0.001/0.001/0.001 |
| 3 | `A0BI` | Cannon Ball Rain | `AHbz` | 6 | 220/200/180/9/10/11 | 150/150/150/150/150/150 | -/850/900/875/900/950 | 225/275/325/350/400/450 | 3/3/3 | 3/3/3 | acas=0.45/0.45/0.45; Hbz1=3/3/3/8/9/11; Hbz2=145/180/250/100/130/180; Hbz3=3/3/3/11/13/16; Hbz4=0.1/0.1/0.1/0.1/0.1/0.1; Hbz6=99999/99999/99999/99999/99999/99999 |
| 4 | `A00X` | Bloodlust | `Ablo` | 6 | 2.5/2.5/2.5/3/4/4 | -/35/35/35/35/35 | -/625/650/675/700/725 |  | 30/35/40/45/50/60 | 30/35/40/45/50/60 | Blo1=0.25/0.5/0.75/1/1.25/1.5; Blo2=-/0.4/0.55/0.70000005/0.85/1; Blo3=0.2/0.25/0.3/0.3/0.35/0.4 |

## 84. Shredder (`E018`)

Base: `Eevi` | Category: 7 | STR: 18 + 2.2 | AGI: 18 + 2 | INT: 18 + 2 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A007` | Force Of Nature | `ACs7` | 6 | 20/20/20/20/20/25 | 50/75/80/90/-/150 |  |  |  |  | Osf1=e00R/e00M/e00N/e00C/e006/e00S; Osf2=3/4/5/6/7/9 |
| 2 | `A0M9` | Boomerang Blade | `AUcs` | 6 | 12/12/12/12/12/12 | 90/90/90/90/90/90 | 800/800/850/950/1000/1100 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=900/900/900/900/900/900; Ucs4=0/0/0/0/0/0 |
| 3 | `A0AH` | Summon Clockwerks | `ANsw` | 6 | 12/12/12/12/12/12 |  |  |  | 25/25/25/25/25/25 | 25/25/25/25/25/25 | Hwe1=n037/n037/n036/n03A/n06B/n06C; Hwe2=-/2/2/2/2/2 |
| 4 | `A062` | Stasis Trap | `Asta` | 3 | 35/35/30/10/12/15 | 80/80/80/125/150/150 | 700/800/800/650/700/850 |  | 45/50/55/120/180/320 | 2/2.5/3.5/120/180/320 | Sta1=1.25/1/0.75/3.5/3/0.5; Sta2=350/400/450/300/350/400; Sta3=500/550/600/450/500/600; Sta4=2/3/4/10/12/15; Sta5=0.8/0.7/0.6; Stau=-/otot/otot/otot/otot/otot |

## 85. Medivh (`H04U`)

Base: `Hblm` | Category: 4 | STR: 16 + 1.9 | AGI: 17 + 1.5 | INT: 22 + 2.4 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `ATOR` | Gust | `AUcs` | 6 | 13/13/14/14/14/14 | -/-/-/130/140/150 | -/725/750/775/800/800 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=0.01/0.01/0.01/0.01/0.01/0.01; Ucs4=0/0/0/0/0/0 |
| 2 | `void` | Banish | `AHbn` | 6 | 8/7/7/7/7/7 | 100/90/80/70/60/50 | -/825/850/875/900/925 |  | -/-/-/48/60/120 | -/-/-/7/8/9 | Hbn1=0.3/0.4/-/0.6/0.7/0.8; Hbn2=0.2/0.3/0.4/0.5/0.6/0.7 |
| 3 | `A08B` | Healing Wave | `AOhw` | 6 | 8.5/8.5/9.5/9.5/9.5/9.5 | 100/100/110/110/115/125 | -/725/750/775/800/825 | -/525/550/575/600/625 |  |  | Ocl1=140/220/-/385/470/750; Ocl2=4/-/-/6/7/12; Ocl3=0.2/0.2/0.2/0.2/0.15/0.05 |
| 4 | `A0LN` | Tornado of Fire | `ANto` | 3 | 220/200/180 | 150 | 850/900/950 |  | 15/17/20 | 15/17/20 | Ntou=-/n00K/n00M |

## 86. Deadeye (`H04W`)

Base: `Hvwd` | Category: 6 | STR: 17 + 1.9 | AGI: 21 + 2.5 | INT: 17 + 1.5 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0Q4` | Crippling Wave | `AUcs` | 6 | 7/7/7/7/7/6 | 80/80/75/70/65/60 | 1200/1200/1200/1200/1200/1200 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=0/0/0/0/0/0; Ucs4=0/0/0/0/0/0 |
| 2 | `A0LW` | Deadshot | `ANfb` | 6 | 10/10/10/10/10/10 | 100/100/100/100/110/125 |  |  | -/-/3.5/-/5/6 | 1/1.5/1.75/2/2.25/2.5 | Htb1=125/200/275/325/400/500 |
| 3 | `A0I7` | Corrupted Attack | `Ansk` | 6 |  |  |  |  |  |  | Ssk1=0; Ssk2=0; Ssk3=0; Ssk4=0; Ssk5=0 |
| 4 | `A0NB` | Shattering Strike | `AOeq` | 3 | 240/230/210/10/10/10 | 200/225/250/90/100/110 | 850/900/950/950/-/1050 | 300/300/300/175/175/175 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Oeq1=100/100/100/100/100/100; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=300/300/300/175/175/175 |

## 87. Queen (`H04X`)

Base: `Hvwd` | Category: 2 | STR: 18 + 1.8 | AGI: 20 + 2 | INT: 18 + 2 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0LB` | Web | `Aweb` | 6 | 10/10/10/10/10/10 | 75/75/75/75/75/75 | 700/800/800/825/850/900 |  | 3/4/5/6/7/8 | 2.5/3/3.5/4/5/6 | Ens1=5/6/7/8/9/10; Ens2=-/200/200/200/200/200; Ens3=-/128/128/128/128/128 |
| 2 | `A0MO` | Summon Nightmares | `ANsq` | 6 | 22/20/20/20/20/20 | 100/100/100/100/125/125 |  |  | -/40/50/55/55/55 | -/45/50/55/55/55 | Hwe1=n05K/n05S/n05U/n05T/n05V/n05W; Hwe2=-/2/2/2/2/2 |
| 3 | `A005` | Unholy Aura | `AUau` | 6 |  |  |  | -/950/1000/1050/1100/1250 | 2/2/2/2/2/2 | 2/2/2/2/2/2 | Uau1=0.15/-/0.25/-/0.5/0.6; Uau2=2.8/4.2/6/8/10/15 |
| 4 | `A0NF` | Spiderling Swarm | `ACsi` | 3 | 200/190/180/10/10/10 | 150/175/200/85/85/85 | 750/800/850/900/900/900 | 250/300/350/295/310/325 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Nsi1=0/-/-/4/4/4 |

## 88. Witch Doctor (`H04Y`)

Base: `Hblm` | Category: 8 | STR: 18 + 2 | AGI: 16 + 1.4 | INT: 20 + 2.2 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0LI` | Forked Bolt | `ANfl` | 6 | 10/10/12/12/12/12 | 100/100/100/100/105 | 750/750/800/825/850/900 | 140/150/160/165/170/170 |  |  | Ocl1=80/135/180/225/325/500; Ocl2=4/4/4/5/5/6; Ucs3=850/875/-/925/940/950; Ucs4=275/-/325/375/400/450 |
| 2 | `A0M2` | Healing Ward | `Ahwd` | 6 | 11/10/9/8/7/7 | 75/75/75/75/75/75 | -/500/550/600/650/700 |  | 15/17/20/25/30/35 | 15/17/20/25/30/35 | hwdu=-/ohwd/ohwd/ohwd/ohwd/ohwd |
| 3 | `A0KW` | Phase Shift | `Apsh` | 6 | 30/20/15/10/9/8 | 75/65/55/45/35/25 |  |  | 1/1.2/1.4/1.6/1.8/2 | 1/1.2/1.4/1.6/1.8/2 |  |
| 4 | `ANtm` | Transmute | `ANtm` | 3 | 145/130/120 | 175/-/225 | 450/450/450 |  |  |  | Ntm1=1.2/1.25/1.3; Ntm3=6/7/8 |

## 89. Chieftan's Companion (`H04F`)

Base: `Hamg` | Category: 3 | STR: 17 + 2 | AGI: 20 + 2.2 | INT: 16 + 1.6 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A0W2` | Ancestral Spirit | `ANef` | 3 | 290/290/290 | 200/225/250 |  |  | -/50/60 | -/50/60 | Nef1=o01I/o01J/o01K |
| 2 | `A0VA` | Feral Spirit | `AOsf` | 6 | 20/20 | -/-/-/75/75/75 |  |  |  |  | Osf1=osw2/osw3/o015/o007/o014/o001; Osf2=-/3/4/5/6/7 |
| 3 | `A0L3` | Fetch | `AUcs` | 6 | 12/12/12/12/11 |  | 1100/1150/1200/1250/1300/1400 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=1125/1175/1225/1275/1325/1425; Ucs4=0/0/0/0/0/0 |
| 4 | `AUav` | Vampiric Aura | `AUav` | 6 |  |  |  | 800/850/-/950/1000/1250 |  |  | Uav1=0.17/0.35/-/0.6/0.65000004/0.75 |

## 90. Sea Giant (`O01L`)

Base: `Otch` | Category: 6 | STR: 18 + 2.5 | AGI: 17 + 1.8 | INT: 15 + 1.7 | Move: 310 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A05E` | Unholy Frenzy | `Auhf` | 6 | 10/10/9/8.5/8/8 | 75/75/75/75/75/75 | 700/700/725/750/775/800 |  | -/45/45/45/45/45 | -/45/45/45/45/45 | Uhf1=0.35/0.6/1/1.25/1.5/1.75; Uhf2=0.5/0.7/0.9/1.1/1.3/1.5 |
| 2 | `A0NN` | Elemental Wave | `AUcs` | 6 | 12/13/13/13/13/13 | -/115/120/130/140/150 | -/725/750/775/800/800 | 0/0/0/0/0/0 |  |  | Ucs1=0/0/0/0/0/0; Ucs2=0/0/0/0/0/0; Ucs3=0.01/0.01/0.01/0.01/0.01/0.01; Ucs4=0/0/0/0/0/0 |
| 3 | `A006` | Tidal Smash | `AOcl` | 6 | 10/-/8/8/7/7 | 50/35/25/20/15/10 | 250/275/300/325/350/375 | 1/1/1/1/1/1 |  |  | Ocl1=0/0/0/0/0/0; Ocl2=1/1/1/1/1/1; Ocl3=0/0/0/0/0/0 |
| 4 | `A0E0` | Tentacles | `ANsq` | 6 | 260/260/260 | 200/225/250/-/100/125 | 500/500/500 | 250/250/250 | 12/14/16/50/55/55 | 12/14/16/35/40/40 | Hwe1=o018/o018/o018/-/nqb4/n00V; Hwe2=3/3/3/3/3/4 |

## 91. Sylvanus (`N00H`)

Base: `Nbrn` | Category: 2 | STR: 17 + 1.8 | AGI: 20 + 2.2 | INT: 17 + 1.9 | Move: 320 | Turn: 0.6

Spell source: object uhab

| Slot | Rawcode | Name | Base | Levels | CD | Mana | Range | Area | Dur | Hero Dur | Data fields |
|---:|---|---|---|---:|---|---|---|---|---|---|---|
| 1 | `A03B` | Dark Binding | `AOeq` | 6 | 11/11/11/11/11/11 | 75/75/75/75/75/75 | 775/800/825/850/875/900 | 125/125/125/125/125/125 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Oeq1=0/0/0/0/0/0; Oeq2=0/0/0/0/0/0; Oeq3=0/0/0/0/0/0; Oeq4=125/125/125/125/125/125 |
| 2 | `A0G5` | Corruption | `ACsi` | 6 | 10/10/10/10/10/10 | 85/85/85/85/85/85 | 900/900/900/900/900/900 | 250/265/280/295/310/325 | 0.01/0.01/0.01/0.01/0.01/0.01 | 0.01/0.01/0.01/0.01/0.01/0.01 | Nsi1=4/4/4/4/4/4 |
| 3 | `A0AE` | Summon Nether Dragons | `AHpx` | 3 | 200/210/220 | -/200/225 |  |  |  |  | Hwe1=h01T/h01Y/h01Z; Hwe2=-/2/3 |
| 4 | `AEar` | Trueshot Aura | `AEar` | 6 |  |  |  | -/925/950/975/1000/1250 |  |  | Ear1=0.25/0.3/0.35/0.45/0.55/0.6 |

