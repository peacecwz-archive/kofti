using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kofti.Manager.Data;
using Kofti.Manager.Data.Entities;
using Kofti.Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kofti.Manager.Controllers
{
    [Route("/apps")]
    public class AppsController : Controller
    {
        private readonly KoftiDbContext _dbContext;
        private readonly IMapper _mapper;
        public AppsController(KoftiDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // GET
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var apps = await _dbContext.Applications.Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
            return View(apps.Select(x => _mapper.Map<ApplicationEntity, AppModel>(x)).ToList());
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(AppModel app)
        {
            var entity = _mapper.Map<AppModel, ApplicationEntity>(app);
            await _dbContext.Applications.AddAsync(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return View(app);
            }

            return RedirectToAction("Index");
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int appId)
        {
            var entity = await _dbContext.Applications.FindAsync(appId);

            if (entity == null)
            {
                return RedirectToAction("Index");
            }

            _dbContext.Applications.Remove(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // ignored
            }

            return RedirectToAction("Index");
        }
    }
}