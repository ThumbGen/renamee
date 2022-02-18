using Microsoft.AspNetCore.Mvc;
using renamee.Server.Repositories;
using renamee.Shared.Models;

namespace renamee.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository settingsRepository;

        public SettingsController(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        [HttpGet]
        public Settings Get()
        {
            return settingsRepository.Load();
        }

        [HttpPut]
        public IActionResult Put(Settings settings)
        {
            if (!ModelState.IsValid)
            {
                return new StatusCodeResult(412);
            }
            settingsRepository.Save(settings);
            return NoContent();
        }
    }
}
