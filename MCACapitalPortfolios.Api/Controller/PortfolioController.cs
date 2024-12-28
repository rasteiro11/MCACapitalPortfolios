using MCACapitalPortfolios.Application.Abstractions.Services;
using MCACapitalPortfolios.Contracts.Portfolio;
using MCACapitalPortfolios.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]/[action]")]
[ApiController]
public class PortfolioController : ControllerBase {
    private readonly IPortfoliosService portfoliosService;
    public PortfolioController(IPortfoliosService portfoliosService)
    {
        this.portfoliosService = portfoliosService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioRequest request) {
        try {
            var p = await portfoliosService.CreatePortfolioAsync(request.Description, request.Positions is not null ? request.Positions.Select(pos => {
                return new Position() {
                    AssetId=pos.AssetId,
                    DtTimestamp=pos.DtTimestamp,
                    PositionType=pos.PositionType,
                    Price=pos.Price,
                    Quantity=pos.Quantity,
                };
            }).ToArray() : []);

            return Ok(p);
        } catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{portfolioId}")]
    public async Task<IActionResult> GetPortfolio(int portfolioId) {
        try {
            var p = await portfoliosService.GetPortfolioByIdAsync(portfolioId);
            if(p is null) {
                return NotFound();
            }

            return Ok(p);
        } catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{portfolioId}")]
    public async Task<IActionResult> DeletePortfolio(int portfolioId) {
        try {
            await portfoliosService.DeletePortfolioByIdAsync(portfolioId);

            return Ok();
        } catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> EvaluatePortfolioAt([FromBody] EvaluatePortfolioAtRequest evaluatePortfolioAtRequest) {
        try {
            return Ok(await portfoliosService.EvaluatePortfolioAt(evaluatePortfolioAtRequest.DtReference, evaluatePortfolioAtRequest.PortfolioId, evaluatePortfolioAtRequest.Store));
        } catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedPortfolios([FromQuery] int page, [FromQuery] int pageSize) {
        try {
            return Ok(await portfoliosService.GetPaginatedPortfolios(page, pageSize));
        } catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> EvaluatePortfolioAtRange([FromBody] EvaluatePortfolioAtRangeRequest evaluatePortfolioAtRangeRequest) {
        try {
            return Ok(await portfoliosService.EvaluatePortfolioAtInRange(evaluatePortfolioAtRangeRequest.DtStart, evaluatePortfolioAtRangeRequest.DtEnd, evaluatePortfolioAtRangeRequest.PortfolioId, evaluatePortfolioAtRangeRequest.Store));
        } catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPortfolioHistoryInRange([FromQuery] int portfolioId, [FromQuery] DateTime? dtStart = null, [FromQuery] DateTime? dtEnd = null) {
        try {
            return Ok(await portfoliosService.GetPortfolioHistoryInRange(portfolioId, dtStart, dtEnd));
        } catch(Exception e) {
            return BadRequest(e.Message);
        }
    }
}