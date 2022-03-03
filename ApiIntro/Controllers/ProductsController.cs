using ApiIntro.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ApiIntro.Controllers
{
    [XmlType(TypeName = "Product")]
    public class ProductDto
    {
        [XmlAttribute("ProductId")]
        public string Id { get; set; }


        [XmlAttribute("ProductName")]
        public string Name { get; set; }

        [XmlElement("UnitPrice")]
        public decimal Price { get; set; }

        [XmlIgnore] // gösterme
        public short Stock { get; set; }

    }


    //[Route("api/[controller]/[action]")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        List<ProductDto> Products
        {
            get
            {
                return new List<ProductDto> {

                new ProductDto
                {
                    Id="1",
                    Name  ="P-1",
                    Stock = 10,
                    Price =20
                },
                new ProductDto
                {
                    Id="2",
                    Name  ="P-2",
                    Stock = 10,
                    Price =20
                }

            };
            }
        }


        // HtrtpGet, HttpPost, HttpPut, HttpDelete içerisinde route yönlendirmesi yapabiliriz.
        [HttpGet("product-json")]
        public IActionResult Products1()
        {

            // dönüş tipi olarak view yok
            // status code 200 demek

            return Ok(new List<ProductDto> {

                new ProductDto
                {
                    Name  ="P-1",
                    Stock = 10,
                    Price =20
                },
                new ProductDto
                {
                    Name  ="P-2",
                    Stock = 10,
                    Price =20
                }

            });
            //return StatusCode(StatusCodes.Status404NotFound);
        }




        [HttpGet("product-xml")]
        [Produces("application/xml")]
        //[Produces("application/json")]
        // apide defulat formatter jsondır.
        public List<ProductDto> Products2()
        {

            // dönüş tipi olarak view yok
            // automatic olarak json serialize ediliyor

            return new List<ProductDto> {

                new ProductDto
                {
                    Id="1",
                    Name  ="P-1",
                    Stock = 10,
                    Price =20
                },
                new ProductDto
                {
                    Id="2",
                    Name  ="P-2",
                    Stock = 10,
                    Price =20
                }

            };
        }


        // {id} süslü parantezler ile tanımladık.
        [HttpGet("{id}")]
        public ProductDto GetProductById(string id)
        {

            var plist = new List<ProductDto> {

                new ProductDto
                {
                    Id="1",
                    Name  ="P-1",
                    Stock = 10,
                    Price =20
                },
                new ProductDto
                {
                    Id="2",
                    Name  ="P-2",
                    Stock = 10,
                    Price =20
                }

            };

            return plist.Where(x => x.Id == id).FirstOrDefault();
        }


        // https://localhost:5001/api/Products?id=2
        // route tanımı yapmazsak otomatik olarak querystring üzerinden get işleminde veri çekebiliriz.// route yok method da parametre tanımı varsa querystring olur.
        [HttpGet]
        public ProductDto GetProductById2(string id)
        {

            var plist = new List<ProductDto> {

                new ProductDto
                {
                    Id="1",
                    Name  ="P-1",
                    Stock = 10,
                    Price =20
                },
                new ProductDto
                {
                    Id="2",
                    Name  ="P-2",
                    Stock = 10,
                    Price =20
                }

            };

            return plist.Where(x => x.Id == id).FirstOrDefault();
        }


        [HttpPatch("{id}")]
        public IActionResult ChangePrice(string id, [FromBody] ChangePriceDto dto)
        {
            if (dto == default(ChangePriceDto))
            {
                return BadRequest(); // 400 bad request hatalı gönderim
            }

            var product = Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
                return NotFound();

            product.Price = dto.Price;

            return NoContent(); // 204 
        }

        // HttpDelete HttpPut isteklerinde 204 no content no navigate yaparız
        // HttpPut işleminde bir verinin tüm değişecek olan alanlarını göndeririz. Eğer partial paçalı bir değişim söz konusu ise HttpPatch kullanırız.
        // HttpPatch de 204 no content döneriz
        // Post işlmelerinde created result döneriz
        // Get işlemlerinde ise Ok result döneriz.
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(string id, UpdateProductDto dto)
        {
            var product = Products.FirstOrDefault(x => x.Id == id);

            // yoksa yeni bir product gir
            if (product == null)
            {
                var productEntity = new ProductDto
                {

                    Id = "3",
                    Name = dto.Name,
                    Price = dto.Price,
                    Stock = dto.Stock

                };

                Products.Add(productEntity);
                // yeni bir resource oluşturuldu.
                return Created(Request.Path, productEntity); // 200 yada
            }

            // product varsa güncelle

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;


            return NoContent(); // 204


        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(string id)
        {
            var product = Products.FirstOrDefault(x => x.Id == id);

            if (product == null)
                return NotFound(); // 404

            Products.Remove(product);

            return NoContent();
        }

        [HttpPost("create")]
        public IActionResult PostProduct([FromBody] ProductCreateDto dto)
        {
            var product = new ProductDto
            {

                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock

            };

            Products.Add(product);

            // endpoint resource yerini location veriririz.

            return Created($"/api/products/{product.Id}", product);
        }



    }
}
