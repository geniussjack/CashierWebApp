using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CashierWebApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashierController : ControllerBase
    {
        [HttpPost("print")]
        public IActionResult PrintReceipt([FromBody] List<Product> product)
        {

            var receiptStrings = new List<string>();
            foreach (var item in product)
            {
                receiptStrings.Add($"{item.Name} ({item.Amount} шт.)\nСтоимость..............................{item.Amount * item.Price}\n");
            }
            
            return Ok(receiptStrings);
        }
    }
}
