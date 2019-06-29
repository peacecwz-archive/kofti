using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kofti.Manager.Data;
using Kofti.Manager.Data.Entities;
using Kofti.Manager.Models;
using Kofti.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kofti.Manager.Controllers
{
    [Route("configs/{appId}")]
    public class ConfigsController : Controller
    {
        private readonly KoftiDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfigService _configService;

        public ConfigsController(KoftiDbContext dbContext, IMapper mapper, IConfigService configService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configService = configService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int appId)
        {
            ViewBag.AppId = appId;
            var configs = await _dbContext.Configs
                .Where(x => !x.IsDeleted
                            && x.ApplicationId == appId)
                .ToListAsync();

            return View(configs.Select(x => _mapper.Map<ConfigEnttiy, ConfigModel>(x)).ToList());
        }

        [HttpGet("create")]
        public IActionResult Create(int appId)
        {
            ViewBag.AppId = appId;
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(int appId, ConfigModel model)
        {
            bool isExists = await _dbContext.Configs.AnyAsync(x => x.Key == model.Key && x.ApplicationId == appId);
            if (isExists)
            {
                return RedirectToAction("Index", new
                {
                    appId
                });
            }

            var entity = _mapper.Map<ConfigModel, ConfigEnttiy>(model);
            entity.ApplicationId = appId;
            await _dbContext.Configs.AddAsync(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
                await ReloadConfig(appId);
            }
            catch
            {
                return View(model);
            }

            return RedirectToAction("Index", new
            {
                appId
            });
        }

        [HttpGet("update/{configId}")]
        public async Task<IActionResult> Update(int appId, int configId)
        {
            var configEntity =
                await _dbContext.Configs.SingleOrDefaultAsync(x =>
                    x.Id == configId && x.ApplicationId == appId && !x.IsDeleted);

            if (configEntity == null)
            {
                return RedirectToAction("Index", new
                {
                    appId
                });
            }

            ViewBag.AppId = appId;

            return View(_mapper.Map<ConfigEnttiy, ConfigModel>(configEntity));
        }

        [HttpPost("update/{configId}")]
        public async Task<IActionResult> Update(int appId, int configId, ConfigModel model)
        {
            var configEntity =
                await _dbContext.Configs.SingleOrDefaultAsync(x =>
                    x.Id == configId && x.ApplicationId == appId && !x.IsDeleted);

            if (configEntity == null)
            {
                return RedirectToAction("Index");
            }

            configEntity.Value = model.Value;
            configEntity.IsActive = model.IsActive;

            _dbContext.Configs.Update(configEntity);

            try
            {
                await _dbContext.SaveChangesAsync();

                await ReloadConfig(appId);
            }
            catch
            {
                return View(model);
            }

            return RedirectToAction("Index", new
            {
                appId
            });
        }

        public IActionResult Delete()
        {
            throw new System.NotImplementedException();
        }

        async Task ReloadConfig(int appId)
        {
            var app = await _dbContext.Applications.Include(x => x.Configs)
                .SingleOrDefaultAsync(x => x.Id == appId);

            await _configService.PublishAsync(app.Name,
                app.Configs.ToDictionary(x => x.Key, x => (object) x.Value));
        }
    }
}