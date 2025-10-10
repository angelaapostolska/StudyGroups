using System.Configuration;
using System.Web.Mvc;

public class SeederController : Controller
{
    // GET: Seeder
    public ActionResult Index()
    {
        return View();
    }

    // POST: Seeder/SeedData
    [HttpPost]
    public ActionResult SeedData()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var seeder = new DatabaseSeeder(connectionString);
            seeder.SeedDatabase();

            ViewBag.Message = "Database seeded successfully! Added 15 Study Groups, 50 Sessions, and 100 Ratings.";
            ViewBag.Success = true;
        }
        catch (System.Exception ex)
        {
            ViewBag.Message = "Error: " + ex.Message;
            ViewBag.Success = false;
        }

        return View("Index");
    }
}