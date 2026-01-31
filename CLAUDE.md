# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (6000.1.9f1) C# game project inspired by "Human: Fall Flat", using Universal Render Pipeline (URP) 17.1.0. Currently a fresh project from the URP Empty Template with no custom game code yet.

## Build & Test Commands

This is a Unity project — there are no standalone CLI build/test scripts. All builds and tests run through Unity.

**Build (command line):**
```
Unity.exe -quit -batchmode -projectPath "C:\Users\cre\LikeHumanFallFlat" -buildWindows64Player "Build/game.exe"
```

**Run tests (command line):**
```
Unity.exe -runTests -batchmode -projectPath "C:\Users\cre\LikeHumanFallFlat" -testResults results.xml -testPlatform EditMode
```

**Code compilation only:** Open `LikeHumanFallFlat.sln` in Visual Studio (requires ManagedGame workload).

## Architecture

- **Assets/Scenes/** — Unity scenes. `SampleScene.unity` is the only scene in the build.
- **Assets/Settings/** — URP renderer configs for PC (`PC_Renderer`, `PC_RPAsset`) and Mobile (`Mobile_Renderer`, `Mobile_RPAsset`), plus volume profiles.
- **Assets/InputSystem_Actions.inputactions** — New Input System config with Player actions: Move, Look, Attack, Interact, Crouch.
- **Packages/manifest.json** — Package dependencies. Key packages: Input System 1.14.0, URP 17.1.0, AI Navigation 2.0.8, Timeline 1.8.7, Test Framework 1.5.1.

## Key Details

- Target: .NET Standard 2.1, C# 9.0
- Two assemblies: `Assembly-CSharp` (game) and `Assembly-CSharp-Editor` (editor tools)
- No version control configured yet — no .gitignore present
- `Assets/TutorialInfo/` contains template boilerplate (Readme ScriptableObject) and can be deleted
