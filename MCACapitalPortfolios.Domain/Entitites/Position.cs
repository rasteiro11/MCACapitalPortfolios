using System.Reflection;
using MCACapitalPortfolios.Domain.Enums;

namespace MCACapitalPortfolios.Domain.Entities;

public class Position {
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public int AssetId { get; set; }
    public PositionType PositionType { get; set; }
    public DateTime DtTimestamp { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }

    public decimal EvaluateAt(DateTime dtTimestamp, decimal priceAt) {
        if(dtTimestamp < DtTimestamp) {
            return decimal.Zero;
        }
        
        return (Price*Quantity)+((priceAt - Price) * Quantity * (PositionType == PositionType.LONG ? 1 : -1));
    }
}