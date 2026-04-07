# .NET 10 Upgrade Plan

## Table of Contents
- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Risk Management](#risk-management)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

## Executive Summary
### Scope
This plan covers a coordinated upgrade of the `BinarySerializer` solution to `.NET 10` using the assessment in `.github/upgrades/scenarios/new-dotnet-version_48f017/assessment.md`.

### Current State
- Total projects: `3`
- Total issues: `19` (`16` mandatory, `3` potential)
- Affected files: `4`
- Total lines of code: `22,598`
- Security vulnerabilities reported: `0`
- Recommended package upgrades: `1`
- Behavioral API issues: `2`

Projects in scope:
- `BinarySerializer\BinarySerializer.csproj` — `netstandard1.3;net462` → `netstandard1.3;net462;net10.0`
- `BinarySerializer.Test\BinarySerializer.Test.csproj` — `net7.0` → `net10.0`
- `BinarySerializer.Performance\BinarySerializer.Performance.csproj` — `netcoreapp3.1` → `net10.0`

### Selected Strategy
**All-At-Once Strategy** — all projects upgraded simultaneously in a single coordinated operation.

**Rationale**:
- The solution has `3` projects, which fits the small-solution profile.
- The dependency graph is simple: one leaf library with two dependants and no circular dependencies.
- The assessment reports low difficulty for all projects.
- No security vulnerabilities were reported.
- Package change scope is small: one true version upgrade plus removal of framework-included packages.
- Total code size is moderate and the expected code-change surface reported by assessment is very small.

### Complexity Classification
**Simple**.

Justification:
- `≤5` projects
- dependency depth of `1`
- no circular dependencies
- no high-risk security findings
- only one explicit NuGet version upgrade recommendation

### Planning Approach
This plan uses a fast batch planning model appropriate for a simple solution:
- Phase 0: prerequisite validation if required
- Phase 1: atomic framework and package upgrade across all projects and imported MSBuild settings
- Phase 2: solution-wide test validation

Expected planning detail pattern: concise but complete project specifications, grouped package guidance, and a single atomic execution path with no intermediate project-by-project upgrade states.

### Critical Findings to Carry Into Execution
- `BinarySerializer\BinarySerializer.csproj` must append `net10.0` while preserving existing `netstandard1.3;net462` targets unless the execution stage intentionally narrows support later.
- `BinarySerializer.Test\BinarySerializer.Test.csproj` should upgrade `System.Text.Encoding.CodePages` from `6.0.0` to `10.0.5`.
- Framework-included packages should be removed where assessment marks them as included by the new framework reference.
- Behavioral review is required for `System.IO.BinaryReader.ReadString` usage in `BinarySerializer\Graph\ValueGraph\ValueValueNode.cs` at lines `483` and `559`.
- `BinarySerializer.Performance\BinarySerializer.Performance.csproj` should remove `System.Runtime.Serialization.Formatters` if it remains unnecessary under `.NET 10`.

## Migration Strategy
### Approach Selection
This upgrade should use the **All-At-Once Strategy**.

The solution is well suited to a single coordinated update because it has only `3` projects, a clear dependency structure, no dependency cycles, low reported project difficulty, and limited package churn. A staged project-by-project migration would add coordination overhead without materially reducing risk.

### Atomic Upgrade Principle
All projects should be upgraded simultaneously in one bounded operation:
- update target frameworks across all project files and any imported MSBuild files
- update or remove package references across the solution
- restore dependencies for the whole solution
- resolve compilation issues introduced by framework and package changes
- verify the full solution builds cleanly

The plan intentionally avoids intermediate states such as upgrading one project while leaving dependants on older assumptions.

### Ordered Execution Within the Atomic Operation
Although the work is atomic, execution should still respect dependency-aware order inside the single batch:
1. Confirm prerequisites such as `.NET 10` SDK and any `global.json` alignment if present.
2. Update shared MSBuild settings if any are discovered during execution.
3. Update `BinarySerializer\BinarySerializer.csproj` to append `net10.0`.
4. Update `BinarySerializer.Performance\BinarySerializer.Performance.csproj` to `net10.0`.
5. Update `BinarySerializer.Test\BinarySerializer.Test.csproj` to `net10.0`.
6. Remove packages made redundant by the framework.
7. Apply the single explicit package upgrade for `System.Text.Encoding.CodePages`.
8. Address compile-time and behavior-sensitive issues discovered after restore/build.
9. Run the test projects after the atomic upgrade is complete.

### Parallel vs Sequential Decisions
- **Framework and package edit scope**: coordinated as one solution-wide batch.
- **Dependency reasoning**: sequential in analysis, atomic in execution.
- **Validation**: build first, then tests.
- **Troubleshooting priority**: fix root library issues before investigating dependant failures.

### Phase Definitions
#### Phase 0: Preparation
- Validate `.NET 10` SDK availability.
- Validate `global.json` compatibility if such a file exists.
- Confirm upgrade branch remains `upgrade-to-NET10`.

#### Phase 1: Atomic Upgrade
- Update all project target frameworks in one coordinated change set.
- Update all package references in one coordinated change set.
- Remove framework-included packages.
- Resolve code issues required for successful compilation.
- Achieve a full solution build with `0` errors.

#### Phase 2: Test Validation
- Execute all automated tests in `BinarySerializer.Test`.
- Review any runtime-sensitive behavior related to `BinaryReader.ReadString`.
- Confirm the performance project still restores and builds correctly against the upgraded shared library.

### Strategy-Specific Considerations
- Preserve the library's existing `netstandard1.3;net462` targets while appending `net10.0`, because the assessment explicitly proposes multi-targeting rather than replacement.
- Treat framework-included packages as removal candidates, not version upgrade candidates.
- Use a single source-control change set for the upgrade whenever practical, because the selected strategy favors one atomic commit or tightly bounded commit sequence.

## Detailed Dependency Analysis
### Dependency Graph Summary
The solution has a shallow two-level dependency graph with no circular references.

- **Level 0 / foundation**: `BinarySerializer\BinarySerializer.csproj`
- **Level 1 / dependants**:
  - `BinarySerializer.Performance\BinarySerializer.Performance.csproj`
  - `BinarySerializer.Test\BinarySerializer.Test.csproj`

Relationship summary:
- `BinarySerializer\BinarySerializer.csproj` has no project dependencies.
- `BinarySerializer.Performance\BinarySerializer.Performance.csproj` depends on `BinarySerializer\BinarySerializer.csproj`.
- `BinarySerializer.Test\BinarySerializer.Test.csproj` depends on `BinarySerializer\BinarySerializer.csproj`.
- No project depends on the performance or test projects.
- No circular dependencies were identified.

### Migration Grouping for Planning
Because the selected strategy is **All-At-Once**, the execution blueprint remains atomic even though dependency order still informs validation and troubleshooting.

#### Logical Foundation Group
- `BinarySerializer\BinarySerializer.csproj`

#### Logical Dependant Group
- `BinarySerializer.Performance\BinarySerializer.Performance.csproj`
- `BinarySerializer.Test\BinarySerializer.Test.csproj`

These groups are for understanding impact only. The actual framework and package update is planned as one coordinated solution-wide operation with no intermediate committed state.

### Critical Path
The critical compatibility path is:
`BinarySerializer\BinarySerializer.csproj` → `BinarySerializer.Performance\BinarySerializer.Performance.csproj`
`BinarySerializer\BinarySerializer.csproj` → `BinarySerializer.Test\BinarySerializer.Test.csproj`

Implications:
- The library target-framework change must be applied consistently before solution-wide validation can succeed.
- Any breaking change in the shared library has immediate impact on both dependant projects.
- The test project is the primary automated validation layer for the shared library after the atomic upgrade completes.

### Dependency-Driven Validation Priorities
During execution, validation should focus on these dependency-sensitive areas:
1. Multi-targeting correctness in `BinarySerializer\BinarySerializer.csproj`
2. Restore behavior after removal of framework-included packages
3. Consumer compatibility for `BinarySerializer.Performance` against the upgraded library target set
4. Test compatibility for `BinarySerializer.Test` against the upgraded library target set
5. Behavioral review for `BinaryReader.ReadString` usage in the shared library because the issue originates in the dependency root

### Circular Dependency Assessment
No circular dependencies are present. No special cycle-breaking plan is required.

## Project-by-Project Plans
### Project: `BinarySerializer\BinarySerializer.csproj`
**Current State**
- Project kind: `ClassLibrary`
- Current target framework: `netstandard1.3;net462`
- Dependencies: `0`
- Dependants: `2`
- Files: `114`
- LOC: `11,029`
- Reported issues: `15`
- Risk level: `Medium`

**Target State**
- Target frameworks: `netstandard1.3;net462;net10.0`
- Package posture: remove framework-included packages; preserve compatible packages where still needed

**Migration Steps**
1. **Prerequisites**
   - Verify whether shared MSBuild props/targets files influence target frameworks or package references.
   - Confirm multi-targeting conditions remain valid after appending `net10.0`.
2. **Project file updates**
   - Append `net10.0` to the existing target framework list rather than replacing `netstandard1.3;net462`.
   - Reevaluate package references identified as framework-included in the assessment.
3. **Package/module/dependency updates**

   | Package | Current | Target | Action | Reason |
   |---|---:|---:|---|---|
   | `NETStandard.Library` | `1.6.1` | Keep | Retain unless execution confirms it is no longer needed for existing targets | Marked compatible |
   | `Microsoft.SourceLink.GitHub` | `1.1.1` | Keep | Retain | Marked compatible |
   | `System.Collections` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.IO` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Linq` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Linq.Expressions` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Reflection` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Reflection.Extensions` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Reflection.TypeExtensions` | `4.7.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Resources.ResourceManager` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Runtime` | `4.3.1` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Runtime.Extensions` | `4.3.1` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Text.Encoding` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
   | `System.Threading` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
4. **Expected breaking changes**
   - Review behavioral change warnings for `System.IO.BinaryReader.ReadString`.
   - Verify any target-framework-specific conditional compilation or package conditions that assume only legacy targets.
5. **Code modifications to plan for**
   - Inspect `BinarySerializer\Graph\ValueGraph\ValueValueNode.cs` at lines `483` and `559`.
   - Validate string deserialization assumptions, especially around malformed payloads, length handling, or runtime behavior changes under `.NET 10`.
6. **Testing strategy**
   - Rebuild all target variants exposed by the project file.
   - Run the full dependent test project after the atomic upgrade.
   - Add regression focus on serialization and deserialization of strings.
7. **Validation checklist**
   - [ ] Project targets include `net10.0` while preserving existing targets
   - [ ] Framework-included package references removed where appropriate
   - [ ] No restore conflicts across target frameworks
   - [ ] Shared library compiles without errors
   - [ ] String serialization tests pass

### Project: `BinarySerializer.Performance\BinarySerializer.Performance.csproj`
**Current State**
- Project kind: `DotNetCoreApp`
- Current target framework: `netcoreapp3.1`
- Dependencies: `1`
- Dependants: `0`
- Files: `5`
- LOC: `287`
- Reported issues: `2`
- Risk level: `Low`

**Target State**
- Target framework: `net10.0`
- Package posture: remove framework-included serialization formatter package if no longer required

**Migration Steps**
1. **Prerequisites**
   - Confirm the shared library project has been updated in the same atomic change set.
2. **Project file updates**
   - Change target framework from `netcoreapp3.1` to `net10.0`.
3. **Package/module/dependency updates**

   | Package | Current | Target | Action | Reason |
   |---|---:|---:|---|---|
   | `Microsoft.SourceLink.GitHub` | `1.1.1` | Keep | Retain | Marked compatible |
   | `System.Runtime.Serialization.Formatters` | `4.3.0` | Remove | Remove package reference | Functionality included with framework reference |
4. **Expected breaking changes**
   - No project-specific API incompatibilities were reported, but runtime or restore issues may surface after the framework jump from `netcoreapp3.1` to `net10.0`.
5. **Code modifications to plan for**
   - No direct code edits are predicted by assessment.
   - Review only if build or runtime validation surfaces formatter-related changes.
6. **Testing strategy**
   - Restore and build against the upgraded shared library.
   - Run any existing benchmark or smoke validation if automated.
7. **Validation checklist**
   - [ ] Target framework updated to `net10.0`
   - [ ] Formatter package removed if redundant
   - [ ] Project builds cleanly against upgraded library

### Project: `BinarySerializer.Test\BinarySerializer.Test.csproj`
**Current State**
- Project kind: `DotNetCoreApp`
- Current target framework: `net7.0`
- Dependencies: `1`
- Dependants: `0`
- Files: `484`
- LOC: `11,282`
- Reported issues: `2`
- Risk level: `Medium`

**Target State**
- Target framework: `net10.0`
- Package posture: upgrade `System.Text.Encoding.CodePages` to `10.0.5`; retain compatible test tooling unless execution discovers newer minimums are required

**Migration Steps**
1. **Prerequisites**
   - Keep this project aligned with the shared library's new target set in the same atomic upgrade.
2. **Project file updates**
   - Change target framework from `net7.0` to `net10.0`.
3. **Package/module/dependency updates**

   | Package | Current | Target | Action | Reason |
   |---|---:|---:|---|---|
   | `System.Text.Encoding.CodePages` | `6.0.0` | `10.0.5` | Upgrade | Recommended by assessment |
   | `Microsoft.NET.Test.Sdk` | `17.6.2` | Keep | Retain initially | Marked compatible |
   | `MSTest.TestAdapter` | `3.0.4` | Keep | Retain initially | Marked compatible |
   | `MSTest.TestFramework` | `3.0.4` | Keep | Retain initially | Marked compatible |
   | `coverlet.collector` | `6.0.0` | Keep | Retain initially | Marked compatible |
   | `coverlet.msbuild` | `6.0.0` | Keep | Retain initially | Marked compatible |
4. **Expected breaking changes**
   - No API incompatibilities were reported for test code, but assertion, encoding, or runner behavior should be revalidated under `.NET 10`.
5. **Code modifications to plan for**
   - No direct test-code edits are predicted by assessment.
   - If encoding-related failures appear, focus first on scenarios relying on code pages and binary/text round-tripping.
6. **Testing strategy**
   - This project is the primary automated validation gate for the upgrade.
   - Run the full suite after all framework and package updates are complete.
7. **Validation checklist**
   - [ ] Target framework updated to `net10.0`
   - [ ] `System.Text.Encoding.CodePages` upgraded to `10.0.5`
   - [ ] All tests execute successfully
   - [ ] No package dependency conflicts remain

## Package Update Reference
### Common Package Cleanup Across the Upgrade
| Package | Current | Target | Projects Affected | Action | Update Reason |
|---|---:|---:|---|---|---|
| `System.Collections` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.IO` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Linq` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Linq.Expressions` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Reflection` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Reflection.Extensions` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Reflection.TypeExtensions` | `4.7.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Resources.ResourceManager` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Runtime` | `4.3.1` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Runtime.Extensions` | `4.3.1` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Text.Encoding` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Threading` | `4.3.0` | Remove | `BinarySerializer` | Remove | Included with framework reference |
| `System.Runtime.Serialization.Formatters` | `4.3.0` | Remove | `BinarySerializer.Performance` | Remove | Included with framework reference |

### Explicit Version Upgrade
| Package | Current | Target | Projects Affected | Action | Update Reason |
|---|---:|---:|---|---|---|
| `System.Text.Encoding.CodePages` | `6.0.0` | `10.0.5` | `BinarySerializer.Test` | Upgrade | Recommended by assessment for `.NET 10` compatibility |

### Compatible Packages to Leave Unchanged Initially
| Package | Version | Projects |
|---|---:|---|
| `NETStandard.Library` | `1.6.1` | `BinarySerializer` |
| `Microsoft.SourceLink.GitHub` | `1.1.1` | `BinarySerializer`, `BinarySerializer.Performance` |
| `Microsoft.NET.Test.Sdk` | `17.6.2` | `BinarySerializer.Test` |
| `MSTest.TestAdapter` | `3.0.4` | `BinarySerializer.Test` |
| `MSTest.TestFramework` | `3.0.4` | `BinarySerializer.Test` |
| `coverlet.collector` | `6.0.0` | `BinarySerializer.Test` |
| `coverlet.msbuild` | `6.0.0` | `BinarySerializer.Test` |

## Breaking Changes Catalog
### Confirmed Assessment Findings
| Area | Location | Type | Planned Response |
|---|---|---|---|
| `BinaryReader.ReadString` behavior | `BinarySerializer\Graph\ValueGraph\ValueValueNode.cs:483` | Behavioral change | Review runtime assumptions and validate with regression tests |
| `BinaryReader.ReadString` behavior | `BinarySerializer\Graph\ValueGraph\ValueValueNode.cs:559` | Behavioral change | Review runtime assumptions and validate with regression tests |

### Expected Upgrade-Sensitive Areas
- **Multi-targeting conditions**: appending `net10.0` may require checking conditional property groups or item groups in `BinarySerializer\BinarySerializer.csproj` and any imported MSBuild files.
- **Framework-included package cleanup**: removal of legacy `System.*` package references may change compile-time resolution and should be validated across all target frameworks.
- **Encoding behavior**: `System.Text.Encoding.CodePages` version change may affect tests or scenarios that depend on explicit code page registration.
- **Formatter behavior**: the performance project should be reviewed if removal of `System.Runtime.Serialization.Formatters` changes any serialization assumptions.

### Breaking Change Investigation Priorities
1. Root-library string deserialization behavior
2. Multi-targeting and restore conditions in the shared library
3. Test encoding and binary round-trip scenarios
4. Performance project restore/build behavior after package cleanup

## Testing & Validation Strategy
### Solution-Level Validation
Because this is an all-at-once upgrade, validation should occur after the full framework and package batch is applied.

Primary validation goals:
- entire solution restores successfully
- entire solution builds with `0` errors
- automated tests pass
- no package dependency conflicts remain
- shared-library behavioral warnings have explicit regression coverage

### Project-Level Validation
| Project | Validation Focus |
|---|---|
| `BinarySerializer` | Multi-targeting correctness, package cleanup, string deserialization behavior |
| `BinarySerializer.Performance` | Restore/build compatibility after framework jump and formatter cleanup |
| `BinarySerializer.Test` | Full automated regression suite, encoding/code-page scenarios, dependent compatibility |

### Validation Sequence
1. Verify SDK and repository prerequisites.
2. Apply all project and package changes in one coordinated batch.
3. Restore dependencies for the solution.
4. Build the full solution.
5. Fix all compilation errors introduced by the upgrade.
6. Rebuild to confirm a clean solution state.
7. Run the test project `BinarySerializer.Test`.
8. Review results for string deserialization and encoding-sensitive paths.

### Validation Checklists
#### Atomic Upgrade Completion
- [ ] All project files reflect target framework changes from the assessment
- [ ] All package removals and upgrades from the assessment are applied
- [ ] Solution restore succeeds
- [ ] Solution build succeeds without errors

#### Test Validation Completion
- [ ] `BinarySerializer.Test` executes successfully
- [ ] String serialization and deserialization scenarios pass
- [ ] Encoding/code-page dependent tests pass
- [ ] No unresolved upgrade-related regressions remain

## Source Control Strategy
### Branching Strategy
- Working branch: `upgrade-to-NET10`
- Source branch baseline: `master`
- Keep the `.NET 10` upgrade isolated to the dedicated upgrade branch until the atomic change set is validated.

### Commit Strategy
Because this plan uses the **All-At-Once Strategy**, prefer a **single coordinated commit** for the full framework and package upgrade if practical.

If one commit is not practical, keep the sequence tightly bounded:
1. prerequisite-only branch preparation changes if required
2. one coordinated commit for all project framework and package updates
3. one follow-up commit only if needed for upgrade-related compilation or test fixes

Recommended commit-message pattern:
- `Upgrade solution to .NET 10`
- `Fix .NET 10 upgrade compatibility issues`

### Review and Merge Process
- Review the shared library changes first because `BinarySerializer` is the dependency root.
- Confirm the review includes target-framework changes, package cleanup, and the `System.Text.Encoding.CodePages` version update.
- Require evidence that the final solution build and automated tests passed before merging.
- Avoid partial merges of individual project upgrades because the plan depends on atomic compatibility.

## Success Criteria
### Technical Criteria
- All projects target the frameworks proposed by the assessment:
  - `BinarySerializer\BinarySerializer.csproj` → `netstandard1.3;net462;net10.0`
  - `BinarySerializer.Performance\BinarySerializer.Performance.csproj` → `net10.0`
  - `BinarySerializer.Test\BinarySerializer.Test.csproj` → `net10.0`
- All package actions from the assessment are applied:
  - `System.Text.Encoding.CodePages` upgraded from `6.0.0` to `10.0.5`
  - framework-included packages removed where identified by assessment
- Solution restore completes without dependency conflicts.
- Solution builds without errors.
- Automated tests pass.
- No security vulnerabilities remain reported as part of the upgraded dependency set.

### Quality Criteria
- Existing multi-targeting support remains intact for `BinarySerializer` unless a later decision explicitly changes support policy.
- Upgrade-related behavior changes are reviewed for `BinaryReader.ReadString` call sites.
- No unresolved warnings or compatibility concerns remain that would block use of the `.NET 10` target.
- Documentation artifacts for the upgrade remain consistent: `assessment.md`, `plan.md`, and later execution artifacts should align on scope and package decisions.

### Process Criteria
- The upgrade follows the **All-At-Once Strategy** with one coordinated solution-wide update.
- Dependency order is respected in analysis and troubleshooting even though execution is atomic.
- Source control remains isolated on `upgrade-to-NET10` until validation is complete.
- The upgrade is ready for task generation without requiring additional discovery.
