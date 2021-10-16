# This project is used for testing Kronii Web API

## Folder structure

> After done your project structure must look like this

```
kronii
 |-- kronii-api
 |       |-- kroniiapi.csproj
 |       |-- ...
 |-- kroniiapiTest
 |       |-- kroniiapiTest.csproj
 |       |-- ...
 |-- kronii.sln
```

## Setup instruction

> Follow this instruction to setup your test project with your Web API project

-   Go to _OUTSIDE_ of your Web API project folder run every command:

```
dotnet new sln -o kronii
cd kronii
```

-   Copy your Web API project to inside the **kronii** solution
-   Clone this project inside of **kronii** solution folder too
-   After cloned, follow this step:

```
dotnet sln add kronii-api/kroniiapi.csproj
dotnet sln add kroniiapiTest/kroniiapiTest.csproj
```

-   After add api project and test project to **kronii** solution, you need to reference api project from test project

```
cd kroniiapiTest
dotnet add reference ../kronii-api/kroniiapi.csproj
dotnet restore kroniiapiTest.csproj
```

## Before code

-   **_Only change code in your task!!_**
-   Careful about the project name and folder path
