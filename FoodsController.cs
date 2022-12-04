using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Angular_Project_Foodhub_Work_02.Models;
using Angular_Project_Foodhub_Work_02.Repositories.Interfaces;
using Angular_Project_Foodhub_Work_02.ViewModels;
using Angular_Project_Foodhub_Work_02.ViewModels.Input;

namespace Angular_Project_Foodhub_Work_02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        
        private IWebHostEnvironment env;
        IUnitOfWork unitOfWork;
        IGenericRepo<Food> repo;
        public FoodsController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.repo = this.unitOfWork.GetRepo<Food>();
            this.env = env;
        }

        // GET: api/Foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
        {
            var data =  await this.repo.GetAllAsync();
            return data.ToList();
        }
        [HttpGet("VM")]
        public async Task<ActionResult<IEnumerable<FoodViewModel>>> GetFoodViewModels()
        {
            var data = await this.repo.GetAllAsync(fd => fd.Include(x => x.OrderDetails));
            return data.ToList().Select(fd => new FoodViewModel
            {
                FoodID = fd.FoodID,
                Price= fd.Price,
                FoodName = fd.FoodName,
                IsAvailable= fd.IsAvailable,
                CanDelete = !fd.OrderDetails.Any(),
                Picture = fd.Picture

            }).ToList();
        }
        // GET: api/Foods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            var food = await this.repo.GetAsync(x=> x.FoodID == id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

        // PUT: api/Foods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFood(int id, Food food)
        {
            if (id != food.FoodID)
            {
                return BadRequest();
            }

            await this.repo.UpdateAsync(food);

            try
            {
                await this.unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               
                    throw;
                
            }

            return NoContent();
        }
        [HttpPut("{id}/VM")]
        public async Task<IActionResult> PutFoodViewModel(int id, FoodInputModel food)
        {
            if (id != food.FoodID)
            {
                return BadRequest();
            }

            var existing = await this.repo.GetAsync(fd=> fd.FoodID == id);
            if (existing != null)
            {
                existing.FoodName = food.FoodName;
                existing.Price= food.Price;
                existing.IsAvailable= food.IsAvailable;
                await this.repo.UpdateAsync(existing);
            }

            try
            {
                await this.unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }

            return NoContent();
        }
        // POST: api/Foods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Food>> PostFood(Food food)
        {
           await this.repo.AddAsync(food);
           await this.unitOfWork.CompleteAsync();

           return food;
        }
        /// <summary>
        // to insert food without image
        ///
        [HttpPost("VM")]
        public async Task<ActionResult<Food>> PostFoodInput(FoodInputModel food)
        {
            var newFood = new Food
            {
                FoodName = food.FoodName,
                Price = food.Price,
                IsAvailable = food.IsAvailable,
                Picture = "no-product-image-400x400.png"
            };
            await this.repo.AddAsync(newFood);
            await this.unitOfWork.CompleteAsync();

            return newFood;
        }
        [HttpPost("Upload/{id}")]
        public async Task<ImagePathResponse> UploadPicture(int id, IFormFile picture)
        {
            var food = await this.repo.GetAsync(fd=> fd.FoodID == id);  
            var ext = Path.GetExtension(picture.FileName);
            string fileName = Guid.NewGuid() + ext;
            string savePath = Path.Combine(this.env.WebRootPath, "Pictures", fileName);
            FileStream  fs = new FileStream(savePath, FileMode.Create);
            picture.CopyTo(fs);
            fs.Close();
            food.Picture = fileName;
            await this.repo.UpdateAsync(food);
            await this.unitOfWork.CompleteAsync();
            return new ImagePathResponse { PictureName = fileName };
        }
        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await this.repo.GetAsync(fd=> fd.FoodID == id);
            if (food == null)
            {
                return NotFound();
            }

            await this.repo.DeleteAsync(food);
            await this.unitOfWork.CompleteAsync();

            return NoContent();
        }

        
    }
}
