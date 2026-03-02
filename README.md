# 🏀 Basketball Circle Guess Mini Game

A small Unity mini game where the player guesses how many basketballs are needed to match a randomly generated circular target.

## Gameplay
- A **dashed circle** appears on the ground at the start of each level.
- The player enters a number (**1–100**) using the UI.
- Basketballs spawn in an **even circular pattern** with equal spacing.
- The spawn radius adjusts automatically and balls **drop & bounce using physics**.

After landing:
- If any ball falls **outside** the dashed circle → Level Failed.
- If all balls stay inside → player receives a **score out of 100** based on accuracy.
- Scores below **50** fail the level.
- Passing adds to the total score and allows continuation.
- Failing resets the current score.
- Each restart generates a **new random target circle** within valid limits.

## Extras
- High score saving
- Basketball bounce sound effects
- Cinemachine camera switching

## Built With
Unity • C# • URP • New Input System • Cinemachine

---

**Chaitanya Shinde**