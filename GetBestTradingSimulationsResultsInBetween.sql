select * from TradingSimulationResult
where LowerPriceChangeFilter between 0.0099 and 0.011 and UpperPriceChangeFilter between 0.0699 and 0.0711
order by ReturnOnInvestment desc