using Microsoft.AspNetCore.Mvc;
using Planner.Application.Products.Queries.GetListOfProducts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class ProductController : BaseController
    {
        [HttpPost]
        public async Task<IEnumerable<ProductListItem>> GetListOfProducts(GetListOfProductsQuery request)
        {
            return await this.Mediator.Send(request);
        }
    }
}
