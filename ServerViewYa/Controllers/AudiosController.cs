using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerViewYa.Database;
using ServerViewYa.Models;
using System.Text;
using System.Net;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;


namespace ServerViewYa.Controllers
{
    public class AudiosController : Controller
    {
        private readonly ApplicationContext _context;
        readonly HttpClient client;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AudiosController(ApplicationContext context,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization",
               "Api-Key AQVNy0iP5LuvWAGUERHMukcmZkDeZisazhg-393d");
        }

        // GET: Audios
        public async Task<IActionResult> Index()
        {
            return _context.audios != null ?
                        View(await _context.audios.ToListAsync()) :
                        Problem("Entity set 'ApplicationContext.audios'  is null.");
        }

        // GET: Audios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.audios == null)
            {
                return NotFound();
            }

            var audio = await _context.audios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (audio == null)
            {
                return NotFound();
            }

            return View(audio);
        }

        // GET: Audios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Audios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AudioId,Status,Text")] Audio audio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(audio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(audio);
        }

        // GET: Audios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.audios == null)
            {
                return NotFound();
            }

            var audio = await _context.audios.FindAsync(id);
            if (audio == null)
            {
                return NotFound();
            }
            return View(audio);
        }











        ///////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> Restart(int? id)
        {
            System.Diagnostics.Debug.WriteLine("STARTTTT");
            StringBuilder stringBuilder = new StringBuilder();
            var audio = await _context.audios.FindAsync(id);
            if (audio == null)
            {
                return NotFound();
            }

            var result = await client.GetAsync($"https://operation.api.cloud.yandex.net/operations/{audio.AudioId}");
            var resp = await result.Content.ReadFromJsonAsync<Resp>();
            if(resp.done != true)
            {
                return Ok("ЖДИ");
            }
            foreach (var item in resp.response.chunks)
            {
                stringBuilder.Append(item.alternatives[0].text + " ");
            }
            audio.Text = stringBuilder.ToString();

            _context.audios.Update(audio);
            await _context.SaveChangesAsync();

            return Ok("Good");
        }


        public async Task<IActionResult> GetPDF(int id)
        {
            var audio = await _context.audios.FindAsync(id);
            if (audio == null)
            {
                return NotFound();
            }

            const string file_type = "application/pdf";
            const string file_name = "sample.pdf";

            PdfDocumentBuilder builder = new PdfDocumentBuilder();

            PdfPageBuilder page = builder.AddPage(PageSize.A4);

            string rootPath = _hostingEnvironment.ContentRootPath;

            var bytes = System.IO.File.ReadAllBytes(
                Path.Combine(rootPath, "wwwroot" , "Roboto-Regular.ttf"));

            var font = builder.AddTrueTypeFont(bytes);

            page.AddText(audio.Text, 12, new PdfPoint(10, 700), font);

            byte[] documentBytes = builder.Build();

            return File(documentBytes, file_type, file_name);
        }










        // POST: Audios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AudioId,Status,Text")] Audio audio)
        {
            if (id != audio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(audio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AudioExists(audio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(audio);
        }

        // GET: Audios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.audios == null)
            {
                return NotFound();
            }

            var audio = await _context.audios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (audio == null)
            {
                return NotFound();
            }

            return View(audio);
        }

        // POST: Audios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.audios == null)
            {
                return Problem("Entity set 'ApplicationContext.audios'  is null.");
            }
            var audio = await _context.audios.FindAsync(id);
            if (audio != null)
            {
                _context.audios.Remove(audio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AudioExists(int id)
        {
            return (_context.audios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    record Alter(string text);

    record Resp(bool done, ResponseYa response, string id);

    record ResponseYa(List<Chanks> chunks);

    record Chanks(List<Alternatives> alternatives);

    record Alternatives(string text);
}
