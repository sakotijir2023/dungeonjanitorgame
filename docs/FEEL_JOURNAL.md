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
2026-11-22 | SlowMotionConfig baseline | wheel time scale 0.2, ease in 0.08 s / out 0.15 s, physics steps scaled with time (smooth goo) | world nearly stops while choosing, snaps back fast on release
2026-11-22 | ToolWheelConfig baseline | mouse pointer speed 1, max pointer 120, mouse deadzone 30, stick deadzone 0.45, folder rest 0.3 s, ring radius 280, slot 150, hover grow 1.18 @ 18/s, pop-open 0.08 s from 0.85 | quick flicks pick a slot; resting briefly on Setup opens the folder without opening by accident
2026-11-22 | StainConfig looks: grime / blood / scorch | blood glossy (0.75) and splattery (speckle x1.8); scorch matte (0.05) with soft edges (x2.5) | same mop, readable variety on the floor
2026-11-22 | ToolWheelConfig.mousePointerSpeed 1 -> 0.6, new slotStickiness 8 deg, new pointerDotSize 22 | owner overshot slots: slower pointer, borders hold the current slot a little longer, and a visible aiming dot shows where you are
2026-11-22 | Mop reworked: hold = sweep, tap = stroke (mouse-wiggle scrubbing removed at owner request) | sweep: half width 0.7 m, 1.4 swings/s, arc 0.15 m, radius 0.4, power 0.8 | stroke: 0.22 s, push 0.45 m, alternating +/-20 deg, radius 0.3, peak power 3.5, 1 queued tap | headFollowTime 0.07 -> 0.04 so strokes snap | hold covers big light stains, taps punch small tough ones; spam is capped by stroke time
