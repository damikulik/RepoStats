    Example appilcation with basic GitHub integration


# Project status

[![.NET](https://github.com/damikulik/RepoStats/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/damikulik/RepoStats/actions/workflows/dotnet.yml)

# How to run and test

There is an `GitHub API` secret which must be set.

| Project | Key |
| -- | -- |
| App | `RepoStats:GitHubLoader:SecurityKey` |
| IntegrationTests | `SecurityKey` |

# API

## Letter occurence Statistics
```
    GET /letter-occurences/

    Returns: 200, 429, 503
    Formatting: application/json
```

# Architecture
Solution uses a simple DDD with Clean Architecture-like approach.

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

## Main features

* Calculator is used by Hosted Service which runs periodically.
* API returns 503 if there is no Statistics calculated yet.

# Statistics Calculator details

Calculator takes advantage of Parallel processing and although Character count is a classic Map-Reduce case it is more optimal to use modified algorythm to produce Statistics in this case.

## Implementation debate
Few implementations of the Calculator are provided to select the most performant

* Naive
* MapReduce
* DirectReduce
* ConcurrentDirectReduce (baseline, used implementation)

## Benchmart Results

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5011/22H2/2022Update)
Intel Core i9-10885H CPU 2.40GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.400
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2


```
| Method       | RepositoryFiles | Mean       | Error    | StdDev    | Ratio | RatioSD | Completed Work Items | Lock Contentions | Gen0       | Gen1      | Allocated | Alloc Ratio |
|------------- |---------------- |-----------:|---------:|----------:|------:|--------:|---------------------:|-----------------:|-----------:|----------:|----------:|------------:|
| **Current**      | **1**               |   **382.1 ms** |  **7.25 ms** |   **6.79 ms** |  **1.00** |    **0.02** |               **4.0000** |                **-** | **27000.0000** |         **-** | **231.03 MB** |        **1.00** |
| DirectReduce | 1               |   272.2 ms |  6.16 ms |  17.97 ms |  0.71 |    0.05 |               2.0000 |                - | 27000.0000 |         - | 230.86 MB |        1.00 |
| MapReduce    | 1               |   193.8 ms |  4.02 ms |  11.54 ms |  0.51 |    0.03 |               2.0000 |                - | 10500.0000 |         - | 127.54 MB |        0.55 |
| Naive        | 1               |   201.6 ms |  4.44 ms |  12.73 ms |  0.53 |    0.03 |               2.0000 |                - | 10500.0000 |         - | 127.67 MB |        0.55 |
|              |                 |            |          |           |       |         |                      |                  |            |           |           |             |
| **Current**      | **100**             |   **389.2 ms** |  **7.55 ms** |  **10.83 ms** |  **1.00** |    **0.04** |             **117.0000** |         **179.0000** | **19000.0000** | **7000.0000** |  **157.6 MB** |        **1.00** |
| DirectReduce | 100             | 4,839.6 ms | 96.78 ms | 125.84 ms | 12.44 |    0.46 |             101.0000 |                - | 20000.0000 | 2000.0000 | 161.99 MB |        1.03 |
| MapReduce    | 100             | 4,805.1 ms | 94.52 ms | 144.34 ms | 12.35 |    0.50 |             101.0000 |                - | 11000.0000 | 2000.0000 |  93.83 MB |        0.60 |
| Naive        | 100             | 4,806.5 ms | 93.81 ms | 118.64 ms | 12.36 |    0.45 |             101.0000 |                - | 10000.0000 | 2000.0000 |  88.79 MB |        0.56 |

# Future work

1. Per file stats calculation could be added to avoid recalculating Stats for files which did not change.
2. Implementation of the Web hooks would be more optimal, especially in combination with #1.  
   Combining it with a simple data store would help with optimizaiton of Repository usage.
3. With minimal changes:
    * more repositories can be supported
    * more Calculators can be added
    * configuration could be provided to control things like Character or Case Sensitivity
    * allow different type of filters besides programming language
4. It is a good example of App which could be hosted as a Function App, it would require addition of simple Storage model though.
5. Bigger repositories would most probably require another approach to obtain the list of the files matching the search criteria.
6. Separate IntegrationTests Action run and make it time-based or pre-release based.
