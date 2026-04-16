using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NewIIDIprogram.Models;
using System.Data.Entity;
using PagedList;

namespace NewIIDIprogram.Controllers
{
    public class ClientInformationsController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();
        // GET: ClientInformations
        public async Task<ActionResult> Index(
      string Biomatric,
      string FirstName,
      string LastName,
      int? page)
        {
           
            const int pageSize = 20;
            int pageNumber = page ?? 1;

            var query = db.ClientInformations.AsQueryable();

            //if (Session["UserType"].ToString() == "Consultant")
            //{
            //    var consultantId = Session["id"]?.ToString();
            //    query = query.Where(c => c.RegisterdBy == consultantId);
            //}

            if (!string.IsNullOrWhiteSpace(Biomatric))
            {
                query = query.Where(c => c.Biomatric.Contains(Biomatric));
                ViewBag.Biomatric = Biomatric;
            }

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                query = query.Where(c => c.FristName.Contains(FirstName));
                ViewBag.FirstName = FirstName;
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                query = query.Where(c => c.LastName.Contains(LastName));
                ViewBag.LastName = LastName;
            }

            query = query.OrderByDescending(c => c.ClientId);

            var list = await query.ToListAsync();
            var pagedEntities = list.ToPagedList(pageNumber, pageSize);

            return View(pagedEntities);
        }

        //GET: ClientInformations/Details/5
    //    public async Task<ActionResult> Details(int? id)
    //    {
    //        try
    //        {
    //            if (id == null)
    //            {
    //                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //            }
    //            ClientInformation clientInformation = await db.ClientInformations.FindAsync(id);
    //            clientInformation.Fingureprits = await db.BiomatricFingerprintMappings
    //.Where(entity => entity.BiomatricLogId == clientInformation.Biomatric)
    //.OrderBy(c => c.Hand)
    //.ToListAsync();

    //            Session["Biomatric"] = clientInformation.Biomatric;

    //            //  clientInformation.BiomatricFingerprintMapping = await db.BiomatricFingerprintMappings.FindAsync(clientInformation.Biomatric);
    //            if (clientInformation == null)
    //            {
    //                return HttpNotFound();
    //            }
    //            //take consultant id
    //            //string consultantId = clientInformation.ReferenceID;
    //            //check seen or not 

    //            return View(clientInformation);
    //        }
    //        catch (Exception)
    //        {
    //            Console.WriteLine($"An error occurred: {"An error occurred while processing your request. Please try again later."}");
    //            return RedirectToAction("home");
    //        }
    //    }
    }
}