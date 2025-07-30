# GitHub Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose
This mod introduces new features and systems to RimWorld, enhancing the gameplay experience by adding unique interactions with a special building known as the Drum Bath. The main purpose of the mod is to provide colonists with a new method of recreation, bathing at the Drum Bath, which impacts mood and offers specific joy types.

## Key Features and Systems
- **Drum Bath Building**: A new building allowing colonists to indulge in a joyful bathing experience, enhancing their mood and providing positive effects.
- **Animal Graphic Setting**: Allows for custom graphics for animals within the game, providing a more varied visual experience.
- **Jobs and Joy Givers**: Includes new job definitions and joy givers related to interacting with the Drum Bath.
- **Adjustments and Management**: Component-based architecture for managing animal jobs and visual adjustments related to the Drum Bath.

## Coding Patterns and Conventions
- **Class and Method Naming**: Follows C# conventions with PascalCase for class and method names.
- **DefModExtension Usage**: Utilizes `DefModExtension` for extending built-in classes with additional properties or methods.
- **ThingComp for Component Logic**: Uses `ThingComp` and `CompProperties` to modularize features and allow easy integration with RimWorld's Thing architecture.

## XML Integration
- **Def Files**: Uses XML definitions to declare new Things, Jobs, and Hediffs. These definitions are critical for introducing new objects or actions into the game.
- **DefModExtensions**: Integrates XML with C# for custom behaviors, such as the `AnimalGraphicSetter` which extends functionality to support enhanced graphics for animals.

## Harmony Patching
- Harmony is essential for modifying existing game methods non-destructively.
- Use Harmony patches to inject additional logic or tweak existing behaviors gracefully.
- Ensure patches are targeted and preserve original functionality where possible.

## Suggestions for Copilot
1. **Code Completion and Suggestions**: When creating new classes or methods, suggest boilerplate code consistent with existing class patterns such as constructors for `ThingComp` and overrides for utility classes.
2. **XML Integration**: Assist with crafting XML snippets for new ThingDefs, JobDefs, and HediffDefs based on existing template structures.
3. **Harmony Patches**: Propose patches for commonly overridden RimWorld methods, ensuring conflict-free modding.
4. **Error Handling and Logging**: Suggest try-catch blocks and logging for debugging purposes to track issues in dynamic interactions.
5. **Performance Optimization**: Propose efficient code patterns, especially when dealing with rendering or frequent method calls to ensure high performance.
6. **Modular Code Design**: Encourage the use of components (`ThingComp`) for extensibility and separation of concerns within the mod.

By adhering to these guidelines and leveraging Copilot effectively, developers can streamline their workflow and focus on enriching the gameplay experience of RimWorld through innovative modding efforts.
