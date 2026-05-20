# CI/CD workflows for .NET and GitHub

A reusable workflow collection to be used to carry out the CI/CD process of a microservice.

## Test Inputs

Use the `tests` input with a JSON array of strings.

Use `[]` (default) to skip tests.

Each string contains: `<project_path> [optional_args]`

### YAML Syntax Example

```yaml
on:
  workflow_call:

jobs:
  ci:
    uses: EasyDesk/workflows/.github/workflows/microservice-cicd.yml@master
    with:
      dotnet-version: 9.x
      main-project: src/MyService/MyService.csproj
      tests: |
        [
          "tests/MyService.UnitTests/MyService.UnitTests.csproj",
          "tests/MyService.IntegrationTests/MyService.IntegrationTests.csproj --filter \"Category=Integration\""
        ]
```

### With Project Paths Containing Spaces

```yaml
jobs:
  ci:
    uses: EasyDesk/workflows/.github/workflows/microservice-cicd.yml@master
    with:
      dotnet-version: 9.x
      main-project: src/MyService/MyService.csproj
      tests: |
        [
          "tests/Project.UnitTests/Project.UnitTests.csproj",
          "\"tests/My Test Project/Project.IntegrationTests.csproj\" --filter \"Category=Integration\""
        ]
```
