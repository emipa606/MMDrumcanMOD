# GitHub Copilot Instructions for MMDrumcanMOD (Continued)

## Mod Overview and Purpose

MMDrumcanMOD is a RimWorld mod that adds drumcan barrels for your colonists to bathe in. These barrels are a historical type of bath in Japan and are still used as an emergency bath for camping and disasters. The mod also includes two new working drumcan barrels that can be found under the Recreation menu. Pawns will receive a positive moodlet for soaking in this historical style of bath.

## Key Features and Systems

- **Drumcan Barrels**: Two new working drumcan barrels for colonists to bathe in, found under the Recreation menu.
- **Fueling**: Drums must be fueled with wood periodically.
- **Positive Moodlet**: Pawns receive a positive moodlet for soaking in the drumcan barrels.
- **Push Drum**: Fun and interactive push drum feature.
- **Prisoner Recreation**: Prisoners also have access to these recreations as they will have additional entertainment gauges.
- **Golden Drums and Drums as Works of Art**: Can be created from the sculpture table.

## Coding Patterns and Conventions

- **C# Code Structure**: The mod follows a clear and organized C# code structure, with separate files for different components and systems.
- **XML Integration**: XML files are used to define various elements of the mod, such as hediff definitions, job definitions, and joy kind definitions.
- **Harmony Patching**: The mod uses Harmony patching to integrate with the RimWorld game code and add new features.
- **Mod Dependencies**: The mod depends on the Harmony mod, which must be installed for the mod to work.

## XML Integration

The mod uses XML files to define various elements of the mod. The XML files are located in the `DefInjected` folder and include:

- `HediffDef/Drumcan_Bath.xml`: Defines the hediff for the drumcan bath.
- `JobDef/Drumcan_Bath.xml`: Defines the job for the drumcan bath.
- `JobDef/Drumcan_Push.xml`: Defines the job for the push drum.
- `JoyKindDef/Drumcan_Bath.xml`: Defines the joy kind for the drumcan bath.

## Harmony Patching

The mod uses Harmony patching to integrate with the RimWorld game code and add new features. The Harmony patches are located in the `DrumBath_Harmony` folder and include:

- `Pawn_GetGizmos.cs`: Postfix patch to add the drumcan bath gizmo to pawns.

## Suggestions for Copilot

- **C# Code Generation**: Use Copilot to generate C# code for new features or components, such as new gizmos or job definitions.
- **XML Generation**: Use Copilot to generate XML files for new hediff definitions, job definitions, or joy kind definitions.
- **Harmony Patch Generation**: Use Copilot to generate Harmony patches for new features or components, such as new gizmos or job definitions.
- **Bug Fixes**: Use Copilot to generate potential fixes for bugs reported by users.

## Contributing to MMDrumcanMOD

If you would like to contribute to MMDrumcanMOD, please follow these steps:

1. Fork the repository on GitHub.
2. Clone the forked repository to your local machine.
3. Create a new branch for your changes.
4. Make your changes to the code.
5. Commit your changes with a descriptive commit message.
6. Push your changes to your forked repository.
7. Create a pull request to the main MMDrumcanMOD repository.

Please also make sure to test your changes thoroughly before submitting a pull request.

## Contact

If you have any questions or issues with the mod, please contact the mod author on the Discord channel or via GitHub issues.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

