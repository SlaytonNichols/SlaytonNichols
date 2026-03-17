import{l as s,o as i,h as r,w as c,m as l,b as e,d as t}from"./app.333304cd.js";const u=e("div",{class:"markdown-body"},[e("h2",null,"Common Nuget Package Architecture"),e("p",null,"NuGet is a package manager for the .NET platform that makes it easy to install, update, and manage libraries and tools in your .NET projects. NuGet packages are distributed as ZIP files that contain compiled code, resources, and metadata, and can be published to public or private package repositories."),e("p",null,"Using NuGet packages can help you centralize logic and reusable components in your projects, and make it easier to share and reuse them across different projects. This can save you time and effort by avoiding the need to manually copy and paste code between projects, and can also help you keep track of the dependencies and versioning of your code."),e("p",null,[t("In this blog post, we will show you how to create a NuGet package from a .NET project, deploy it to a package repository, and use it in other projects. We will also demonstrate how to automate the process of building and deploying the NuGet package using a GitHub Action. By the end of this post, you should have a good understanding of the benefits of using NuGet packages and how to use them in your own projects. This article provides an overview of the architecture used in "),e("a",{href:"https://github.com/SlaytonNichols/SlaytonNichols.Common"},"SlaytonNichols.Common"),t(".")]),e("h3",null,"Package"),e("pre",null,[e("code",null,`SlaytonNichols.Common
 \u2523 .github
 \u2503 \u2517 workflows
 \u2503 \u2503 \u2517 nuget.yml
 \u2523 .vscode
 \u2503 \u2523 launch.json
 \u2503 \u2517 tasks.json
 \u2523 Infrastructure
 \u2503 \u2523 Cron
 \u2503 \u2503 \u2523 CronClient.cs
 \u2503 \u2503 \u2517 ICronClient.cs
 \u2503 \u2523 Logging
 \u2503 \u2503 \u2523 CustomLogger.cs
 \u2503 \u2503 \u2523 CustomLoggerConfiguration.cs
 \u2503 \u2503 \u2523 CustomLoggerExtensions.cs
 \u2503 \u2503 \u2523 CustomLoggerProvider.cs
 \u2503 \u2503 \u2517 LogEntry.cs
 \u2503 \u2517 MongoDb
 \u2503 \u2503 \u2523 Repositories
 \u2503 \u2503 \u2503 \u2523 IMongoRepository.cs
 \u2503 \u2503 \u2503 \u2517 MongoRepository.cs
 \u2503 \u2503 \u2523 BsonCollectionAttribute.cs
 \u2503 \u2503 \u2523 Document.cs
 \u2503 \u2503 \u2517 IDocument.cs
 \u2523 out
 \u2503 \u2523 SlaytonNichols.Common.1.0.0.nupkg
 \u2503 \u2517 SlaytonNichols.Common.1.0.24.nupkg
 \u2523 ServiceStack
 \u2503 \u2523 Auth
 \u2503 \u2503 \u2523 AppUserAuthEvents.cs
 \u2503 \u2503 \u2523 CustomRegistrationValidator.cs
 \u2503 \u2503 \u2517 CustomUserSession.cs
 \u2503 \u2517 Configure.cs
 \u2523 .gitignore
 \u2523 CommonDependencyInjectionExtensions.cs
 \u2523 nuget.config
 \u2517 SlaytonNichols.Common.csproj
`)]),e("h3",null,"Creating a Nuget Package"),e("p",null,"Open a terminal window and navigate to the root directory of your .NET project."),e("p",null,"Run the following command to create a .csproj file for your project, if it does not already exist:"),e("pre",null,[e("code",{class:"language-sh"},`dotnet new console -n <project_name>
`)]),e("p",null,[t("Add the "),e("code",null,"PackageReference"),t(" element to the "),e("code",null,".csproj"),t(" file to specify the package metadata and dependencies. For example:")]),e("pre",null,[e("code",{class:"language-xml"},`<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
  </ItemGroup>
</Project>
`)]),e("p",null,"Run the following command to build the NuGet package:"),e("pre",null,[e("code",{class:"language-sh"},`dotnet pack
`)]),e("p",null,"This will create a .nupkg file in the bin/Debug or bin/Release directory, depending on the configuration of your project. You can use the --configuration flag to specify the build configuration (e.g. dotnet pack --configuration Release)."),e("p",null,"To specify the package version, you can use the --version-suffix flag: Copy code dotnet pack --version-suffix 1.0.0 This will create a NuGet package with the version specified in the .csproj file, plus the suffix specified in the --version-suffix flag."),e("h3",null,"Build Pipeline"),e("ol",null,[e("li",null,"Set up a personal access token: You will need a personal access token (PAT) to authenticate with the GitHub Package Registry. You can create a PAT by going to your GitHub Settings > Developer Settings > Personal Access Tokens, and clicking the \u201CGenerate token\u201D button. Make sure to give the PAT the write:packages scope, so that it has permission to publish packages to your repository."),e("li",null,"Add the PAT to your repository secrets: Go to your repository Settings > Secrets, and click the \u201CNew secret\u201D button. Give the secret the name PAT_TOKEN, and paste the value of your personal access token in the \u201CValue\u201D field."),e("li",null,"Create a GitHub Action workflow: In your repository, create a new file in the .github/workflows directory and give it a name (e.g. package.yml). This file will contain the workflow configuration for building and deploying the NuGet package."),e("li",null,"Add the following YAML code to the workflow file to configure the build and deploy steps:")]),e("pre",null,[e("code",null,`name: NuGet Generation

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
          NUGET_AUTH_TOKEN: \${{secrets.GITHUB_TOKEN}}        
          
      - name: Build solution and generate NuGet package
        run: |  
          cd ../SlaytonNichols.Common
          dotnet pack -c Release -o out  

      - name: Push generated package to GitHub registry
        run: dotnet nuget push **\\*.nupkg --api-key \${{secrets.GITHUB_TOKEN}} --skip-duplicate
`)]),e("p",null,"This workflow will run on every push to the main branch, and will perform the following steps:"),e("ul",null,[e("li",null,"Check out the repository code"),e("li",null,"Set up the .NET SDK"),e("li",null,"Build the NuGet package using dotnet pack"),e("li",null,"Publish the NuGet package to the GitHub Package Registry using the actions/nuget/publish action and the personal access token stored in the repository secrets.")]),e("ol",{start:"5"},[e("li",null,"Commit and push the workflow file to the repository: Once you have added the workflow file to your repository, commit and push it to the main branch. This will trigger the workflow and start the build and deploy process.")])],-1),k="Common Nuget Package Architecture",f="Common Nuget Package Architecture",y="2022-10-19T00:00:00.000Z",w=["dotnet","architecture","nuget"],N=!0,b=[{property:"og:title",content:"Common Nuget Package Architecture"}],C={__name:"common-nuget-package",setup(p,{expose:n}){const o={title:"Common Nuget Package Architecture",summary:"Common Nuget Package Architecture",date:"2022-10-19T00:00:00.000Z",tags:["dotnet","architecture","nuget"],draft:!0,meta:[{property:"og:title",content:"Common Nuget Package Architecture"}]};return n({frontmatter:o}),s({title:"Common Nuget Package Architecture",meta:[{property:"og:title",content:"Common Nuget Package Architecture"}]}),(h,d)=>{const a=l;return i(),r(a,{frontmatter:o},{default:c(()=>[u]),_:1})}}};export{y as date,C as default,N as draft,b as meta,f as summary,w as tags,k as title};
