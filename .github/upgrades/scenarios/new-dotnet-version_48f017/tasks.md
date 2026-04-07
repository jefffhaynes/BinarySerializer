# BinarySerializer .NET 10 Upgrade Tasks

## Overview

This document tracks the execution of the BinarySerializer solution upgrade from multiple .NET versions to .NET 10. All three projects will be upgraded simultaneously in a single atomic operation, followed by testing and validation.

**Progress**: 0/4 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §Phase 0

- [✓] (1) Verify .NET 10 SDK is installed
- [✓] (2) .NET 10 SDK version meets minimum requirements (**Verify**)
- [▶] (3) Check global.json compatibility if file exists in repository
- [ ] (4) global.json compatible with .NET 10 or file does not exist (**Verify**)

---

### [ ] TASK-002: Atomic framework and package upgrade
**References**: Plan §Phase 1, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [ ] (1) Update BinarySerializer\BinarySerializer.csproj to append net10.0 to existing targets per Plan §Project: BinarySerializer
- [ ] (2) Update BinarySerializer.Performance\BinarySerializer.Performance.csproj to net10.0 per Plan §Project: BinarySerializer.Performance
- [ ] (3) Update BinarySerializer.Test\BinarySerializer.Test.csproj to net10.0 per Plan §Project: BinarySerializer.Test
- [ ] (4) All project files updated to target frameworks (**Verify**)
- [ ] (5) Remove framework-included packages per Plan §Package Update Reference (13 packages from BinarySerializer, 1 from BinarySerializer.Performance)
- [ ] (6) Upgrade System.Text.Encoding.CodePages from 6.0.0 to 10.0.5 in BinarySerializer.Test per Plan §Package Update Reference
- [ ] (7) All package references updated (**Verify**)
- [ ] (8) Restore all dependencies for the solution
- [ ] (9) All dependencies restored successfully (**Verify**)
- [ ] (10) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog
- [ ] (11) Solution builds with 0 errors (**Verify**)

---

### [ ] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Phase 2 Testing, Plan §Breaking Changes Catalog

- [ ] (1) Run tests in BinarySerializer.Test project
- [ ] (2) Fix any test failures focusing on BinaryReader.ReadString behavioral changes per Plan §Breaking Changes (lines 483, 559 in ValueValueNode.cs)
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "Upgrade BinarySerializer solution to .NET 10"

---


