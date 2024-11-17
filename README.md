    Example appilcation with basic GitHub integration


# Project status

[![.NET](https://github.com/damikulik/RepoStats/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/damikulik/RepoStats/actions/workflows/dotnet.yml)

# How to run and test
TBD

# Architecture
TBD

## System Context

```mermaid
C4Context
    title System Context diagram for Internet Banking System
    Enterprise_Boundary(www, "Internet") {
        Person(user, "API Consumer", "Client consuming API.")

        System_Boundary(repoStatsB, "RepoStats") {
            System(repoStats, "RepoStats", "The application for calculating statistics for GibHub Repositories.")
        }

        System_Boundary(githubB, "GitHub") {
             System_Ext(github, "GitHub", "The complete developer platform to build, scale, and deliver secure software.")
        }
    }

    Rel(user, repoStats, "Requests statistics", "Rest API")
    Rel(repoStats, github, "Fetches repository content", "Rest API")
```

# Statistics Calculator details
TBD
