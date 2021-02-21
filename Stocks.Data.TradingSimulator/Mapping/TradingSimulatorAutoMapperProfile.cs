using AutoMapper;
using Stocks.Data.Model;
using Stocks.Data.TradingSimulator.Models;
using System;
using System.Linq;

namespace Stocks.Data.TradingSimulator.Mapping
{
    public class TradingSimulatorAutoMapperProfile : Profile
    {
        /// <inheritdoc />
        public TradingSimulatorAutoMapperProfile()
        {
            CreateMap<SimulationResult, TradingSimulationResult>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SimulationDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.InitialInvestment,
                    opt => opt.MapFrom(src => src.TradingSimulationConfig.StartingCash))
                .ForMember(dest => dest.TopN, opt => opt.MapFrom(src => src.TradingSimulationConfig.TopN))
                .ForMember(dest => dest.TruePositives, opt => opt.MapFrom(src => src.ROC.TruePositives))
                .ForMember(dest => dest.FalseNegatives, opt => opt.MapFrom(src => src.ROC.FalseNegatives))
                .ForMember(dest => dest.TrueNegatives, opt => opt.MapFrom(src => src.ROC.TrueNegatives))
                .ForMember(dest => dest.FalsePositives, opt => opt.MapFrom(src => src.ROC.FalsePositives))
                .ForMember(dest => dest.TotalBuyOrders,
                    opt => opt.MapFrom(src =>
                        src.TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Buy)))
                .ForMember(dest => dest.TotalSellOrders,
                    opt => opt.MapFrom(src =>
                        src.TransactionsLedger.Count(x => x.TransactionType == StockTransactionType.Sell)))
                .ForMember(dest => dest.FromDateInclusive,
                    opt => opt.MapFrom(src => src.TradingSimulationConfig.FromDate))
                .ForMember(dest => dest.ToDateInclusive, opt => opt.MapFrom(src => src.TradingSimulationConfig.ToDate))
                .ForMember(dest => dest.ExcludePennyStocks,
                    opt => opt.MapFrom(src => src.TradingSimulationConfig.ExcludePennyStocks))
                .ForMember(dest => dest.ExcludePennyStocksThreshold,
                    opt => opt.MapFrom(src => src.TradingSimulationConfig.ExcludePennyStocksThreshold));
        }
    }
}
