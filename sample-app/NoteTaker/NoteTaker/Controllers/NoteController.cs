using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NoteTaker.Models;
using Newtonsoft.Json;
using System.Net;

namespace NoteTaker.Controllers
{
    public class NoteController : Controller
    {
        //
        // GET: /Note/

        public ActionResult Index()
        {
            var json = Couch.Uri.Get("_all_docs?include_docs=true");
            var notes = JsonConvert.DeserializeObject<DocumentCollection<Note>>(json);
            
            return View(notes.Items.Select(i=>i.Item));
        }

        public ActionResult Create()
        {

            var note = new Note() { Id = Guid.NewGuid() };
            return View("Edit", note);
        }

        public ActionResult Edit(Guid id)
        {
            var json=Couch.Uri.Get(id);
            return View(JsonConvert.DeserializeObject<Note>(json));
        }

        [HttpPost]
        public ActionResult Edit(Note note)
        {
            try
            {
                Couch.Uri.Put(note.Id, JsonConvert.SerializeObject(note));
                return RedirectToAction("Edit", new { id = note.Id });
            }
            catch (WebException ex)
            {
                var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                if (statusCode != HttpStatusCode.Conflict)
                    throw;

                ModelState.AddModelError("Revision", "Conflict Detected");
                return View(note);
            }
            
        }

        public ActionResult Delete(Guid id,string rev)
        {
            var json = Couch.Uri.Delete(id,rev);
            return RedirectToAction("Index");
        }


    }
}
