using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    public class CatalogController : BaseApiController
    {
        private readonly IMediator _mediator;
        public CatalogController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [Route("[action]/{id}", Name = "GetProductById")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetProductById(string id)
        {
            var query = new GetProductsByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{productName}", Name = "GetProductByProductName")]
        [ProducesResponseType(typeof(IList<ProductResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetProductByProductName(string productName)
        {
            var query = new GetProductsByNameQuery(productName);
            var result = await _mediator.Send(query);
            if (result == null || !result.Any()) return NotFound();
            return Ok(result);
        }

        // GetAllProducts
        [HttpGet]
        [Route("GetAllProducts")]
        [ProducesResponseType(typeof(IList<ProductResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetAllProducts()
        {
            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);
            if (result == null || !result.Any()) return NotFound();
            return Ok(result);
        }
        // Create Product
        [HttpPost]
        [Route("CreateProduct")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductCommand productCommand)
        {
            var result = await _mediator.Send<ProductResponseDto>(productCommand);
            return Ok(result);
        }
        // update product
        [HttpPut]
        [Route("UpdateProduct")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct([FromBody] UpdateProductCommand productCommand)
        {
            var result = await _mediator.Send<bool>(productCommand);
            return Ok(result);
        }
        // delete product
        [HttpPut]
        [Route("DeleteProduct")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponseDto>> DeleteProduct(string id)
        {
            var result = await _mediator.Send<bool>(new DeleteProductCommand(id));
            return Ok(result);
        }
    }
}
