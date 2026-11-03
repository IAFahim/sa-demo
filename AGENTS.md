# SA Shape workshop agent instructions

Read the current local `sa-motion/AGENTS.md`, or [its maintained main copy](https://github.com/IAFahim/sa-motion/blob/main/AGENTS.md). Load the official `unity-cli` skill (`unity skill show` if unavailable in the client) and the [SA companion skill](https://github.com/IAFahim/sa-motion/blob/main/.claude/skills/sa-unity-cli/SKILL.md), maintained in `sa-motion/.claude/skills/sa-unity-cli/`. The companion also has focused rig-coverage and shape-rendering references. Keep custom lessons outside the official plugin cache.

## Before changing the project

- Fetch this repository and both package source repositories, read the requested issue and its latest comments, and inspect the actual branch and package resolution. The packages are pinned Git dependencies; code under `Library/PackageCache` is generated. Change the source repository and update the immutable manifest pin instead of patching the cache. Keep the lockfile and `.meta` assets.
- Use the existing Editor for this project. Check `unity status --format json` and the scene/play/dirty state before changing anything. Pin direct commands with `--project-path <absolute sa-demo root>`. A source worktree is not a reason to launch another Editor.
- From a current `sa-motion` checkout, use `SA_PROJECT=<absolute sa-demo root> eng/unity.sh compile | test <assembly> | eval <file.cs>` for all compile/test/C# execution. `SA_PROJECT` otherwise defaults to ruk2. The wrapper holds the shared lock. Discover other operations with `unity command --caller plugin --skill unity-cli --project-path <absolute sa-demo root> --query <term> --detail compact`.
- Use live Editor APIs to change scenes, prefabs and assets. Do not overwrite dirty scenes or hand-edit their YAML while the Editor is reachable. One connection failure during import is not proof the Editor is closed. Never kill a windowed Editor or capture the desktop.

## The person's walk

1. Open `Assets/Scenes/ShapeWalk.unity` or use `Tools/SA/Shape Workshop/Open Scene`. Play from this host, not `Assets/ShapeWalk/Character.unity` alone. Allow closed SubScene streaming.
2. Defaults show a neutral character in Idle. Verify frames advance. Move Both legs, Left leg and Right arm; switch Idle/Walk/Run; Pause clip must keep shape controls active. Restore neutral must visibly restore the character.
3. Use `Tools/SA/Shape Workshop/Frame Game View` before capture. It sets the actual 1280 x 720 render resolution; a CLI output size can merely stretch an old texture. Inspect the captured Unity PNG and a fresh runtime Console. Keep clip time/camera/light/resolution fixed and exclude the UI for pixel comparisons.
4. Save, reopen from disk and Play again. Check the permanent material, original atlas and shape profile resolve. Scratch cleanup must not delete referenced assets. Leave no probes, telemetry drivers or hidden switches in the scene.

## Preserve the boundaries and claims

- Keep GPU palettes/readbacks `float4x4` with 64-byte stride; CPU/blob affine storage stays 3 x 4. Nonuniform skinning needs the inverse transpose of the blended position matrix for normals and the forward transform for tangents. Vulkan preview/startup paths need valid buffers even when no skin is resident.
- Preserve ordinary Humanoid/Generic import and ECS playback. Humanoid sources use Avatar conversion; Generic clips require compatible transform bindings. Shared-family retargeting, runtime proportions, terrain IK and live physics resizing are separate checks. Do not add Animator Controllers or parameters for scheduling.
- Last measured pinned-package run: Unity 7000.0.0a12, Linux, Intel RPL-S, Vulkan; 95/95 `SA.Tests`, 3/3 `SA.Motion.Unity.Tests`. The earlier OpenGL run had 94 tests before the startup regression was added. Report new results only when actually run, and require a nonzero test count.
- The saved Synty demo/evidence establishes visible joint/skin proportions and neutral restoration. The 50-character/500-animation pilot, wardrobe/morphs, terrain planting, live ragdoll resizing and other platform/player builds remain unverified here. Read `README.md` and `docs/evidence/render-check.json` before broadening a claim.
