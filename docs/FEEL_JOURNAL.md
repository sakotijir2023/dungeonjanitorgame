# Feel Journal

A dated log of meaningful changes to feel values (speeds, gravity, suction, mop radius, slow-motion, hitstop, shake, camera lag, timings) and why they changed.

Format: `YYYY-MM-DD | config.value: old -> new | reason`

2026-11-21 | MovementConfig baseline | walk 4.5, sprint 7.5, accel 60/decel 50, air 15, gravity 25 (fall x1.5), jump 1.2 m, coyote/buffer 0.1 s, mouse 0.12 deg/px, stick 200 deg/s (curve 2), look smoothing 0.02 s, eye 1.6 m, FOV 75 | starting point: snappy but grounded, gravity above real-world for a less floaty jump
2026-11-21 | MovementConfig baseline | no change | owner playtested defaults and approved them as-is
2026-11-21 | VacuumConfig baseline | range 5 m, cone 22 deg, falloff 0.8, pull 55, lateral damping 8, swallow radius 0.6, cleaning speed 1, max hardness 3, spin-up 0.15 s / down 0.25 s, gulp kick 0.06 m | first pass: goo should visibly fight, then snap in
2026-11-21 | GooConfig baseline | mass 0.4, tug stretch 0.15 -> 0.7 as it loosens, wobble 0.08 @ 9 Hz, loosen decay 0.4/s, break-free pop 2.5 m/s, spring 180 / damping 9, swallow 0.1 s | jelly that strains toward the nozzle, then pops
2026-11-21 | GooConfig.drag: 0.6 -> 0.83, GooConfig.tugWobbleAmount: 0.08 -> 0.10 | owner playtest (preset "T"): slightly heavier flight, more visible struggle on stuck goo. Owner confirms: dark (hard) goo should take a LONG time without upgrades; easy goo should stay effortless
2026-11-21 | MopConfig baseline | reach 3.2 m, head radius 0.35 m, head follow 0.07 s, cleaning speed 1, max hardness 3, idle scrub 30%, full scrub at 2.5 m/s head speed, brush falloff 0.6, splash 45/s | scrubbing back and forth should clearly beat holding still (measured: ~2x faster on hardness 1)
2026-11-21 | StainConfig baseline | 64px dirt map, opacity 0.92, clean at <5% left, fade 0.3 s | stains wipe exactly where the head passes; hardness darkens brown grime toward charred black
