# StocksData

[![CI](https://github.com/PFalkowski/StocksData/actions/workflows/ci.yml/badge.svg)](https://github.com/PFalkowski/StocksData/actions/workflows/ci.yml)
[![NuGet (Model)](https://img.shields.io/nuget/v/Stocks.Data.Model.svg?label=Model)](https://www.nuget.org/packages/Stocks.Data.Model/)
[![NuGet (Infrastructure)](https://img.shields.io/nuget/v/Stocks.Data.Infrastructure.svg?label=Infrastructure)](https://www.nuget.org/packages/Stocks.Data.Infrastructure/)
[![NuGet (Ef)](https://img.shields.io/nuget/v/Stocks.Data.Ef.svg?label=Ef)](https://www.nuget.org/packages/Stocks.Data.Ef/)
[![NuGet (Ado)](https://img.shields.io/nuget/v/Stocks.Data.Ado.svg?label=Ado)](https://www.nuget.org/packages/Stocks.Data.Ado/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=PFalkowski_StocksData&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=PFalkowski_StocksData)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://choosealicense.com/licenses/mit/)
[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-support-yellow.svg)](https://www.buymeacoffee.com/piotrfalkowski)

A collection of .NET 8 libraries for working with stock market data (OHLCV format).

## Packages

| Package | Description |
|---------|-------------|
| `Stocks.Data.Model` | Domain model: `StockQuote`, `Company`, `TradingSimulationResult` |
| `Stocks.Data.Infrastructure` | CSV import helpers, OnTheFlyStats aggregation |
| `Stocks.Data.Ef` | EF Core repositories and Unit of Work for stock data |
| `Stocks.Data.Ado` | SQL Server bulk-insert via `SqlBulkCopy` |

## Features

- Abstract base class (`TradingSimulatorBase`) for implementing custom trading simulators.
- Simulation of trading strategies over configurable date ranges and stock universes.
- Integration with repositories for historical stock quotes.
- Progress reporting and logging support.
- Extensible design for custom signal generation and trading logic.

## Getting Started

### Prerequisites

- .NET Standard 2.1 compatible environment (e.g., .NET Core 3.0+, .NET 5+, or .NET Framework 4.8+ with support).
- C# 8.0 or later.

### Usage

1. **Implement a Simulator:**
   Derive from `TradingSimulatorBase` and implement the `GetTopN` method to define your stock selection logic.

2. **Run a Simulation:**
   Use your simulator to run simulations with historical data and configuration.

## Key Classes

- `TradingSimulatorBase`: Abstract base for trading simulators.
- `SimulationResult`: Holds simulation results and statistics.
- `TradingSimulationConfig`: Configuration for simulation runs.
- `StockQuote`, `StockTransaction`: Data models for quotes and transactions.

## Extending

- Implement custom trading strategies by overriding `GetTopN`.
- Use dependency injection for logging, data access, and configuration.