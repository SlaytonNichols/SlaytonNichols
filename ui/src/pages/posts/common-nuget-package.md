---
title: "Common Nuget Package Architecture"
summary: "Common Nuget Package Architecture"
date: 2022-10-19
tags:
  - dotnet
  - architecture
  - nuget
draft: true
---
## Common Nuget Package Architecture

NuGet is a package manager for the .NET platform that makes it easy to install, update, and manage libraries and tools in your .NET projects. NuGet packages are distributed as ZIP files that contain compiled code, resources, and metadata, and can be published to public or private package repositories.

Using NuGet packages can help you centralize logic and reusable components in your projects, and make it easier to share and reuse them across different projects. This can save you time and effort by avoiding the need to manually copy and paste code between projects, and can also help you keep track of the dependencies and versioning of your code.

In this blog post, we will show you how to create a NuGet package from a .NET project, deploy it to a package repository, and use it in other projects. We will also demonstrate how to automate the process of building and deploying the NuGet package using a GitHub Action. By the end of this post, you should have a good understanding of the benefits of using NuGet packages and how to use them in your own projects.
This article provides an overview of the architecture used in [SlaytonNichols.Common](https://github.com/SlaytonNichols/SlaytonNichols.Common).

### Package
```
SlaytonNichols.Common
 ┣ .github
 ┃ ┗ workflows
 ┃ ┃ ┗ nuget.yml
 ┣ .vscode
 ┃ ┣ launch.json
 ┃ ┗ tasks.json
 ┣ Infrastructure
 ┃ ┣ Cron
 ┃ ┃ ┣ CronClient.cs
 ┃ ┃ ┗ ICronClient.cs
 ┃ ┣ Logging
 ┃ ┃ ┣ CustomLogger.cs
 ┃ ┃ ┣ CustomLoggerConfiguration.cs
 ┃ ┃ ┣ CustomLoggerExtensions.cs
 ┃ ┃ ┣ CustomLoggerProvider.cs
 ┃ ┃ ┗ LogEntry.cs
 ┃ ┗ MongoDb
 ┃ ┃ ┣ Repositories
 ┃ ┃ ┃ ┣ IMongoRepository.cs
 ┃ ┃ ┃ ┗ MongoRepository.cs
 ┃ ┃ ┣ BsonCollectionAttribute.cs
 ┃ ┃ ┣ Document.cs
 ┃ ┃ ┗ IDocument.cs
 ┣ out
 ┃ ┣ SlaytonNichols.Common.1.0.0.nupkg
 ┃ ┗ SlaytonNichols.Common.1.0.24.nupkg
 ┣ ServiceStack
 ┃ ┣ Auth
 ┃ ┃ ┣ AppUserAuthEvents.cs
 ┃ ┃ ┣ CustomRegistrationValidator.cs
 ┃ ┃ ┗ CustomUserSession.cs
 ┃ ┗ Configure.cs
 ┣ .gitignore
 ┣ CommonDependencyInjectionExtensions.cs
 ┣ nuget.config
 ┗ SlaytonNichols.Common.csproj
```

### Creating a Nuget Package

Open a terminal window and navigate to the root directory of your .NET project.

Run the following command to create a .csproj file for your project, if it does not already exist:

```sh
dotnet new console -n <project_name>
```
Add the <PackageReference> element to the .csproj file to specify the package metadata and dependencies. For example:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
  </ItemGroup>
</Project>
```
Run the following command to build the NuGet package:
```sh
dotnet pack
```
This will create a .nupkg file in the bin/Debug or bin/Release directory, depending on the configuration of your project. You can use the --configuration flag to specify the build configuration (e.g. dotnet pack --configuration Release).

To specify the package version, you can use the --version-suffix flag:
Copy code
dotnet pack --version-suffix 1.0.0
This will create a NuGet package with the version specified in the .csproj file, plus the suffix specified in the --version-suffix flag.

### Build Pipeline
1. Set up a personal access token: You will need a personal access token (PAT) to authenticate with the GitHub Package Registry. You can create a PAT by going to your GitHub Settings > Developer Settings > Personal Access Tokens, and clicking the "Generate token" button. Make sure to give the PAT the write:packages scope, so that it has permission to publish packages to your repository.
2. Add the PAT to your repository secrets: Go to your repository Settings > Secrets, and click the "New secret" button. Give the secret the name PAT_TOKEN, and paste the value of your personal access token in the "Value" field.
3. Create a GitHub Action workflow: In your repository, create a new file in the .github/workflows directory and give it a name (e.g. package.yml). This file will contain the workflow configuration for building and deploying the NuGet package.
4. Add the following YAML code to the workflow file to configure the build and deploy steps:
```
name: NuGet Generation

on:
  push:
    branches:
      - main

jobs:
  build:
    runs-on: ubuntu-18.04
    name: Update NuGet package
    steps:
      - uses: actions/delete-package-versions@v3
        with:
          package-name: 'SlaytonNichols.Common'
          min-versions-to-keep: 3

      - name: Checkout repository
        uses: actions/checkout@v1

      - name: Setup .NET Core @ Latest
        uses: actions/setup-dotnet@v1
        with:
          source-url: https://nuget.pkg.github.com/SlaytonNichols/index.json
        env:
          NUGET_AUTH_TOKEN: ${{secrets.GITHUB_TOKEN}}        
          
      - name: Build solution and generate NuGet package
        run: |  
          cd ../SlaytonNichols.Common
          dotnet pack -c Release -o out  

      - name: Push generated package to GitHub registry
        run: dotnet nuget push **\*.nupkg --api-key ${{secrets.GITHUB_TOKEN}} --skip-duplicate
```
This workflow will run on every push to the main branch, and will perform the following steps:
- Check out the repository code
- Set up the .NET SDK
- Build the NuGet package using dotnet pack
- Publish the NuGet package to the GitHub Package Registry using the actions/nuget/publish action and the personal access token stored in the repository secrets.

5. Commit and push the workflow file to the repository: Once you have added the workflow file to your repository, commit and push it to the main branch. This will trigger the workflow and start the build and deploy process.
