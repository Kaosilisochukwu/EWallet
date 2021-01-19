# EWallet

An E-Wallet Management System

## Getting Started

The followint instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Before you get started you should have a dotnet runtime installed on your local machine.

### Installing

## Option 1

- clone the repository by typing in the following commant in the folder you want to work with

```bash
git clone https://github.com/Kaosilisochukwu/EWallet.git
```

- change the connection string on the appsetting.json file
- type

```bash
  dotnet ef database update
```

you can build the file to to install all dependencies by running

```bash
dotnet build
```

in the project file

then run the project with

```bash
dotnet run
```

to run the application and start testing the endpoint on your.

follow this link to the documentation

- [Link](https://noobelitewallet.herokuapp.com/swagger/index.html)
- [Documentation](https://docs.google.com/spreadsheets/d/1vDK73LtvL4geYoGEt81woP72SUz18sXqVZipLrk2gTc/edit?usp=sharing)

# Technology used

- Database: SQLite
- Language: ASP.Net C#
- ORM: Entity Framework
- Authentication: JWT
- Containerization: Docker
