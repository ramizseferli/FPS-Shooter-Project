# FPS Shooter Project — Modular Unity Framework

A modular, event-driven First-Person Shooter (FPS) framework built in Unity using C#. Designed with clean code practices, **SOLID principles**, and performance optimization in mind.

---

## Key Technical Features

* **Event-Driven Architecture:** Decoupled systems using C# delegates and events (`OnFired`, weapon state changes) to eliminate hard dependencies between movement, animation, and shooting mechanics.
* **ScriptableObject Data System:** Centralized `WeaponData` system allowing zero-code creation, modification, and balancing of new weapons, damage parameters, and recoil profiles.
* **Procedural Recoil Mechanics:** Dynamic camera and weapon mesh recoil featuring mathematical interpolation (`Vector3.Lerp` & `Slerp`) for realistic snappiness and return speed curves.
* **Raycast & Damage Pipeline:** Precise raycasting mechanics integrated with animation events for accurate weapon damage delivery and impact handling.
* **Modular Weapon Structure:** Weapon hierarchies structured with separated sub-components (Slide, Magazine, Body, Safety, Trigger) for isolated procedural animation control.

---

## Technical Stack

* **Engine:** Unity 6
* **Language:** C#
* **Architecture:** Event-Driven, ScriptableObjects, SOLID Principles
* **Render Pipeline:** Universal Render Pipeline (URP)

---

## Architecture Overview

```text
[ WeaponInputHandler ] ---> [ WeaponController ] ---> (OnFired Event)
                                  |
            +---------------------+---------------------+
            |                     |                     |
   [ Raycast Damage ]    [ ProceduralRecoil ]   [ WeaponAnimationController ]
