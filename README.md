
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