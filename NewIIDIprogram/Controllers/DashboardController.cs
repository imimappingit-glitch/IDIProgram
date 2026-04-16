using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using NewIIDIprogram.Models;

namespace NewIIDIprogram.Controllers
{
    public class DashboardController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        public ActionResult Index(int programId = 0)
        {
            if (programId == 0)
            {
                var firstProgram = db.Programs.FirstOrDefault();
                if (firstProgram != null)
                    programId = firstProgram.ProgramId;
            }

            // Fetch steps
            var steps = db.ProgramSteps
                          .Include(s => s.MasterStep)
                          .Include(s => s.Activities)
                          .Where(s => s.ProgramId == programId)
                          .ToList();

            // Fetch clients assigned to this program using mapping table
            var clients = (from map in db.ClientProgramMappings
                           join client in db.ClientInformations
                           on map.ClientId equals client.ClientId
                           where map.ProgramId == programId
                           select client)
                           .ToList();

            ViewBag.Clients = clients;



            ViewBag.Clients = clients;

            // Pass program name
            ViewBag.ProgramName = db.Programs
                                     .Where(p => p.ProgramId == programId)
                                     .Select(p => p.ProgramName)
                                     .FirstOrDefault();

            return View(steps);
        }


    }
}
