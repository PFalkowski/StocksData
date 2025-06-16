# Stocks.Data.TradingSimulator

A .NET Standard 2.1 library for simulating stock trading strategies using historical market data.

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